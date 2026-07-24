using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Logger
{
    public class EventLogger : ILogger, ILogSource
    {
        public event Action<string> OnLog;

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }
    }
}
