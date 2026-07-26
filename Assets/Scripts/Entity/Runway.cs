#nullable enable
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Assets.Scripts.Entity
{
    public class Runway
    {
        public int Id { get; set; }
        public bool IsOccupied { get; set; } = false;
        public Flight? CurrentFlight { get; set; }
        public float RealDuration { get; set; }
        public Vector3 Position { get; set; }
        public float RunwayLong {  get; set; }

        private readonly object _lock = new();
        public Action<bool>? OnStateChanged;

        public Runway(int id, Action<bool>? onStateChanged = null)
        {
            Id = id;
            OnStateChanged = onStateChanged;
        }

        public void SetRunwayLong(float runwayLong)
        {
            RunwayLong = runwayLong;
        }

        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        public bool AssignFlight(Flight flight)
        {
            if (flight == null) return false;
            lock (_lock)
            {
                if (IsOccupied) return false;
                CurrentFlight = flight;
                IsOccupied = true;
                OnStateChanged?.Invoke(IsOccupied);
                return true;
            }
        }

        public void Free()
        {
            lock (_lock)
            {
                CurrentFlight = null;
                IsOccupied = false;
            }
            OnStateChanged?.Invoke(IsOccupied);
        }
    }
}
