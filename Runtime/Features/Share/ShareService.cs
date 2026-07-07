using UnityEngine;

// SNS 이미지 공유 / 문의 메일. 네이티브 브릿지 없이 유니티 단독 구현.
// 게임 고유 요소(공유 문구/지원 메일 주소)는 제거하고 파라미터로 역전.
namespace RedMinS
{
    public static class ShareService
    {
        public static void ShareImage(string filePath, string message)
        {
#if UNITY_EDITOR
            Debug.Log("[ShareService] ShareImage (에디터 스텁): " + filePath + " / " + message);
#elif UNITY_ANDROID
            using (var bridge = new AndroidJavaClass("com.redmins.core.ShareBridge"))
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                bridge.CallStatic("shareImage", activity, filePath, message);
            }
#else
            Debug.Log("[ShareService] ShareImage not supported on this platform: " + filePath);
#endif
        }

        public static void SendEmail(string to, string subject, string body)
        {
#if UNITY_EDITOR
            Debug.Log("[ShareService] SendEmail (에디터 스텁): " + to + " / " + subject + "\n" + body);
#else
            string url = "mailto:" + to
                + "?subject=" + System.Uri.EscapeDataString(subject)
                + "&body=" + System.Uri.EscapeDataString(body);
            Application.OpenURL(url);
#endif
        }
    }
}
