using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.FlightPool
{
    public class FlightPool : IFlightPool
    {
        private readonly FlightView _prefab;
        private readonly Transform _poolParent;
        private readonly Queue<FlightView> _availableFlights;

        public FlightPool(FlightView prefab, Transform poolParent)
        {
            _prefab = prefab;
            _poolParent = poolParent;
            _availableFlights = new Queue<FlightView>();
        }

        public void PoolInit(int count, float scale)
        {
            for (int i = 0; i < count; i++)
            {
                FlightView newFlight = CreateNewFlight();
                newFlight.gameObject.SetActive(false);
                newFlight.transform.localScale = UnityEngine.Vector3.one * scale;
                _availableFlights.Enqueue(newFlight);
            }
        }

        public FlightView GetFlight(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, float speed)
        {
            FlightView flight;

            if (_availableFlights.Count == 0)
            {
                flight = CreateNewFlight();
            }
            else
            {
                flight = _availableFlights.Dequeue();
            }

            flight.transform.SetPositionAndRotation(position, rotation);
            flight.gameObject.SetActive(true);

            return flight;
        }

        public void ReleaseFlight(FlightView flight)
        {
            flight.gameObject.SetActive(false);
            flight.transform.SetParent(_poolParent); 

            _availableFlights.Enqueue(flight);
        }

        private FlightView CreateNewFlight()
        {
            return UnityEngine.Object.Instantiate(_prefab, _poolParent);
        }
    }
}
