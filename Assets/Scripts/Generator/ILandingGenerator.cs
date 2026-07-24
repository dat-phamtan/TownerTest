#nullable enable
using Assets.Scripts.Logger;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Generator
{
    public interface ILandingGenerator
    {
        public Flight? CheckGenerate(DateTime simulatedTime, ILogger logger);
        public void Reset();
    }
}
