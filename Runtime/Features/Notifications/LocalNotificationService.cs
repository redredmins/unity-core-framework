using System;
using System.Collections;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.Notifications.Android;
using UnityEngine.Android;
#elif UNITY_IOS && !UNITY_EDITOR
using Unity.Notifications.iOS;
#endif

// 로컬 푸시 알림 (Android/iOS).
// 게임 고유 요소(채널ID/문구)는 제거하고 Initialize 파라미터로 역전. 에디터에서는 로그 스텁.
namespace RedMinS
{
    public class LocalNotificationService : SingletonMonobehaviour<LocalNotificationService>
    {
        string channelId = "red_alarm";
        string channelName = "Red";
        string defaultTitle = "Red";

        public void Initialize(string channelId, string channelName, string defaultTitle)
        {
            this.channelId = channelId;
            this.channelName = channelName;
            this.defaultTitle = defaultTitle;

#if UNITY_ANDROID && !UNITY_EDITOR
            var channel = new AndroidNotificationChannel()
            {
                Id = channelId,
                Name = channelName,
                Importance = Importance.Default,
                Description = channelName,
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#elif UNITY_IOS && !UNITY_EDITOR
            StartCoroutine(IERequestNotificationAuthorization());
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        IEnumerator IERequestNotificationAuthorization()
        {
            using (var req = new AuthorizationRequest(
                AuthorizationOption.Alert | AuthorizationOption.Sound, false))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                }
            }
        }
#endif

        // Android 13+ 알림 권한 요청 (iOS는 권한 요청 코루틴 재사용)
        public void RequestPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
#elif UNITY_IOS && !UNITY_EDITOR
            StartCoroutine(IERequestNotificationAuthorization());
#endif
        }

        public void ScheduleAfter(TimeSpan delay, string text, string title = null)
        {
            string useTitle = string.IsNullOrEmpty(title) ? defaultTitle : title;

#if UNITY_EDITOR
            Debug.Log("[LocalNotification] ScheduleAfter (에디터 스텁): delay=" + delay + " title=" + useTitle + " text=" + text);
#elif UNITY_ANDROID
            var noti = new AndroidNotification()
            {
                Title = useTitle,
                Text = text,
                FireTime = DateTime.Now.Add(delay),
            };
            AndroidNotificationCenter.SendNotification(noti, channelId);
#elif UNITY_IOS
            var noti = new iOSNotification()
            {
                Title = useTitle,
                Body = text,
                ShowInForeground = false,
                Trigger = new iOSNotificationTimeIntervalTrigger()
                {
                    TimeInterval = delay,
                    Repeats = false,
                },
            };
            iOSNotificationCenter.ScheduleNotification(noti);
#endif
        }

        public void CancelAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS && !UNITY_EDITOR
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
        }
    }
}
