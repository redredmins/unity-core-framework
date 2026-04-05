#if FIREBASE_AUTH
using System;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

namespace RedMinS
{
    /// <summary>
    /// Google 로그인 제공자.
    /// Google Sign-In SDK를 통해 ID Token을 얻은 후 Firebase에 연결합니다.
    ///
    /// 사용법:
    /// 1. Google Sign-In Unity SDK 설치
    /// 2. google-services.json / GoogleService-Info.plist 설정
    /// 3. var provider = new GoogleAuthProvider("your-web-client-id");
    /// </summary>
    public class GoogleAuthProvider : IAuthProvider
    {
        private readonly string _webClientId;

        public string ProviderName => "Google";
        public bool IsSignedIn => FirebaseAuth.DefaultInstance.CurrentUser != null;

        public GoogleAuthProvider(string webClientId)
        {
            _webClientId = webClientId;
        }

        public void SignIn(Action<string> onSuccess, Action<string> onFail)
        {
            // Google Sign-In SDK로 ID Token을 먼저 획득한 후 이 메서드를 호출하세요.
            // 아래는 ID Token이 있다고 가정한 Firebase 인증 흐름입니다.
            //
            // 실제 구현 시:
            // 1. Google Sign-In SDK의 GoogleSignIn.DefaultInstance.SignIn() 호출
            // 2. 성공 콜백에서 idToken을 받아 SignInWithGoogleToken() 호출

            onFail?.Invoke("Google Sign-In SDK를 통해 idToken을 먼저 획득하세요. SignInWithGoogleToken()을 사용하세요.");
        }

        /// <summary>
        /// Google ID Token으로 Firebase 인증을 수행합니다.
        /// Google Sign-In SDK에서 토큰을 받은 후 이 메서드를 호출하세요.
        /// </summary>
        public void SignInWithGoogleToken(string idToken, Action<string> onSuccess, Action<string> onFail)
        {
            Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(idToken, null);

            FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    onFail?.Invoke("Google sign-in was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    onFail?.Invoke($"Google sign-in failed: {task.Exception?.Message}");
                    return;
                }

                string uid = task.Result.User.UserId;
                Debug.Log($"[GoogleAuthProvider] Signed in: {uid}");
                onSuccess?.Invoke(uid);
            });
        }

        public void SignOut()
        {
            FirebaseAuth.DefaultInstance.SignOut();
            Debug.Log("[GoogleAuthProvider] Signed out.");
        }
    }
}
#endif
