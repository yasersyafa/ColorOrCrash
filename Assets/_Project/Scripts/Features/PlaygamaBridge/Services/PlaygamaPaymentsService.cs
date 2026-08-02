using System;
using System.Collections.Generic;
#if UNITY_WEBGL
using Playgama;
#endif

namespace ColorOrCrash.Features.PlaygamaBridge.Services
{
    public static class PlaygamaPaymentsService
    {
        public static bool IsSupported
        {
            get
            {
#if UNITY_WEBGL
                return Bridge.payments.isSupported;
#else
                return false;
#endif
            }
        }

        public static void Purchase(string id, Action<bool, Dictionary<string, string>> onComplete = null)
        {
#if UNITY_WEBGL
            Bridge.payments.Purchase(id, onComplete);
#else
            onComplete?.Invoke(false, null);
#endif
        }

        public static void ConsumePurchase(string id, Action<bool, Dictionary<string, string>> onComplete = null)
        {
#if UNITY_WEBGL
            Bridge.payments.ConsumePurchase(id, onComplete);
#else
            onComplete?.Invoke(false, null);
#endif
        }

        public static void GetPurchases(Action<bool, List<Dictionary<string, string>>> onComplete)
        {
#if UNITY_WEBGL
            Bridge.payments.GetPurchases(onComplete);
#else
            onComplete?.Invoke(false, null);
#endif
        }

        public static void GetCatalog(Action<bool, List<Dictionary<string, string>>> onComplete)
        {
#if UNITY_WEBGL
            Bridge.payments.GetCatalog(onComplete);
#else
            onComplete?.Invoke(false, null);
#endif
        }
    }
}
