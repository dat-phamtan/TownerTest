using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ControlTowner.Utility
{
    internal class SimpleClock
    {
        public static SimpleClock Instance { get; } = new SimpleClock();
        private SimpleClock() { }

        public DateTime SimulatedTime { get; private set; }
        private float _timeScale;
        private MaintenancePeriod _maintenancePeriod;
        private readonly object lockObject = new();

        public event Action<DateTime> OnTick;
        public event Action OnMaintenanceStart;
        public event Action OnNewDayStart;


        public void InitClock(int startHour, int startMinute, float timeScale, MaintenancePeriod maintenancePeriod)
        {
            SimulatedTime = DateTime.Today.AddHours(startHour).AddMinutes(startMinute);
            _maintenancePeriod = maintenancePeriod;
            _timeScale = timeScale;
        }


        public void UpdateClock(float deltaTime)
        {
            DateTime oldTime, newTime;
            lock (lockObject)
            {
                oldTime = SimulatedTime;
                SimulatedTime = SimulatedTime.AddSeconds(deltaTime * _timeScale);
                newTime = SimulatedTime;
            }
            CheckSpecialEvent(oldTime, newTime);
            OnTick?.Invoke(newTime);
        }


        private void CheckSpecialEvent(DateTime oldTime, DateTime newTime)
        {
            TimeSpan maintenanceStart = new(_maintenancePeriod.MaintenanceStartHour, _maintenancePeriod.MaintenanceStartMinute, 0);
            TimeSpan maintenanceEnd = new(_maintenancePeriod.MaintenanceEndHour, _maintenancePeriod.MaintenanceEndMinute, 0);

            if (HasCrossedThreshold(oldTime, newTime, maintenanceStart))
                OnMaintenanceStart.Invoke();
            if (HasCrossedThreshold(oldTime, newTime, maintenanceEnd))
                OnNewDayStart.Invoke();
        }


        private bool HasCrossedThreshold(DateTime oldTime, DateTime newTime, TimeSpan threshold)
        {
            TimeSpan oldTimeOfDay = oldTime.TimeOfDay;
            TimeSpan newTimeOfDay = newTime.TimeOfDay;

            if (oldTimeOfDay <= newTimeOfDay) // not cross the midnight
                return oldTimeOfDay < threshold && newTimeOfDay >= threshold;
            else
                return oldTimeOfDay < threshold || newTimeOfDay >= threshold;
        }
    }
}
