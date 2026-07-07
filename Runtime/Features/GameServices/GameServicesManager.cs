using System;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

// 업적/리더보드. Android는 Google Play Games plugin v2 사용.
// 게임 고유 요소(애널리틱스/자동 로그인 시점)는 제거하고 명시적 Authenticate로 역전.
// 에디터에서는 Debug.Log 스텁. (iOS는 별도 단계 — DllImport 미포함)
namespace RedMinS
{
    public class GameServicesManager : SingletonMonobehaviour<GameServicesManager>
    {
#if UNITY_EDITOR
        public void Authenticate(Action<bool> onComplete)
        {
            Debug.Log("[GameServices] Authenticate (에디터 스텁)");
            if (onComplete != null) onComplete(true);
        }

        public void UnlockAchievement(string id)
        {
            Debug.Log("[GameServices] UnlockAchievement : " + id);
        }

        public void IncrementAchievement(string id, int steps)
        {
            Debug.Log("[GameServices] IncrementAchievement : " + id + " / " + steps);
        }

        public void ShowAchievementsUI()
        {
            Debug.Log("[GameServices] ShowAchievementsUI");
        }

        public void SubmitScore(string leaderboardId, long score)
        {
            Debug.Log("[GameServices] SubmitScore : " + leaderboardId + " / " + score);
        }

        public void ShowLeaderboardUI(string leaderboardId)
        {
            Debug.Log("[GameServices] ShowLeaderboardUI : " + leaderboardId);
        }

#elif UNITY_ANDROID
        bool isAuthenticated = false;

        public void Authenticate(Action<bool> onComplete)
        {
            // PGS v2 로그인 (Play Games 프로필이 있으면 무음, UI 없음)
            PlayGamesPlatform.Instance.Authenticate((status) =>
            {
                isAuthenticated = (status == SignInStatus.Success);
                Debug.Log("[GameServices] PGS auth - " + status);
                if (onComplete != null) onComplete(isAuthenticated);
            });
        }

        public void UnlockAchievement(string id)
        {
            if (!isAuthenticated) return;
            PlayGamesPlatform.Instance.UnlockAchievement(id);
        }

        public void IncrementAchievement(string id, int steps)
        {
            if (!isAuthenticated) return;
            PlayGamesPlatform.Instance.IncrementAchievement(id, steps, (success) => { });
        }

        public void ShowAchievementsUI()
        {
            if (!isAuthenticated) return;
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }

        public void SubmitScore(string leaderboardId, long score)
        {
            if (!isAuthenticated) return;
            PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, (success) => { });
        }

        public void ShowLeaderboardUI(string leaderboardId)
        {
            if (!isAuthenticated) return;
            PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardId);
        }

#else
        // 그 외 플랫폼(iOS 등)은 별도 단계에서 처리 — API 표면만 유지
        public void Authenticate(Action<bool> onComplete) { if (onComplete != null) onComplete(false); }
        public void UnlockAchievement(string id) { }
        public void IncrementAchievement(string id, int steps) { }
        public void ShowAchievementsUI() { }
        public void SubmitScore(string leaderboardId, long score) { }
        public void ShowLeaderboardUI(string leaderboardId) { }
#endif
    }
}
