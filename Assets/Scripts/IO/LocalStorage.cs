using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Assets.Scripts.IO
{
    public class LocalStorage : IStorage
    {
        private readonly string _saveDirectory;

        public LocalStorage()
        {
            _saveDirectory = Application.persistentDataPath;
            Debug.Log($"Path: {_saveDirectory}");
        }

        public T Load<T>(string fileName, T defaultData = default)
        {
            string filePath = _saveDirectory + "/" + fileName; 
            if (!File.Exists(filePath))
                    return defaultData;

            string loadedData = File.ReadAllText(filePath);
            Debug.Log(loadedData);
            Debug.Log($"Load from: {filePath}");
            //return JsonUtility.FromJson<T>(loadedData);
            return JsonConvert.DeserializeObject<T>(loadedData);
        }

        public void Save<T>(string fileName, T data)
        {
            string filePath = _saveDirectory + "/" + fileName;
            //string jsonData = JsonUtility.ToJson(data);
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            Debug.Log(jsonData);
            File.WriteAllText(filePath, jsonData);
            Debug.Log($"Save to: {filePath}");
        }

        public void Append<T>(string fileName, T data)
        {
            string filePath = _saveDirectory + "/" + fileName;
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            Debug.Log(jsonData);
            File.AppendAllText(filePath, jsonData);
            Debug.Log($"Append to: {filePath}");
        }
    }
}
