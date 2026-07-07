using System;
using UnityEngine;

// Google Credential Manager 로그인 브릿지 (Firebase 무관, 항상 컴파일).
// Android Java 브릿지가 UnitySendMessage로 "email|idToken" 또는 "NULL"을 콜백한다.
namespace RedMinS
{
    public class GoogleCredentialService : MonoBehaviour
    {
        const string CallbackObject = "RedGoogleSignInCallback";
        const string CallbackMethod = "OnGoogleSignInResult";

        Action<string, string> _onSuccess; // (email, idToken)
        Action _onFail;

        public static void SignIn(string webClientId, Action<string, string> onSuccess, Action onFail)
        {
#if UNITY_EDITOR
            // 에디터 스텁: idToken은 의도적으로 빈 문자열이다. 실제 Google idToken은 기기에서만
            // 발급되므로 에디터에서 Firebase Google 로그인(GoogleAuthProvider)은 테스트할 수 없고,
            // idToken이 필요 없는 Legacy 경로(LegacyGoogleIdProvider)만 이메일로 동작한다.
            if (onSuccess != null) onSuccess("tester1@dev.local", "");
#elif UNITY_ANDROID
            var go = new GameObject(CallbackObject);
            var receiver = go.AddComponent<GoogleCredentialService>();
            receiver._onSuccess = onSuccess;
            receiver._onFail = onFail;

            using (var bridge = new AndroidJavaClass("com.redmins.core.GoogleSignInBridge"))
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                bridge.CallStatic("signIn", activity, webClientId, CallbackObject, CallbackMethod);
            }
#else
            if (onFail != null) onFail();
#endif
        }

        // Java 브릿지가 UnitySendMessage로 호출. "email|idToken" 또는 "NULL"
        public void OnGoogleSignInResult(string result)
        {
            if (string.IsNullOrEmpty(result) || result == "NULL")
            {
                if (_onFail != null) _onFail();
            }
            else
            {
                int sep = result.IndexOf('|');
                string email = (sep >= 0) ? result.Substring(0, sep) : result;
                string idToken = (sep >= 0) ? result.Substring(sep + 1) : "";
                if (_onSuccess != null) _onSuccess(email, idToken);
            }

            Destroy(gameObject);
        }
    }
}
