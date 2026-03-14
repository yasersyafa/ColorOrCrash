using System;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Global.Components
{
    [Service]
    public class PokiService : MonoBehaviour, IGameService
    {
        private static PokiService _instance;

        public static PokiService Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindAnyObjectByType<PokiService>();
                if (_instance != null) return _instance;

                // Auto-create on the PokiUnitySDK singleton so it persists across scenes
                var pokiSDK = PokiUnitySDK.Instance;
                if (pokiSDK != null)
                {
                    _instance = pokiSDK.gameObject.AddComponent<PokiService>();
                }
                else
                {
                    var go = new GameObject("[PokiService]");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<PokiService>();
                }

                return _instance;
            }
        }

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
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.SetMute(true);

            PokiUnitySDK.Instance.commercialBreakCallBack = () =>
            {
                audio?.SetMute(false);
                onComplete?.Invoke();
            };
            PokiUnitySDK.Instance.commercialBreak();
        }

        public void RewardedBreak(Action<bool> onComplete)
        {
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.SetMute(true);

            PokiUnitySDK.Instance.rewardedBreakCallBack = (withReward) =>
            {
                audio?.SetMute(false);
                onComplete?.Invoke(withReward);
            };
            PokiUnitySDK.Instance.rewardedBreak();
        }

        public bool IsAdBlocked()
        {
            return PokiUnitySDK.Instance.isAdBlocked();
        }

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
            ServiceLocator.Unregister<PokiService>();
        }
    }
}
