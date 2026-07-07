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
