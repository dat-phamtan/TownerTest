#nullable enable
using Assets.Scripts.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Manager
{
    public interface IRunwayManager
    {
        public event Action? OnBecomeAvailable;
        public void Init(int count);
        public Runway? GetAvailableRunway();
        public bool AllRunwayEmpty();
        public Runway[]? GetRunways();
    }
}
