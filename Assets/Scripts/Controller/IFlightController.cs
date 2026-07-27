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
        public event Action<List<FlightDiary>> OnLogDiary;
        public event Action<bool> OnStatusChanged;
        public event Action? OnRunwayInit;
        public event Action<int>? OnLandingQueueChanged;
        public event Action<int>? OnTakeoffQueueChanged;

        public event Action<Runway>? OnPreLanding;
        public event Action<Runway>? OnPreTakeoff;

        public event Action<Runway>? OnFlightLanding;
        public event Action<Runway>? OnFlightTakeoff;

        public event Action<Runway>? OnPostLanding;
        public event Action<Runway>? OnPostTakeoff;

        public void LoadData();
        public void StartSimulation();
        public Runway[]? GetRunways();
        public bool IsMaintenanceMode();
        //public ScreenData GetScreenData();
        //public int GetRunwayCount();
    }
}
