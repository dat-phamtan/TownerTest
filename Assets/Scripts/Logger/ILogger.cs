#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Logger
{
    public interface ILogger
    {
        void Log(string message);
    }
}
