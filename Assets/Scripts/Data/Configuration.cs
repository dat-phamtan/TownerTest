using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Data
{
    public class Configuration
    {
        public int RunwayCount { get; set; } = 4;
        public float TimeScale { get; set; } = 1.5f;
        public Durations Durations { get; set; } = default;
        public MaintenancePeriod MaintenancePeriod { get; set; } = default;
        
        public Configuration() { }

        public Configuration(int runwayCount, float timeScale, Durations durations, MaintenancePeriod maintenancePeriod)
        {
            RunwayCount = runwayCount;
            TimeScale = timeScale;
            Durations = durations;
            MaintenancePeriod = maintenancePeriod;
        }
    }
}
