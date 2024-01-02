using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Core.Managers
{
    public class AdsManager : BaseManager
    {
        public AdsManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            IronSourceEvents.onSdkInitializationCompletedEvent += OnInitializationCompleted;
            
            var developerSettings = Resources.Load<IronSourceMediationSettings>(IronSourceConstants.IRONSOURCE_MEDIATION_SETTING_NAME);
            
            IronSource.Agent.init(developerSettings.AndroidAppKey, IronSourceAdUnits.REWARDED_VIDEO);
        }

        private static void PreInit()
        {
            IronSourceConfig.Instance.setClientSideCallbacks(true);

            string id = IronSource.Agent.getAdvertiserId();
            MyDebug.Log($"Advertiser Id: {id}");

            IronSource.Agent.validateIntegration();
            MyDebug.Log("Integration is Valid");
        }

        private void OnInitializationCompleted()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent -= OnInitializationCompleted;
            OnInitComplete();
        }

        private void LoadRewardAd()
        {
            IronSource.Agent.loadRewardedVideo();
        }
        
        private void LoadAd()
        {
            IronSource.Agent.loadInterstitial();
        }

        private Action<bool> rewardAction;
        
        public bool TryShowRewardAd(Action<bool> onComplete)
        {
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                rewardAction = onComplete;
                IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewarded;
                IronSourceRewardedVideoEvents.onAdOpenedEvent += OnRewardOpened;
                IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnFailReward;
                IronSourceRewardedVideoEvents.onAdClosedEvent += OnRewardedAdClosed;
                
                IronSource.Agent.showRewardedVideo();
                return true;
            }

            return false;
        }

        private void OnRewardedAdClosed(IronSourceAdInfo obj)
        {
            rewardAction?.Invoke(false);
            rewardAction = null;
        }

        private void OnFailReward(IronSourceError arg1, IronSourceAdInfo arg2)
        {
            rewardAction?.Invoke(false);
            rewardAction = null;
        }

        private void OnRewardOpened(IronSourceAdInfo obj)
        {
        }

        private void OnRewarded(IronSourcePlacement arg1, IronSourceAdInfo arg2)
        {
            rewardAction?.Invoke(true);
            rewardAction = null;
        }


        #region AdInterstitial
        
        private Action AdShowCompleteAction;
        
        public void TryShowAd(Action onComplete)
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                AdShowCompleteAction = onComplete;

                AdShowCompleteAction += RemoveAdListeners;
                
                IronSource.Agent.showInterstitial();
                IronSourceInterstitialEvents.onAdOpenedEvent += OnAdOpened;
                IronSourceInterstitialEvents.onAdClosedEvent += OnAdClosed;
                IronSourceInterstitialEvents.onAdShowFailedEvent += OnAdShowFailed;
            }
        }

        private void OnAdShowFailed(IronSourceError arg1, IronSourceAdInfo arg2)
        {
            AdShowCompleteAction.Invoke();
        }

        private void OnAdClosed(IronSourceAdInfo obj)
        {
            AdShowCompleteAction.Invoke();
        }

        private void OnAdOpened(IronSourceAdInfo obj)
        {
            GameManager.AnalyticsManager.SendAnalytics(AnalyticsEventName.AdDisplayed, new Dictionary<string, object>());
        }

        private void RemoveAdListeners()
        {
            IronSourceInterstitialEvents.onAdOpenedEvent -= OnAdOpened;
            IronSourceInterstitialEvents.onAdClosedEvent -= OnAdClosed;
            IronSourceInterstitialEvents.onAdShowFailedEvent -= OnAdShowFailed;
        }
        
        #endregion

        
        public void TryShowAudioAd(Action onComplete)
        {
            
        }
    }
}