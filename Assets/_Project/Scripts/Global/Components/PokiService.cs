using System;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Global.Components
{
    [Service]
    public class PokiService : MonoBehaviour, IGameService
    {
        private bool _isGameplayActive;

        public bool IsShowingAd => PokiUnitySDK.Instance.isShowingAd;

        public void GameplayStart()
        {
            if (_isGameplayActive || IsShowingAd) return;
            _isGameplayActive = true;
            PokiUnitySDK.Instance.gameplayStart();
        }

        public void GameplayStop()
        {
            if (!_isGameplayActive || IsShowingAd) return;
            _isGameplayActive = false;
            PokiUnitySDK.Instance.gameplayStop();
        }

        public void CommercialBreak(Action onComplete)
        {
#if UNITY_EDITOR
            onComplete?.Invoke();
#else
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.SetMute(true);

            PokiUnitySDK.Instance.commercialBreakCallBack = () =>
            {
                audio?.SetMute(false);
                onComplete?.Invoke();
            };
            PokiUnitySDK.Instance.commercialBreak();
#endif
        }

        public void RewardedBreak(Action<bool> onComplete)
        {
#if UNITY_EDITOR
            onComplete?.Invoke(true);
#else
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.SetMute(true);

            PokiUnitySDK.Instance.rewardedBreakCallBack = (withReward) =>
            {
                audio?.SetMute(false);
                onComplete?.Invoke(withReward);
            };
            PokiUnitySDK.Instance.rewardedBreak();
#endif
        }

        public bool IsAdBlocked()
        {
#if UNITY_EDITOR
            return false;
#else
            return PokiUnitySDK.Instance.isAdBlocked();
#endif
        }

        void OnDestroy()
        {
            ServiceLocator.Unregister<PokiService>();
        }
    }
}
