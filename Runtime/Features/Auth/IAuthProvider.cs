using System;

namespace RedMinS
{
    public interface IAuthProvider
    {
        /// <summary>
        /// 인증 제공자 이름 (예: "Google", "Anonymous", "Kakao")
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 로그인을 수행합니다.
        /// 성공 시 Firebase UID를 반환합니다.
        /// </summary>
        void SignIn(Action<string> onSuccess, Action<string> onFail);

        /// <summary>
        /// 로그아웃을 수행합니다.
        /// </summary>
        void SignOut();

        /// <summary>
        /// 현재 로그인 상태인지 확인합니다.
        /// </summary>
        bool IsSignedIn { get; }
    }
}
