using Assets.Scripts.Data;

#nullable enable
using Assets.Scripts.Unity;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Manager
{
    public interface IStorageManager
    {
        public void GenerateDailySchedule(MaintenancePeriod maintenancePeriod, List<Flight>? delayedDailySchedule);
        public List<FlightSchedule> LoadDailySchedule(DateTime today);
        public void SaveDailyLog(FlightDiary diary);
    }
}
