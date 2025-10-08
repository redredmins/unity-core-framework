using UnityEngine;
using System;

namespace RedMinS
{
    public static class Core
    {
        public static AppManager app => AppManager.Instance;
        public static NetworkManager network => NetworkManager.Instance;
        public static DatabaseManager database => DatabaseManager.Instance;

        
        public static bool IsInitialized => 
            AppManager.HasInstance && 
            NetworkManager.HasInstance && 
            DatabaseManager.HasInstance;

        
        public static void Initialize()
        {
            // 매니저들의 인스턴스 생성을 강제로 트리거
            _ = app;
            _ = network;
            _ = database;
            
            Debug.Log("[Core] All systems initialized.");
        }


        /// 애플리케이션 종료 시 정리 작업
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFields()
        {
            // 도메인 리로드 시 정적 필드 초기화
        }


        /// 디버그용 시스템 상태 출력
        public static void LogSystemStatus()
        {
            Debug.Log($"[Core] System Status:");
            Debug.Log($"  - App: {(AppManager.HasInstance ? "✓" : "✗")}");
            Debug.Log($"  - Network: {(NetworkManager.HasInstance ? "✓" : "✗")}");
            Debug.Log($"  - Database: {(DatabaseManager.HasInstance ? "✓" : "✗")}");
        }
    }
}