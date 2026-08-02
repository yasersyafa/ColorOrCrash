using System;
#if UNITY_WEBGL
using Playgama;
#endif

namespace ColorOrCrash.Features.PlaygamaBridge.Services
{
    public static class PlaygamaPlayerService
    {
        private const string FallbackName = "Guest";

        public static string DisplayName
        {
            get
            {
#if UNITY_WEBGL
                try
                {
                    var name = Bridge.player.name;
                    return string.IsNullOrEmpty(name) ? FallbackName : name;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning($"[PlaygamaPlayerService] DisplayName failed: {e.Message}");
                    return FallbackName;
                }
#else
                return FallbackName;
#endif
            }
        }

        public static string Id
        {
            get
            {
#if UNITY_WEBGL
                try
                {
                    return Bridge.player.id;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning($"[PlaygamaPlayerService] Id failed: {e.Message}");
                    return null;
                }
#else
                return null;
#endif
            }
        }
    }
}
