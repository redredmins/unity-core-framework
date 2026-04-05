#if FIREBASE_AUTH
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Firebase.Auth;
using Firebase.Extensions;

namespace RedMinS
{
    /// <summary>
    /// 카카오, 네이버 등 Firebase가 네이티브 지원하지 않는 인증 제공자용.
    /// Custom Token 방식으로 동작합니다.
    ///
    /// 흐름:
    /// 1. 각 플랫폼 SDK로 로그인하여 액세스 토큰 획득 (클라이언트)
    /// 2. 액세스 토큰을 Cloud Function에 전송 (클라이언트 → 서버)
    /// 3. Cloud Function이 토큰을 검증하고 Firebase Custom Token 발급 (서버)
    /// 4. Custom Token으로 Firebase 로그인 (클라이언트)
    ///
    /// Cloud Function 샘플은 Examples/Firebase/CloudFunction_CustomAuth.js 참고.
    /// </summary>
    public class CustomTokenAuthProvider : IAuthProvider
    {
        private readonly string _providerName;
        private readonly string _cloudFunctionUrl;

        public string ProviderName => _providerName;
        public bool IsSignedIn => FirebaseAuth.DefaultInstance.CurrentUser != null;

        /// <param name="providerName">인증 제공자 이름 (예: "Kakao", "Naver")</param>
        /// <param name="cloudFunctionUrl">Custom Token 발급 Cloud Function URL</param>
        public CustomTokenAuthProvider(string providerName, string cloudFunctionUrl)
        {
            _providerName = providerName;
            _cloudFunctionUrl = cloudFunctionUrl;
        }

        public void SignIn(Action<string> onSuccess, Action<string> onFail)
        {
            // 각 플랫폼 SDK에서 액세스 토큰을 먼저 획득한 후
            // SignInWithAccessToken()을 호출하세요.
            onFail?.Invoke($"{_providerName} SDK에서 accessToken을 먼저 획득하세요. SignInWithAccessToken()을 사용하세요.");
        }

        /// <summary>
        /// 플랫폼 액세스 토큰으로 Firebase 인증을 수행합니다.
        /// MonoBehaviour의 StartCoroutine으로 실행하세요.
        /// </summary>
        public IEnumerator SignInWithAccessToken(string accessToken, Action<string> onSuccess, Action<string> onFail)
        {
            // 1. Cloud Function에 액세스 토큰 전송하여 Custom Token 획득
            string jsonBody = JsonUtility.ToJson(new TokenRequest { token = accessToken, provider = _providerName });
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

            using (var request = new UnityWebRequest(_cloudFunctionUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = 10;

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    onFail?.Invoke($"[{_providerName}] Cloud Function request failed: {request.error}");
                    yield break;
                }

                var response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);

                if (string.IsNullOrEmpty(response.customToken))
                {
                    onFail?.Invoke($"[{_providerName}] Empty custom token received.");
                    yield break;
                }

                // 2. Custom Token으로 Firebase 로그인
                SignInWithCustomToken(response.customToken, onSuccess, onFail);
            }
        }

        private void SignInWithCustomToken(string customToken, Action<string> onSuccess, Action<string> onFail)
        {
            FirebaseAuth.DefaultInstance.SignInWithCustomTokenAsync(customToken).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    onFail?.Invoke($"[{_providerName}] Custom token sign-in was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    onFail?.Invoke($"[{_providerName}] Custom token sign-in failed: {task.Exception?.Message}");
                    return;
                }

                string uid = task.Result.User.UserId;
                Debug.Log($"[CustomTokenAuthProvider:{_providerName}] Signed in: {uid}");
                onSuccess?.Invoke(uid);
            });
        }

        public void SignOut()
        {
            FirebaseAuth.DefaultInstance.SignOut();
            Debug.Log($"[CustomTokenAuthProvider:{_providerName}] Signed out.");
        }

        [Serializable]
        private class TokenRequest
        {
            public string token;
            public string provider;
        }

        [Serializable]
        private class TokenResponse
        {
            public string customToken;
        }
    }
}
#endif
