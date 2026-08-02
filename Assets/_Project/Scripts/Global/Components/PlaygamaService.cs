using System;
using NocturneThree.ServiceLocator;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Advertisement;
using Playgama.Modules.Platform;
#endif

namespace ColorOrCrash.Global.Components
{
    [Service]
    public class PlaygamaService : MonoBehaviour, IGameService
    {
        private bool _isGameplayActive;

        public void GameplayStart()
        {
            _isGameplayActive = true;
        }

        public void GameplayStop()
        {
            _isGameplayActive = false;
        }

        public void NotifyGameReady()
        {
#if UNITY_WEBGL
            Bridge.platform.SendMessage(PlatformMessage.GameReady);
            Bridge.platform.SendMessage(PlatformMessage.InGameLoadingStopped);
#endif
        }

        public void NotifyLevelStarted()
        {
#if UNITY_WEBGL
            Bridge.platform.SendMessage(PlatformMessage.LevelStarted);
#endif
        }

        public void NotifyLevelCompleted()
        {
#if UNITY_WEBGL
            Bridge.platform.SendMessage(PlatformMessage.LevelCompleted);
#endif
        }

        public void CommercialBreak(Action onComplete)
        {
#if UNITY_WEBGL
            void Handler(InterstitialState state)
            {
                if (state != InterstitialState.Closed && state != InterstitialState.Failed) return;

                Bridge.advertisement.interstitialStateChanged -= Handler;
                onComplete?.Invoke();
            }

            Bridge.advertisement.interstitialStateChanged += Handler;
            Bridge.advertisement.ShowInterstitial();
#else
            onComplete?.Invoke();
#endif
        }

        public void RewardedBreak(Action<bool> onComplete)
        {
#if UNITY_WEBGL
            bool wasRewarded = false;

            void Handler(RewardedState state)
            {
                if (state == RewardedState.Rewarded)
                {
                    wasRewarded = true;
                    return;
                }

                if (state != RewardedState.Closed && state != RewardedState.Failed) return;

                Bridge.advertisement.rewardedStateChanged -= Handler;
                onComplete?.Invoke(wasRewarded);
            }

            Bridge.advertisement.rewardedStateChanged += Handler;
            Bridge.advertisement.ShowRewarded();
#else
            onComplete?.Invoke(false);
#endif
        }

        public void IsAdBlocked(Action<bool> callback)
        {
#if UNITY_WEBGL
            Bridge.advertisement.CheckAdBlock(callback);
#else
            callback?.Invoke(false);
#endif
        }

        void OnDestroy()
        {
            ServiceLocator.Unregister<PlaygamaService>();
        }
    }
}
