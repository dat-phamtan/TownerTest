using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Data
{
    public class FlightDiary
    {
        public string Code { get; set; }
        public int RunwayIndex { get; set; }
        public char IsLanding { get; set; }
        public DateTime DiaryTime { get; set; }

        public FlightDiary(string code, int runwayIndex, char isLanding, DateTime diaryTime)
        {
            Code = code;
            RunwayIndex = runwayIndex;
            IsLanding = isLanding;
            DiaryTime = diaryTime;
        }
    }
}
