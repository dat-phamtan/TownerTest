using Assets.Scripts.Data;
using Assets.Scripts.IO;
using Assets.Scripts.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Config
{
    public class SimulationConfig : IConfig
    {
        private Configuration _configData;
        private readonly IStorage _storage;

        public SimulationConfig(IStorage storage)
        {
            _storage = storage;
        }

        public void Load(ILogger logger)
        {
            //log
            _configData = _storage.Load<Configuration>("Config.json");
            if (_configData != null)
                logger.Log("[SYSTEM] Load config success");
            else
            {
                _configData = new Configuration(4, 0.01f, new Durations(20, 25), new MaintenancePeriod(2, 30, 5, 15));
                _storage.Save("Config.json", _configData);
                logger.Log("[SYSTEM] Load default");
            }
        }

        //just for test
        public void Save(Configuration configData)
        {
            _storage.Save<Configuration>("Config.json", configData);
        }

        public Configuration Get()
        {
            return _configData;
        }
    }
}
