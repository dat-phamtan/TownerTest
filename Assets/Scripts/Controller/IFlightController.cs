#nullable enable
using Assets.Scripts.Data;
using Assets.Scripts.Entity;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Controller
{
    public interface IFlightController
    {
        public event Action<List<Flight>> OnScheduleUpdated;
        public event Action OnLogDiary;
        public event Action<bool> OnStatusChanged;
        public event Action? OnRunwayInit;
        public event Action<int>? OnLandingQueueChanged;
        public event Action<int>? OnTakeoffQueueChanged;

        public void Init();
        public Runway[]? GetRunways();
        public bool IsMaintenanceMode();
        public ScreenData GetScreenData();
    }
}
