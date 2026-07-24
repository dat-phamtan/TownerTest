#nullable enable
using Assets;
using Assets.Scripts.Entity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Manager
{
    public class RunwayManager : IRunwayManager
    {
        private Runway[]? runways;
        public event Action? OnBecomeAvailable;

        public RunwayManager(){}

        public void Init(int count)
        {
            runways = new Runway[count];

            for (int i = 0; i < count; i++)
            {
                runways[i] = new Runway(i, OnRunwayChanged);
            }
        }

        public Runway? GetAvailableRunway()
        {
            if (runways == null) return default;
            for (int i = 0; i < runways.Length; i++)
            {
                if (!runways[i].IsOccupied) return runways[i];
            }
            return null;
        }

        public bool AllRunwayEmpty()
        {
            if (runways == null) return false; 
            for (int i = 0; i < runways.Length; i++)
                if (runways[i].IsOccupied) return false;
            return true;
        }


        public Runway[]? GetRunways()
        {
            if (runways == null) return default;
            return runways;
        }


        private void OnRunwayChanged(bool isOccupied)
        {
            if (!isOccupied)
                OnBecomeAvailable?.Invoke();
        }
    }
}
