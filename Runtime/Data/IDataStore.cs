using System;

namespace RedMinS
{
    public interface IDataStore
    {
        // 동기 API (로컬 저장소용)
        void Save<T>(string key, T data) where T : class;
        T Load<T>(string key) where T : class;
        bool HasKey(string key);
        void Delete(string key);
        void DeleteAll();

        // 비동기 API (Firebase 등 원격 저장소용)
        void SaveAsync<T>(string key, T data, Action onComplete = null, Action<string> onError = null) where T : class;
        void LoadAsync<T>(string key, Action<T> onComplete, Action<string> onError = null) where T : class;
        void DeleteAsync(string key, Action onComplete = null, Action<string> onError = null);
    }
}
