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
        //private int _screenHeight;
        //
        private readonly List<string> _logBuffer = new();

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
            //_controller.OnScheduleUpdated += HandleSchedule;
            //_controller.OnLogDiary += HandleYesterdayDiary;
            _controller.OnStatusChanged += HandleStatusChanged;
            _controller.OnRunwayInit += HandleRunwayInit;
            _controller.OnLandingQueueChanged += HandleLandingQueueChanged;
            _controller.OnTakeoffQueueChanged += HandleTakeoffQueueChanged;


            _controller.OnPreLanding += HandlePreLanding;
            _controller.OnFlightLanding += HandleFlightLanding;
            
        }

        //temp
        private void HandlePreLanding(Runway runway)
        {
            var position = new Vector3(runway.Position.x, _screenData.ScreenHeight / 2, 0);
            //var flightView = _flightPool.GetFlight(position, , )


        }

        //private IEnumerator HoldAndLandRoutine()

        private void HandleFlightLanding(Runway runway)
        {
            //float xPos = _runwayXPos[runway.Id];
            //if (runway.CurrentFlight.Type == FlightType.Landing)
            //{
            //    var position = Vector3.(xPos, -_screenHeight / 2, 0);

            //}
        }

        private void HandleTakeoffQueueChanged(int queueCount)
        {
            _uiManager.ChangeTakeoffQueue("x" + queueCount);
        }

        private void HandleLandingQueueChanged(int queueCount)
        {
            _uiManager.ChangeLandingQueue("x" + queueCount);
        }

        private void HandleStatusChanged(bool status)
        {
            if (status)
                _uiManager.ChangeStatus("Working");
            else
                _uiManager.ChangeStatus("Maintenance");
        }

        private void HandleRunwayInit()
        {
            _uiManager.ActiveRunways();
        }

        //public void Start()
        //{
        //    Task.Run(RenderLoop);
        //}

        //private async Task RenderLoop()
        //{
        //    while (true)
        //    {
        //        RenderClock();
        //        RenderStatus();
        //        RenderRunways();
        //        RenderSchedule();
        //        RenderLog();
        //        //Test();
        //        await Task.Delay(200);
        //    }
        //}



        //private void RenderClock()
        //{
        //    var time = SimpleClock.Instance.SimulatedTime;

        //}


        //private void RenderStatus()
        //{
        //    string status = (controller.IsMaintenanceMode()) ? "Status: Maintenance" : "Status: Working";
        //    WriteAtPosition(status, ROW_STATUS);
        //}


        //private void RenderRunways()
        //{
        //    var runways = controller.GetRunways();
        //    for (int i = 0; i < runways.Length; i++)
        //    {
        //        string info;
        //        var runway = runways[i];
        //        if (runway.IsOccupied)
        //            info = $"Runway {runway.id}: [USED] {runway.CurrentFlight?.Code} {runway.CurrentFlight?.Type}";
        //        else
        //            info = $"Runway {runway.id}: [EMPTY]";

        //        WriteAtPosition(info, ROW_RUNWAY_HEADER + 1 + i);
        //    }
        //}


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

        //private static void WriteAtPosition(string text, int row, int col = 0)
        //{
        //    lock (consoleLock)
        //    {
        //        Console.SetCursorPosition(col, row);
        //        int availableSpace = Console.WindowWidth - col - 1;
        //        if (availableSpace <= 0) return;

        //        if (text.Length > availableSpace)
        //        {
        //            text = text.Substring(0, availableSpace - 3);
        //            text += "...";
        //        }
        //        Console.Write(text.PadRight(availableSpace));
        //    }
        //}
    }
}
