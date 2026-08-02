using System;
using NocturneThree.ServiceLocator;

namespace ColorOrCrash.Global.Components
{
    /// <summary>
    /// Dispatches to PokiService or PlaygamaService depending on the active build profile's
    /// PLAYGAMA_BUILD define, so callers don't need to branch per build target.
    /// </summary>
    public static class AdsBridge
    {
        public static void GameplayStart()
        {
#if PLAYGAMA_BUILD
            ServiceLocator.Get<PlaygamaService>()?.GameplayStart();
#else
            ServiceLocator.Get<PokiService>()?.GameplayStart();
#endif
        }

        public static void GameplayStop()
        {
#if PLAYGAMA_BUILD
            ServiceLocator.Get<PlaygamaService>()?.GameplayStop();
#else
            ServiceLocator.Get<PokiService>()?.GameplayStop();
#endif
        }

        public static void CommercialBreak(Action onComplete)
        {
#if PLAYGAMA_BUILD
            var service = ServiceLocator.Get<PlaygamaService>();
#else
            var service = ServiceLocator.Get<PokiService>();
#endif
            if (service != null)
            {
                service.CommercialBreak(onComplete);
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        public static void RewardedBreak(Action<bool> onComplete)
        {
#if PLAYGAMA_BUILD
            var service = ServiceLocator.Get<PlaygamaService>();
#else
            var service = ServiceLocator.Get<PokiService>();
#endif
            if (service != null)
            {
                service.RewardedBreak(onComplete);
            }
            else
            {
                onComplete?.Invoke(false);
            }
        }

        public static void NotifyGameReady()
        {
#if PLAYGAMA_BUILD
            ServiceLocator.Get<PlaygamaService>()?.NotifyGameReady();
#endif
        }

        public static void NotifyLevelStarted()
        {
#if PLAYGAMA_BUILD
            ServiceLocator.Get<PlaygamaService>()?.NotifyLevelStarted();
#endif
        }

        public static void NotifyLevelCompleted()
        {
#if PLAYGAMA_BUILD
            ServiceLocator.Get<PlaygamaService>()?.NotifyLevelCompleted();
#endif
        }
    }
}
