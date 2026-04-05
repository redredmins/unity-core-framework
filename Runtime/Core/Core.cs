using UnityEngine;
using System;

namespace RedMinS
{
    public static class Core
    {
        public static AppManager app => AppManager.Instance;
        public static NetworkManager network => NetworkManager.Instance;
        public static DatabaseManager database => DatabaseManager.Instance;

#if FIREBASE_AUTH
        public static AuthManager auth => AuthManager.Instance;
#endif

        public static bool IsInitialized =>
            AppManager.HasInstance &&
            NetworkManager.HasInstance &&
            DatabaseManager.HasInstance;

        public static void Initialize()
        {
            _ = app;
            _ = network;
            _ = database;

            Debug.Log("[Core] All systems initialized.");
        }

#if FIREBASE_AUTH && FIREBASE_DATABASE
        /// <summary>
        /// Firebase 초기화 → 인증 → DB 연결까지 한 번에 수행합니다.
        /// </summary>
        public static void InitializeWithFirebase(string authProvider, Action onComplete = null, Action<string> onFail = null)
        {
            Initialize();

            auth.InitializeFirebase(
                onReady: () =>
                {
                    auth.SignIn(authProvider,
                        onSuccess: uid =>
                        {
                            var firebaseStore = new FirebaseDataStore(uid);
                            database.SetDataStore(firebaseStore);
                            network.StartConnectionMonitor();
                            Debug.Log($"[Core] Firebase initialized. User: {uid}");
                            onComplete?.Invoke();
                        },
                        onFail: onFail);
                },
                onFail: onFail);
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFields()
        {
            // 도메인 리로드 시 정적 필드 초기화
        }

        public static void LogSystemStatus()
        {
            Debug.Log($"[Core] System Status:");
            Debug.Log($"  - App: {(AppManager.HasInstance ? "✓" : "✗")}");
            Debug.Log($"  - Network: {(NetworkManager.HasInstance ? "✓" : "✗")}");
            Debug.Log($"  - Database: {(DatabaseManager.HasInstance ? "✓" : "✗")}");
#if FIREBASE_AUTH
            Debug.Log($"  - Auth: {(AuthManager.HasInstance ? "✓" : "✗")} {(auth.IsSignedIn ? $"(User: {auth.UserId})" : "")}");
#endif
        }
    }
}