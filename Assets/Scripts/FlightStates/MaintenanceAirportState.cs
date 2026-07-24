using Assets.Scripts.Logger;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.FlightStates
{
    public class MaintenanceAirportState : IAirportState
    {
        public void HandleLanding(Flight commingFlight, ILogger logger)
        {
            logger.Log($"[ATC_M] {commingFlight.FlightSchedule.Code} - Landing confirmed");
        }

        public void HandleTakeoff(Flight takingoffFlight, ILogger logger)
        {
            logger.Log($"[ATC_M] {takingoffFlight.FlightSchedule.Code} - Takeoff confirmed");
        }
    }
}
