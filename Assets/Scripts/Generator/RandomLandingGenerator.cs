#nullable enable
using Assets.Scripts.Data;
using Assets.Scripts.Logger;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace Assets.Scripts.Generator
{
    public class RandomLandingGenerator : ILandingGenerator
    {
        //can be changed
        private const int minGapMinute = 10;
        private const int maxGapMinute = 25;

        private bool isWaiting = false;
        private DateTime generateTime;
        private string[] header = { "MH", "VN", "SK", "FA", "OL" };


        public Flight? CheckGenerate(DateTime simulatedTime, ILogger logger)
        {
            var random = new Random();
            if (!isWaiting)
            {
                int randomPeriod = random.Next(minGapMinute * 60, maxGapMinute * 60);
                generateTime = simulatedTime.AddSeconds(randomPeriod);
                isWaiting = true;
                return null;
            }

            if (generateTime <= simulatedTime)
            {
                isWaiting = false;
                var headerIndex = random.Next(header.Length);
                var code = header[headerIndex] + random.Next(100, 999).ToString();
                var schedule = new FlightSchedule(code, simulatedTime);
                return new Flight(schedule, FlightType.Landing, FlightState.Waiting, logger);
            }
            return null;
        }


        public void Reset()
        {
            isWaiting = false;
        }
    }
}
