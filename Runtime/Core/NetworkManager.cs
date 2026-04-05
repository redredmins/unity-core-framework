using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RedMinS
{
    public enum NetworkPolicy
    {
        AlwaysRequired,   // 네트워크 끊기면 앱 종료 (재화 관리 게임용)
        RequiredForSync,  // 끊기면 쓰기 차단, 읽기는 캐시 허용
        Optional          // 오프라인 허용 (싱글플레이 게임용)
    }

    public class NetworkManager : SingletonMonobehaviour<NetworkManager>
    {
        [Header("Network Settings")]
        [SerializeField] private float timeoutDuration = 10f;
        [SerializeField] private int maxRetryCount = 3;

        [Header("Network Policy")]
        [SerializeField] private NetworkPolicy networkPolicy = NetworkPolicy.AlwaysRequired;
        [SerializeField] private float connectionCheckInterval = 5f;

        private bool _isConnected = true;
        private Coroutine _connectionCheckCoroutine;

        public NetworkPolicy Policy => networkPolicy;
        public bool IsConnected => _isConnected;

        public event Action OnNetworkConnected;
        public event Action OnNetworkDisconnected;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
        }

        /// <summary>
        /// 네트워크 연결 감시를 시작합니다. 앱 초기화 후 호출하세요.
        /// </summary>
        public void StartConnectionMonitor()
        {
            if (_connectionCheckCoroutine != null)
                StopCoroutine(_connectionCheckCoroutine);

            _connectionCheckCoroutine = StartCoroutine(ConnectionMonitorCoroutine());
        }

        public void StopConnectionMonitor()
        {
            if (_connectionCheckCoroutine != null)
            {
                StopCoroutine(_connectionCheckCoroutine);
                _connectionCheckCoroutine = null;
            }
        }

        private IEnumerator ConnectionMonitorCoroutine()
        {
            var wait = new WaitForSeconds(connectionCheckInterval);

            while (true)
            {
                bool wasConnected = _isConnected;
                _isConnected = Application.internetReachability != NetworkReachability.NotReachable;

                if (wasConnected && !_isConnected)
                {
                    OnNetworkDisconnected?.Invoke();
                    HandleDisconnection();
                }
                else if (!wasConnected && _isConnected)
                {
                    OnNetworkConnected?.Invoke();
                }

                yield return wait;
            }
        }

        private void HandleDisconnection()
        {
            switch (networkPolicy)
            {
                case NetworkPolicy.AlwaysRequired:
                    Debug.LogError("[NetworkManager] Network lost. Policy: AlwaysRequired - Quitting application.");
                    Core.app.ui.ShowSystemPopup(true,
                        "Network Error",
                        "Network connection lost. The app will close.",
                        () => Application.Quit()
                    );
                    break;

                case NetworkPolicy.RequiredForSync:
                    Debug.LogWarning("[NetworkManager] Network lost. Policy: RequiredForSync - Cloud sync disabled.");
                    break;

                case NetworkPolicy.Optional:
                    Debug.Log("[NetworkManager] Network lost. Policy: Optional - Continuing offline.");
                    break;
            }
        }

        public void CheckNetworkConnection()
        {
            _isConnected = Application.internetReachability != NetworkReachability.NotReachable;

            if (_isConnected)
                OnNetworkConnected?.Invoke();
            else
                OnNetworkDisconnected?.Invoke();
        }

        public void SendRequest<T>(string url, Action<T> onSuccess, Action<string> onError = null) where T : class
        {
            StartCoroutine(SendRequestCoroutine(url, onSuccess, onError));
        }

        private IEnumerator SendRequestCoroutine<T>(string url, Action<T> onSuccess, Action<string> onError) where T : class
        {
            string lastError = null;

            for (int attempt = 0; attempt < maxRetryCount; attempt++)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    request.timeout = (int)timeoutDuration;

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        try
                        {
                            T result = JsonUtility.FromJson<T>(request.downloadHandler.text);
                            onSuccess?.Invoke(result);
                        }
                        catch (Exception e)
                        {
                            onError?.Invoke($"JSON Parse Error: {e.Message}");
                        }
                        yield break;
                    }

                    lastError = request.error;
                    Debug.LogWarning($"[NetworkManager] Request failed (attempt {attempt + 1}/{maxRetryCount}): {lastError}");
                }

                if (attempt < maxRetryCount - 1)
                    yield return new WaitForSeconds(1f);
            }

            onError?.Invoke($"Network Error: {lastError}");
        }
    }
}