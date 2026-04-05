#if FIREBASE_DATABASE
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

namespace RedMinS
{
    public class FirebaseDataStore : IDataStore
    {
        private readonly DatabaseReference _rootRef;
        private readonly string _userId;

        public FirebaseDataStore(string userId)
        {
            _userId = userId;
            _rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        }

        private DatabaseReference UserRef => _rootRef.Child("Users").Child(_userId);

        // === 동기 API (Firebase에서는 캐시 기반 간이 구현) ===

        public void Save<T>(string key, T data) where T : class
        {
            SaveAsync(key, data);
        }

        public T Load<T>(string key) where T : class
        {
            Debug.LogWarning("[FirebaseDataStore] 동기 Load는 지원되지 않습니다. LoadAsync를 사용하세요.");
            return null;
        }

        public bool HasKey(string key)
        {
            Debug.LogWarning("[FirebaseDataStore] 동기 HasKey는 지원되지 않습니다.");
            return false;
        }

        public void Delete(string key)
        {
            DeleteAsync(key);
        }

        public void DeleteAll()
        {
            UserRef.RemoveValueAsync();
        }

        // === 비동기 API ===

        public void SaveAsync<T>(string key, T data, Action onComplete = null, Action<string> onError = null) where T : class
        {
            string json = JsonUtility.ToJson(data);
            UserRef.Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[FirebaseDataStore] Save failed: {task.Exception?.Message}");
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }
                onComplete?.Invoke();
            });
        }

        public void LoadAsync<T>(string key, Action<T> onComplete, Action<string> onError = null) where T : class
        {
            UserRef.Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[FirebaseDataStore] Load failed: {task.Exception?.Message}");
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    onComplete?.Invoke(null);
                    return;
                }

                string json = snapshot.GetRawJsonValue();
                T data = JsonUtility.FromJson<T>(json);
                onComplete?.Invoke(data);
            });
        }

        public void DeleteAsync(string key, Action onComplete = null, Action<string> onError = null)
        {
            UserRef.Child(key).RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[FirebaseDataStore] Delete failed: {task.Exception?.Message}");
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }
                onComplete?.Invoke();
            });
        }

        // === 유틸리티 ===

        /// <summary>
        /// Dictionary 데이터를 특정 경로에 저장합니다.
        /// PuppyboyGame 패턴처럼 Dictionary<string, object> 기반 저장 시 사용.
        /// </summary>
        public void SetData(string path, Dictionary<string, object> data, Action onComplete = null, Action<string> onError = null)
        {
            UserRef.Child(path).SetValueAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// 특정 경로의 자식 필드만 부분 업데이트합니다.
        /// </summary>
        public void UpdateData(string path, Dictionary<string, object> updates, Action onComplete = null, Action<string> onError = null)
        {
            UserRef.Child(path).UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// 특정 경로의 단일 값을 업데이트합니다.
        /// </summary>
        public void SetValue(string path, object value, Action onComplete = null, Action<string> onError = null)
        {
            UserRef.Child(path).SetValueAsync(value).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// 특정 경로의 데이터를 Dictionary로 읽어옵니다.
        /// </summary>
        public void GetData(string path, Action<Dictionary<string, object>> onComplete, Action<string> onError = null)
        {
            UserRef.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    onComplete?.Invoke(null);
                    return;
                }

                var data = snapshot.Value as Dictionary<string, object>;
                onComplete?.Invoke(data);
            });
        }

        /// <summary>
        /// 유저 루트가 아닌 글로벌 경로에서 데이터를 읽습니다. (예: AppConfig)
        /// </summary>
        public void GetGlobalData(string path, Action<Dictionary<string, object>> onComplete, Action<string> onError = null)
        {
            _rootRef.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    onError?.Invoke(task.Exception?.Message);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    onComplete?.Invoke(null);
                    return;
                }

                var data = snapshot.Value as Dictionary<string, object>;
                onComplete?.Invoke(data);
            });
        }
    }
}
#endif
