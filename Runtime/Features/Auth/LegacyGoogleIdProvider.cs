using System;

// Firebase 없이 Google 이메일 로컬파트를 유저 식별자로 쓰는 레거시 로그인 제공자.
// (Credential Manager로 이메일 획득 후 로컬파트 사용)
namespace RedMinS
{
    public class LegacyGoogleIdProvider : IAuthProvider
    {
        readonly string _webClientId;
        bool _isSignedIn = false;

        public string ProviderName => "GoogleLegacy";
        public bool IsSignedIn => _isSignedIn;

        public LegacyGoogleIdProvider(string webClientId)
        {
            _webClientId = webClientId;
        }

        public void SignIn(Action<string> onSuccess, Action<string> onFail)
        {
            GoogleCredentialService.SignIn(_webClientId,
                (email, idToken) =>
                {
                    if (string.IsNullOrEmpty(email))
                    {
                        if (onFail != null) onFail("Google credential returned empty email.");
                        return;
                    }

                    _isSignedIn = true;
                    // '@'가 없으면 Split 결과 [0]은 전체 문자열이라 안전
                    string localPart = email.Split('@')[0];
                    if (onSuccess != null) onSuccess(localPart);
                },
                () =>
                {
                    if (onFail != null) onFail("Google credential sign-in failed.");
                });
        }

        public void SignOut()
        {
            _isSignedIn = false;
        }
    }
}
