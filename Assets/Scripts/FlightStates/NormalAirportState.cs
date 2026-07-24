using Assets.Scripts.Logger;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.FlightStates
{
    public class NormalAirportState : IAirportState
    {
        public void HandleLanding(Flight commingFlight, ILogger logger)
        {
            logger.Log($"[ATC] {commingFlight.FlightSchedule.Code} - Landing confirmed");
        }

        public void HandleTakeoff(Flight takeoffFlight, ILogger logger)
        {
            logger.Log($"[ATC] {takeoffFlight.FlightSchedule.Code} - Landing confirmed");
        }
    }
}
