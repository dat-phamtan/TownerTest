using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Data
{
    public class Durations
    {
        public float TakeoffDuration { get; set; } = 20;
        public float LandingDuration { get; set; } = 25;

        public Durations(float takeoffDuration, float landingDuration)
        {
            TakeoffDuration = takeoffDuration;
            LandingDuration = landingDuration;
        }
    }
}
