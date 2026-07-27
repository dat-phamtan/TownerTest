using Assets.Scripts.Controller;
using Assets.Scripts.Data;
using Assets.Scripts.Entity;
using Assets.Scripts.FlightPool;
using Assets.Scripts.Logger;
using Assets.Scripts.Scenes;
using Assets.Scripts.Unity;
using Assets.Scripts.Utility;
using ControlTowner.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Assets.Scripts.Manager
{
    public class DisplayManager
    {
        private IFlightController _controller;
        private IUIManager _uiManager;
        private IFlightPool _flightPool;
        private ScreenData _screenData;
        private List<Flight> _schedule;
        private GameObject[] _runways;
        //temp
        private List<float> _runwayXPos;

        private readonly float _diaryIntervalSeconds = 1f;
        private readonly List<string> _logBuffer = new();
        private readonly Dictionary<int, FlightView> _activeFlights = new();

        private static readonly object _logLock = new();
        private static readonly object _consoleLock = new();
        private static readonly object _scheduleLock = new();

        public DisplayManager(IFlightController controller, IUIManager uiManager, ILogSource logSource = null)
        {
            logSource.OnLog += AddLog;
            _uiManager = uiManager;
            _controller = controller;
            _runwayXPos = Locator.Get<List<float>>();
            _flightPool = Locator.Get<IFlightPool>();
            _screenData = Locator.Get<ScreenData>();

            _controller.OnScheduleUpdated += HandleSchedule;
            _controller.OnLogDiary += HandleYesterdayDiary;

            _controller.OnStatusChanged += HandleStatusChanged;
            _controller.OnRunwayInit += HandleRunwayInit;
            _controller.OnLandingQueueChanged += HandleLandingQueueChanged;
            _controller.OnTakeoffQueueChanged += HandleTakeoffQueueChanged;

            _controller.OnPreLanding += HandlePreFlight;
            _controller.OnPreTakeoff += HandlePreFlight;

            _controller.OnFlightLanding += HandleStartMovement;
            _controller.OnFlightTakeoff += HandleStartMovement;

            _controller.OnPostLanding += HandleFlightCompleted;
            _controller.OnPostTakeoff += HandleFlightCompleted;

        }

        private void HandleFlightCompleted(Runway runway)
        {
            var id = runway.Id;

            Dispatcher.Enqueue(() =>
            {
                if (_activeFlights.TryGetValue(id, out var flightView))
                {
                    _flightPool.ReleaseFlight(flightView);
                    _activeFlights.Remove(id);
                }
            });
        }

        private void HandleStartMovement(Runway runway)
        {
            var id = runway.Id;
            var travelDistance = runway.RunwayLong;
            var realDuration = runway.RealDuration;
            var type = runway.CurrentFlight.Type;
            Dispatcher.Enqueue(() =>
            {
                if (!_activeFlights.TryGetValue(id, out var flightView)) return;
                float speed = travelDistance / realDuration;

                if (type == FlightType.Landing)
                    flightView.StartLanding(speed);
                else
                    flightView.StartTakeoff(speed);
            });
        }

        private void HandlePreFlight(Runway runway)
        {
            var flight = runway.CurrentFlight;
            var waitingPos = GetWaitingPosition(runway, flight.Type);
            var runwayCenter = runway.Position;
            var id = runway.Id;
            Dispatcher.Enqueue(() =>
            {
                if (flight == null) return;
                var flightView = _flightPool.GetFlight(waitingPos, Quaternion.identity, 0f);
                flightView.InitData(flight, runwayCenter, 0f);
                flightView.StartWaiting(waitingPos, flight.FlightSchedule.Code);
                _activeFlights[id] = flightView;
                //Debug.Log(_activeFlights);
            });
        }

        private void HandleTakeoffQueueChanged(int queueCount)
        {
            Dispatcher.Enqueue(() => _uiManager.ChangeTakeoffQueue("x" + queueCount));
        }

        private void HandleLandingQueueChanged(int queueCount)
        {
            Dispatcher.Enqueue(() => _uiManager.ChangeLandingQueue("x" + queueCount));
        }

        private void HandleStatusChanged(bool status)
        {
            Dispatcher.Enqueue(() =>
            {
                if (status)
                    _uiManager.ChangeStatus("Working");
                else
                    _uiManager.ChangeStatus("Maintenance");
            });
        }

        private void HandleRunwayInit()
        {
            Dispatcher.Enqueue(() => _uiManager.ActiveRunways());
        }

        private Vector3 GetWaitingPosition(Runway runway, FlightType type)
        {
            float x = runway.Position.x;
            float y = runway.RunwayLong/2;
            if (type == FlightType.Takeoff) 
                y = -y;
            return new Vector3(x, y, 0);
        }

        private void HandleSchedule(List<Flight> flights)
        {
            var schedules = flights.Select(f => f.FlightSchedule).ToList();
            Dispatcher.Enqueue(() => _uiManager.RenderSchedule(schedules));
        }

        private void HandleYesterdayDiary(List<FlightDiary> diary)
        {
            Dispatcher.Enqueue(() => _uiManager.ShowDiary(diary, _diaryIntervalSeconds));
        }

        //private void RenderSchedule()
        //{
        //    List<Flight> scheduleList;
        //    lock (scheduleLock)
        //    {
        //        scheduleList = new(schedule);
        //    }

        //    int numSchedule = scheduleList.Count;
        //    for (int i = 0; i < numSchedule; i++)
        //    {
        //        string line = "";
        //        if (i < scheduleList.Count) line = $"{scheduleList[i].ScheduledTime.ToString("dd/MM/yyyy HH:mm")} {scheduleList[i].Code} {scheduleList[i].State}";
        //        WriteAtPosition(line, ROW_SCHEDULE_START + i, SECOND_COL_NUMS);
        //    }
        //}


        //private void UpdateSchedule(List<Flight> flights)
        //{
        //    lock (scheduleLock)
        //    {
        //        schedule = flights;
        //    }
        //}


        //private void RenderLog()
        //{
        //    List<string> log;
        //    lock (logLock)
        //    {
        //        log = new(logBuffer);
        //    }

        //    for (int i = 0; i < MAX_LOG_LINES; i++)
        //    {
        //        string line = " ";
        //        if (i < log.Count)
        //            line += log[log.Count - 1 - i];
        //        WriteAtPosition(line.PadRight(50), ROW_LOG_START + i, SECOND_COL_NUMS);

        //    }
        //}


        private void AddLog(string newLog)
        {
            lock (_logLock)
            {
                _logBuffer.Add(newLog);
                if (_logBuffer.Count > 50) _logBuffer.RemoveAt(0);
            }
            Dispatcher.Enqueue(() => _uiManager.AppendLog(newLog));
        }


        //private async void HandleYesterdayDiary()
        //{
        //    await ShowYesterdayDiaryAsync(1);
        //}


        //private async Task ShowYesterdayDiaryAsync(float intervalSeconds = 1)
        //{
        //    string diary = FlightDiaryIO.Load().Trim();
        //    FlightDiaryIO.ClearDiary();
        //    if (string.IsNullOrWhiteSpace(diary))
        //    {
        //        AddLog("[ATC] Tommorrow's diary count: 0");
        //        return;
        //    }
        //    string[] diaryList = diary.Split('\n');
        //    AddLog($"[ATC] Tommorrow's diary count: {diaryList.Length}");
        //    for (int i = 0; i < diaryList.Length; i++)
        //    {
        //        WriteAtPosition(diaryList[i], ROW_DIARY_START + i);
        //        await Task.Delay((int)(intervalSeconds * 1000));
        //    }
        //}
    }
}
