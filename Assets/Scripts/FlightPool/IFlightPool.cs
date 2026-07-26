using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Assets.Scripts.FlightPool
{
    public interface IFlightPool
    {
        public void PoolInit(int count, float scale);
        public FlightView GetFlight(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, float speed);
        public void ReleaseFlight(FlightView flight);
    }
}
