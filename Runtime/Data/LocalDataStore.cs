using UnityEngine;
using System;

namespace RedMinS
{
    public class LocalDataStore : IDataStore
    {
        public void Save<T>(string key, T data) where T : class
        {
            string jsonData = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();
        }

        public T Load<T>(string key) where T : class
        {
            string jsonData = PlayerPrefs.GetString(key, string.Empty);
            if (string.IsNullOrEmpty(jsonData))
                return null;

            return JsonUtility.FromJson<T>(jsonData);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
