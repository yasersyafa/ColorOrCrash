using System;
using System.Collections.Generic;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Leaderboards;
#endif

namespace ColorOrCrash.Features.PlaygamaBridge.Services
{
    public static class PlaygamaLeaderboardService
    {
        private const string LeaderboardId = "main";

        public static bool IsAvailable
        {
            get
            {
#if UNITY_WEBGL
                try
                {
                    return Bridge.leaderboards.type != LeaderboardType.NotAvailable;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning($"[PlaygamaLeaderboardService] IsAvailable failed: {e.Message}");
                    return false;
                }
#else
                return false;
#endif
            }
        }

        public static void SubmitScore(int score, Action<bool> onComplete = null)
        {
#if UNITY_WEBGL
            try
            {
                Bridge.leaderboards.SetScore(LeaderboardId, score, onComplete);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"[PlaygamaLeaderboardService] SubmitScore failed: {e.Message}");
                onComplete?.Invoke(false);
            }
#else
            onComplete?.Invoke(false);
#endif
        }

        public static void GetEntries(int count, Action<bool, List<Dictionary<string, string>>> onComplete)
        {
#if UNITY_WEBGL
            try
            {
                Bridge.leaderboards.GetEntries(LeaderboardId, (success, entries) =>
                {
                    if (success && entries != null && entries.Count > count)
                    {
                        entries = entries.GetRange(0, count);
                    }

                    onComplete?.Invoke(success, entries);
                });
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"[PlaygamaLeaderboardService] GetEntries failed: {e.Message}");
                onComplete?.Invoke(false, null);
            }
#else
            onComplete?.Invoke(false, null);
#endif
        }

        public static void ShowNativePopup(Action<bool> onComplete = null)
        {
#if UNITY_WEBGL
            try
            {
                Bridge.leaderboards.ShowNativePopup(LeaderboardId, onComplete);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"[PlaygamaLeaderboardService] ShowNativePopup failed: {e.Message}");
                onComplete?.Invoke(false);
            }
#else
            onComplete?.Invoke(false);
#endif
        }
    }
}
