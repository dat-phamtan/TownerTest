#nullable enable
//using UnityEngine;
using Assets.Scripts.Config;
using Assets.Scripts.Data;
using Assets.Scripts.Entity;
using Assets.Scripts.FlightStates;
using Assets.Scripts.Generator;
using Assets.Scripts.IO;
using Assets.Scripts.Logger;
using Assets.Scripts.Manager;
using Assets.Scripts.Unity;
using ControlTowner.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using static UnityEngine.Rendering.STP;

namespace Assets.Scripts.Controller
{
    public class FlightController : IFlightController
    {
        private readonly int _initHour;
        private readonly int _initMinute;
        private bool _maintenanceMode = false;

        private readonly Queue<Flight> _takeoffQueue = new();
        private readonly Queue<Flight> _landingQueue = new();

        private readonly object _queueLock = new();
        private readonly object _processLock = new();


        private IRunwayManager _runwayManager;
        private readonly IConfig _config;
        private readonly ILandingGenerator _generator;
        private readonly IStorageManager _storageManager;
        private IAirportState? _currentState;
        private readonly Logger.ILogger _logger;
        //private float _timeScale = 1000.0f;

        private List<Flight> _todaySchedule = new();
        private List<Flight> _unfinishedSchedule = new();

        //IMPLEMENT INTERFACE
        public event Action<List<Flight>>? OnScheduleUpdated;
        public event Action? OnLogDiary;
        public event Action<bool>? OnStatusChanged;
        public event Action? OnRunwayInit;
        public event Action<int>? OnLandingQueueChanged;
        public event Action<int>? OnTakeoffQueueChanged;

        public event Action<Runway>? OnPreLanding;
        public event Action? OnPreTakeoff;

        public event Action<Runway>? OnFlightLanding;
        public event Action<int>? OnFlightTakeoff;

        //maybe no need
        public event Action? OnPostLanding;
        public event Action? OnPostTakeoff;

        public bool IsMaintenanceMode()
        {
            return _maintenanceMode;
        }


        public Runway[]? GetRunways()
        {
            return _runwayManager.GetRunways();
        }

        //public ScreenData GetScreenData()
        //{
        //    return _screenData;
        //}

 

        // INITIALIZE
        public FlightController(IConfig config, ILandingGenerator generator, IStorageManager storageManager, IRunwayManager runwayManager, Logger.ILogger logger, int initHour, int initMinute)
        {
            _initHour = initHour;
            _initMinute = initMinute;
            _config = config;
            _generator = generator;
            _storageManager = storageManager;
            _runwayManager = runwayManager;
            _logger = logger;
        }

        public void LoadData()
        {
            _config.Load(_logger);
            _runwayManager.Init(_config.Get().RunwayCount);
            
            MaintenanceModeInit();
        }

        public void StartSimulation()
        {
            SimpleClock.Instance.InitClock(_initHour, _initMinute, _config.Get().TimeScale, _config.Get().MaintenancePeriod);

            OnRunwayInit?.Invoke();
            OnStatusChanged?.Invoke(!_maintenanceMode);

            _runwayManager.OnBecomeAvailable += ProcessQueues;
            SimpleClock.Instance.OnTick += HandleClockTick;
            SimpleClock.Instance.OnMaintenanceStart += HandleMaintenanceStart;
            SimpleClock.Instance.OnNewDayStart += HandleNewDayStart;
        }


        public void MaintenanceModeInit()
        {
            var maintenancePeriod = _config.Get().MaintenancePeriod;
            var startMaintenance = new TimeSpan(maintenancePeriod.MaintenanceStartHour, maintenancePeriod.MaintenanceStartMinute, 0);
            var endMaintenance = new TimeSpan(maintenancePeriod.MaintenanceEndHour, maintenancePeriod.MaintenanceEndMinute, 0);
            var target = new TimeSpan(_initHour, _initMinute, 0);

            if (IsTimeOfDayBetween(target, startMaintenance, endMaintenance))
            {
                _maintenanceMode = true;
                _currentState = new NormalAirportState();
            }
            else
            {
                _maintenanceMode = false;
                _currentState = new MaintenanceAirportState();
            }
        }


        public bool IsTimeOfDayBetween(TimeSpan target, TimeSpan start, TimeSpan end)
        {
            if (start <= end)
                return target >= start && target <= end;
            else
                return target >= start || target <= end;
        }


        // LOAD SCHEDULE
        public void LoadSchedule(DateTime today)
        {
            var listSchedule = _storageManager.LoadDailySchedule(today);
            _todaySchedule = ConvertToFlightList(listSchedule);
            _unfinishedSchedule.Clear();
            //_logger.Log($"[ATC] Today's schedule count: {_todaySchedule.Count} ");
            OnScheduleUpdated?.Invoke(_todaySchedule);
        }


        private List<Flight> ConvertToFlightList(List<FlightSchedule> listSchedule)
        {
            var flights = new List<Flight>();
            foreach (var schedule in listSchedule)
            {
                var flight = new Flight(schedule, FlightType.Takeoff, FlightState.Waiting, _logger);
                flights.Add(flight);
            }
            return flights;
        }


        //check 
        private void DispathScheduledFights(DateTime now)
        {
            var toDispath = new List<Flight>();
            for (int i = 0; i < _todaySchedule.Count; i++)
            {
                if (IsDispatched(_todaySchedule[i], now))
                {
                    toDispath.Add(_todaySchedule[i]);
                }
            }

            foreach (var flight in toDispath)
            {
                flight.State = FlightState.Operating;
                _logger.Log($"[ATC] Takeoff requirement: {flight.FlightSchedule.Code} ({flight.FlightSchedule:HH:mm})");
                EnqueueTakeoff(flight);
            }
        }

        private bool IsDispatched(Flight flight, DateTime now)
        {
            return flight.State == FlightState.Waiting && flight.FlightSchedule.ScheduleTime <= now;
        }


        public void EnqueueTakeoff(Flight flight)
        {
            lock (_queueLock)
            {
                _takeoffQueue.Enqueue(flight);
                OnTakeoffQueueChanged?.Invoke(_takeoffQueue.Count);
            }
            _logger.Log($"[SYSTEM] Takeoff queue append: {flight.FlightSchedule.Code}");

            ProcessQueues();
        }

        public void EnqueueLanding(Flight flight)
        {
            lock (_queueLock)
            {
                _landingQueue.Enqueue(flight);
                OnLandingQueueChanged?.Invoke(_landingQueue.Count);
            }
            _logger.Log($"[SYSTEM] Landing queue append: {flight.FlightSchedule.Code}");
            ProcessQueues();
        }


        public void ProcessQueues()
        {
            lock (_processLock)
            {
                lock (_queueLock)
                {
                    while (true)
                    {
                        if (_landingQueue.Count == 0 && _takeoffQueue.Count == 0) break;

                        Runway? runway = _runwayManager.GetAvailableRunway();
                        if (runway == null) break;

                        Flight? flight = null;
                        if (_landingQueue.TryDequeue(out Flight? lf))
                        {
                            flight = lf;
                            OnLandingQueueChanged?.Invoke(_landingQueue.Count);
                        }
                            
                        else if (_takeoffQueue.TryDequeue(out Flight? tf))
                        {
                            flight = tf;
                            OnTakeoffQueueChanged?.Invoke(_takeoffQueue.Count);
                        }

                        if (flight != null && runway.AssignFlight(flight))
                        {
                            float landingDuration = _config.Get().Durations.LandingDuration;
                            float takeoffDuration = _config.Get().Durations.TakeoffDuration;
                            runway.RealDuration = (flight.Type == FlightType.Landing) ? landingDuration : takeoffDuration;

                            OnPreLanding?.Invoke(runway); //prelanding/preta
                            ExecuteFlight(runway, flight);
                        }
                    }
                }
            }
        }


        private void ExecuteFlight(Runway runway, Flight flight)
        {
            flight.OnRequestConfirmation += ATCHandleFlightConfirm;
            flight.OnActionCompleted += ATCHandleFlightComplete;
            Task.Run(() => flight.ExecuteActionAsync(runway, runway.RealDuration));
        }


        private async Task ATCHandleFlightConfirm(Flight flight)
        {
            if (flight.Type == FlightType.Landing) 
                _currentState?.HandleLanding(flight, _logger);
            else 
                _currentState?.HandleTakeoff(flight, _logger);

            await Task.CompletedTask;
        }
    

        private void ATCHandleFlightComplete(Runway runway, Flight flight)
        {
            string action = (flight.Type == FlightType.Takeoff) ? "took off" : "landed";
            _logger.Log($"[ATC] Completed: Flight {flight.FlightSchedule.Code} {action} ({runway.Id})");

            char flightType = (flight.Type == FlightType.Takeoff) ? 'T' : 'L';
            DateTime time = SimpleClock.Instance.SimulatedTime;
            var flightDiary = new FlightDiary(flight.FlightSchedule.Code, runway.Id, flightType, time);
            _storageManager.SaveDailyLog(flightDiary);
            Task.Run(runway.Free);
        }


        private void HandleClockTick(DateTime simulatedTime)
        {
            if (!_maintenanceMode)
            {
                var landing = _generator.CheckGenerate(simulatedTime, _logger);
                if (landing != null)
                {
                    _logger.Log($"[SYSTEM] Generated landing flight: {landing.FlightSchedule.Code}");
                    EnqueueLanding(landing);
                }
            }
            DispathScheduledFights(simulatedTime);
        }


        private void HandleMaintenanceStart()
        {
            _maintenanceMode = true;
            _currentState = new MaintenanceAirportState();
            OnStatusChanged?.Invoke(false);
            for (int i = 0; i < _todaySchedule.Count; i++)
            {
                if (_todaySchedule[i].State == FlightState.Waiting)
                {
                    _unfinishedSchedule.Add(_todaySchedule[i]);
                }
            }
            Task.Run(GenerateTomorrowSchedule);
            Task.Run(WaitForAllFlightsCompleted);
        }


        private void HandleNewDayStart()
        {
            _maintenanceMode = false;
            _currentState = new NormalAirportState();
            OnStatusChanged?.Invoke(true);
            _generator.Reset();

            var today = SimpleClock.Instance.SimulatedTime.Date;
            _logger.Log($"[ATC] Start new day ({today:dd/MM/yyyy})");
            LoadSchedule(today);
            OnLogDiary?.Invoke();
        }


        private async void GenerateTomorrowSchedule()
        {
            _storageManager.GenerateDailySchedule(_config.Get().MaintenancePeriod, _unfinishedSchedule);
            _logger.Log($"[SYSTEM] Generated schedule for tomorrow");
        }


        private async Task WaitForAllFlightsCompleted()
        {
            while (true)
            {
                bool queueEmpty;
                lock (_queueLock)
                {
                    queueEmpty = _landingQueue.Count == 0 && _takeoffQueue.Count == 0;
                }

                if (queueEmpty && _runwayManager.AllRunwayEmpty())
                {
                    _logger.Log($"[ATC] All flight completed!!");
                    break;
                }
                await Task.Delay(500);
            }
        }
    }
}

