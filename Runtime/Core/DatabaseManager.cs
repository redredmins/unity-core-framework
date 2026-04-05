using UnityEngine;
using System;

namespace RedMinS
{
    public class DatabaseManager : SingletonMonobehaviour<DatabaseManager>
    {
        [Header("Database Settings")]
        [SerializeField] private bool enableCloudSync = false;

        private IDataStore _dataStore;
        public IDataStore DataStore => _dataStore;

        public bool IsCloudSync => enableCloudSync;

        public event Action OnDataLoaded;
        public event Action OnDataSaved;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            _dataStore = new LocalDataStore();
            Debug.Log("[DatabaseManager] Initialized with LocalDataStore.");
        }

        /// <summary>
        /// 데이터 저장소를 교체합니다. (예: Firebase 전환 시 사용)
        /// </summary>
        public void SetDataStore(IDataStore dataStore)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            Debug.Log($"[DatabaseManager] DataStore changed to {dataStore.GetType().Name}.");
        }

        // === 동기 API ===

        public void SaveData<T>(string key, T data) where T : class
        {
            try
            {
                _dataStore.Save(key, data);
                Debug.Log($"[DatabaseManager] Data saved: {key}");
                OnDataSaved?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[DatabaseManager] Save failed: {e.Message}");
            }
        }

        public T LoadData<T>(string key) where T : class
        {
            try
            {
                T data = _dataStore.Load<T>(key);
                if (data != null)
                {
                    Debug.Log($"[DatabaseManager] Data loaded: {key}");
                    OnDataLoaded?.Invoke();
                }
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DatabaseManager] Load failed: {e.Message}");
                return null;
            }
        }

        public bool HasData(string key)
        {
            return _dataStore.HasKey(key);
        }

        public void DeleteData(string key)
        {
            _dataStore.Delete(key);
            Debug.Log($"[DatabaseManager] Data deleted: {key}");
        }

        public void ClearAllData()
        {
            _dataStore.DeleteAll();
            Debug.Log("[DatabaseManager] All data cleared.");
        }

        // === 비동기 API ===

        public void SaveDataAsync<T>(string key, T data, Action onComplete = null, Action<string> onError = null) where T : class
        {
            _dataStore.SaveAsync(key, data,
                () =>
                {
                    Debug.Log($"[DatabaseManager] Data saved (async): {key}");
                    OnDataSaved?.Invoke();
                    onComplete?.Invoke();
                },
                error =>
                {
                    Debug.LogError($"[DatabaseManager] Save failed (async): {error}");
                    onError?.Invoke(error);
                });
        }

        public void LoadDataAsync<T>(string key, Action<T> onComplete, Action<string> onError = null) where T : class
        {
            _dataStore.LoadAsync<T>(key,
                data =>
                {
                    if (data != null)
                    {
                        Debug.Log($"[DatabaseManager] Data loaded (async): {key}");
                        OnDataLoaded?.Invoke();
                    }
                    onComplete?.Invoke(data);
                },
                error =>
                {
                    Debug.LogError($"[DatabaseManager] Load failed (async): {error}");
                    onError?.Invoke(error);
                });
        }

        public void DeleteDataAsync(string key, Action onComplete = null, Action<string> onError = null)
        {
            _dataStore.DeleteAsync(key,
                () =>
                {
                    Debug.Log($"[DatabaseManager] Data deleted (async): {key}");
                    onComplete?.Invoke();
                },
                error =>
                {
                    Debug.LogError($"[DatabaseManager] Delete failed (async): {error}");
                    onError?.Invoke(error);
                });
        }
    }
}
