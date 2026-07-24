using Assets.Scripts.Logger;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.FlightStates
{
    public interface IAirportState
    {
        void HandleLanding(Flight commingFlight, ILogger logger);
        void HandleTakeoff(Flight takeoffFlight, ILogger logger);
    }
}
