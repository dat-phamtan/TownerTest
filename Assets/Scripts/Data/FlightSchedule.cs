using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Data
{
    public class FlightSchedule
    {
        public string Code { get; set; }
        public DateTime ScheduleTime { get; set; }

        public FlightSchedule(string code, DateTime scheduleTime)
        {
            Code = code;
            ScheduleTime = scheduleTime;
        }
    }
}
