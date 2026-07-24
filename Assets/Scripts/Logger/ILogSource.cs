using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Logger
{
    public interface ILogSource
    {
        public event Action<string> OnLog;
    }
}
