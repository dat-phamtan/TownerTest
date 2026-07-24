using Assets.Scripts.Data;
using Assets.Scripts.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts
{
    public interface IConfig
    {
        public void Load(ILogger logger);
        public void Save(Configuration configData);
        public Configuration Get();
    }

}
