using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Data
{
    public class MaintenancePeriod
    {
        public int MaintenanceStartHour { get; set; } = 2;
        public int MaintenanceStartMinute { get; set; } = 30;
        public int MaintenanceEndHour { get; set; } = 5;
        public int MaintenanceEndMinute { get; set; } = 15;

        public MaintenancePeriod(int maintenanceStartHour, int maintenanceStartMinute, int maintenanceEndHour, int maintenanceEndMinute)
        {
            MaintenanceStartHour = maintenanceStartHour;
            MaintenanceStartMinute = maintenanceStartMinute;
            MaintenanceEndHour = maintenanceEndHour;
            MaintenanceEndMinute = maintenanceEndMinute;
        }
    }
}
