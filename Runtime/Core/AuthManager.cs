#if FIREBASE_AUTH
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

namespace RedMinS
{
    public class AuthManager : SingletonMonobehaviour<AuthManager>
    {
        private FirebaseAuth _auth;
        private FirebaseUser _user;
        private Dictionary<string, IAuthProvider> _providers = new Dictionary<string, IAuthProvider>();

        private bool _isFirebaseReady = false;

        /// <summary>현재 로그인된 유저의 UID. 미로그인 시 빈 문자열.</summary>
        public string UserId => _user?.UserId ?? string.Empty;

        /// <summary>현재 로그인된 FirebaseUser.</summary>
        public FirebaseUser CurrentUser => _user;

        /// <summary>Firebase 초기화 완료 여부.</summary>
        public bool IsFirebaseReady => _isFirebaseReady;

        /// <summary>로그인 상태 여부.</summary>
        public bool IsSignedIn => _user != null;

        public event Action<string> OnSignedIn;   // uid 전달
        public event Action OnSignedOut;
        public event Action OnFirebaseReady;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
        }

        /// <summary>
        /// Firebase를 초기화합니다. 앱 시작 시 가장 먼저 호출하세요.
        /// </summary>
        public void InitializeFirebase(Action onReady = null, Action<string> onFail = null)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result != DependencyStatus.Available)
                {
                    string error = $"[AuthManager] Firebase dependency error: {task.Result}";
                    Debug.LogError(error);
                    onFail?.Invoke(error);
                    return;
                }

                _auth = FirebaseAuth.DefaultInstance;
                _auth.StateChanged += OnAuthStateChanged;

                // 이전 세션의 로그인이 유지되어 있는 경우
                _user = _auth.CurrentUser;

                _isFirebaseReady = true;
                Debug.Log("[AuthManager] Firebase initialized.");
                OnFirebaseReady?.Invoke();
                onReady?.Invoke();
            });
        }

        /// <summary>
        /// 인증 제공자를 등록합니다.
        /// </summary>
        public void RegisterProvider(IAuthProvider provider)
        {
            _providers[provider.ProviderName] = provider;
            Debug.Log($"[AuthManager] Provider registered: {provider.ProviderName}");
        }

        /// <summary>
        /// 등록된 인증 제공자로 로그인합니다.
        /// </summary>
        public void SignIn(string providerName, Action<string> onSuccess = null, Action<string> onFail = null)
        {
            if (!_isFirebaseReady)
            {
                onFail?.Invoke("[AuthManager] Firebase is not initialized yet.");
                return;
            }

            if (!_providers.TryGetValue(providerName, out IAuthProvider provider))
            {
                onFail?.Invoke($"[AuthManager] Provider not found: {providerName}");
                return;
            }

            provider.SignIn(
                uid =>
                {
                    _user = _auth.CurrentUser;
                    Debug.Log($"[AuthManager] Signed in via {providerName}: {uid}");
                    OnSignedIn?.Invoke(uid);
                    onSuccess?.Invoke(uid);
                },
                error =>
                {
                    Debug.LogError($"[AuthManager] Sign-in failed ({providerName}): {error}");
                    onFail?.Invoke(error);
                });
        }

        /// <summary>
        /// 로그아웃합니다.
        /// </summary>
        public void SignOut()
        {
            _auth?.SignOut();
            _user = null;
            Debug.Log("[AuthManager] Signed out.");
            OnSignedOut?.Invoke();
        }

        /// <summary>
        /// 등록된 인증 제공자를 반환합니다.
        /// </summary>
        public T GetProvider<T>(string providerName) where T : class, IAuthProvider
        {
            if (_providers.TryGetValue(providerName, out IAuthProvider provider))
                return provider as T;
            return null;
        }

        private void OnAuthStateChanged(object sender, EventArgs e)
        {
            FirebaseUser newUser = _auth.CurrentUser;

            if (newUser != _user)
            {
                _user = newUser;

                if (_user != null)
                    OnSignedIn?.Invoke(_user.UserId);
                else
                    OnSignedOut?.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (_auth != null)
                _auth.StateChanged -= OnAuthStateChanged;
        }
    }
}
#endif
