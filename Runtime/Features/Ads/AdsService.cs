using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
using GoogleMobileAds.Api;
#endif

// 광고 제어. GoogleMobileAds Unity SDK 직접 사용.
// 게임 고유 요소(유닛ID/게이팅/토스트/애널리틱스)는 제거하고 config + OnAdFailed 이벤트로 역전.
// 에디터에서는 광고 없이 즉시 콜백.
namespace RedMinS
{
    public class AdsConfig
    {
        public string bannerId;
        public string interstitialId;
        public string rewardedId;
        public float bannerRefreshSeconds = 60f;
    }

    public class AdsService : SingletonMonobehaviour<AdsService>
    {
        // 광고 실패 알림 (type: "banner" / "interstitial" / "rewarded")
        public event Action<string> OnAdFailed;

        AdsConfig config = null;

        public void Initialize(AdsConfig config)
        {
            this.config = config;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
            InitMobileAds();
#endif
        }

#if UNITY_EDITOR
        // 구글 애드몹 광고(배너)
        public void ShowBanner()
        {
            Debug.Log("[AdsService] ShowBanner (에디터 스텁)");
        }

        public void HideBanner()
        {
            Debug.Log("[AdsService] HideBanner (에디터 스텁)");
        }

        public void DestroyBanner()
        {
            Debug.Log("[AdsService] DestroyBanner (에디터 스텁)");
        }

        // 전면광고
        public void ShowInterstitial(Action onClosed)
        {
            Debug.Log("[AdsService] ShowInterstitial (에디터 스텁)");
            if (onClosed != null) onClosed();
        }

        public bool IsRewardedReady { get { return true; } }

        // 보상형 광고 (true = 보상 지급)
        public void ShowRewarded(Action<bool> onResult)
        {
            Debug.Log("[AdsService] ShowRewarded (에디터 스텁)");
            if (onResult != null) onResult(true);
        }

#elif UNITY_ANDROID || UNITY_IPHONE
        BannerView bannerView;
        InterstitialAd interstitialAd;
        RewardedAd rewardedAd;

        void InitMobileAds()
        {
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            MobileAds.Initialize(status =>
            {
                LoadInterstitial();
                LoadRewarded();
            });
        }

        public void ShowBanner()
        {
            if (bannerView == null)
            {
                bannerView = new BannerView(config.bannerId, AdSize.Banner, AdPosition.Bottom);
            }
            bannerView.LoadAd(new AdRequest());
            bannerView.Show();
        }

        public void HideBanner()
        {
            if (bannerView != null)
            {
                bannerView.Hide();
            }
        }

        public void DestroyBanner()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
                bannerView = null;
            }
        }

        void LoadInterstitial()
        {
            if (interstitialAd != null) return;

            InterstitialAd.Load(config.interstitialId, new AdRequest(), (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    if (OnAdFailed != null) OnAdFailed("interstitial");
                    return;
                }
                interstitialAd = ad;
            });
        }

        public void ShowInterstitial(Action onClosed)
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                var ad = interstitialAd;
                interstitialAd = null;
                ad.OnAdFullScreenContentClosed += () =>
                {
                    ad.Destroy();
                    LoadInterstitial();
                    if (onClosed != null) onClosed();
                };
                ad.OnAdFullScreenContentFailed += (err) =>
                {
                    ad.Destroy();
                    LoadInterstitial();
                    if (OnAdFailed != null) OnAdFailed("interstitial");
                    if (onClosed != null) onClosed();
                };
                ad.Show();
            }
            else
            {
                LoadInterstitial(); // 다음 노출 대비
                if (onClosed != null) onClosed();
            }
        }

        void LoadRewarded()
        {
            if (rewardedAd != null) return;

            RewardedAd.Load(config.rewardedId, new AdRequest(), (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    if (OnAdFailed != null) OnAdFailed("rewarded");
                    return;
                }
                rewardedAd = ad;
            });
        }

        public bool IsRewardedReady
        {
            get { return rewardedAd != null && rewardedAd.CanShowAd(); }
        }

        public void ShowRewarded(Action<bool> onResult)
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                var ad = rewardedAd;
                rewardedAd = null;
                bool rewarded = false;
                ad.OnAdFullScreenContentClosed += () =>
                {
                    ad.Destroy();
                    LoadRewarded();
                    if (rewarded == false && onResult != null) onResult(false);
                };
                ad.OnAdFullScreenContentFailed += (err) =>
                {
                    ad.Destroy();
                    LoadRewarded();
                    if (OnAdFailed != null) OnAdFailed("rewarded");
                    if (onResult != null) onResult(false);
                };
                ad.Show(reward =>
                {
                    rewarded = true;
                    if (onResult != null) onResult(true);
                });
            }
            else
            {
                LoadRewarded();
                if (OnAdFailed != null) OnAdFailed("rewarded");
                if (onResult != null) onResult(false);
            }
        }
#endif

        // 배너 광고 주기적 갱신
        public void CallBannerAds()
        {
            if (BannerCallerCoroutine != null) StopCoroutine(BannerCallerCoroutine);
            BannerCallerCoroutine = IECallBannerAds();
            StartCoroutine(BannerCallerCoroutine);
        }

        IEnumerator BannerCallerCoroutine;
        IEnumerator IECallBannerAds()
        {
            float refresh = (config != null) ? config.bannerRefreshSeconds : 60f;
            yield return new WaitForSeconds(refresh);

            ShowBanner();
            CallBannerAds();
        }
    }
}
