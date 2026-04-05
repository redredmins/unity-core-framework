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

        // 비동기 API — 로컬이므로 동기 실행 후 즉시 콜백
        public void SaveAsync<T>(string key, T data, Action onComplete = null, Action<string> onError = null) where T : class
        {
            try
            {
                Save(key, data);
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }

        public void LoadAsync<T>(string key, Action<T> onComplete, Action<string> onError = null) where T : class
        {
            try
            {
                T data = Load<T>(key);
                onComplete?.Invoke(data);
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }

        public void DeleteAsync(string key, Action onComplete = null, Action<string> onError = null)
        {
            try
            {
                Delete(key);
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }
    }
}
