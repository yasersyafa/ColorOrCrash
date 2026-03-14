using System;
using LootLocker.Requests;
using UnityEngine;

namespace ColorOrCrash.Features.LootLocker.Services
{
    public static class LeaderboardService
    {
        const string leaderboardKey = "janeismine3";
        public static string memberKey = "member_id";
        public static string playerNameKey = "player_name";

        public static bool IsSessionActive { get; set; }

        public static bool HasPlayerName
        {
            get
            {
                try { return PlayerPrefs.HasKey(playerNameKey) && !string.IsNullOrEmpty(PlayerPrefs.GetString(playerNameKey)); }
                catch (Exception) { return false; }
            }
        }

        public static void SetPlayerName(string playerName, Action<PlayerNameResponse> onComplete = null)
        {
            if (!IsSessionActive) { onComplete?.Invoke(null); return; }
            LootLockerSDKManager.SetPlayerName(playerName, onComplete);
        }

        public static void TrySubmitScore(string memberId, int score)
        {
            if (!IsSessionActive) return;
            LootLockerSDKManager.SubmitScore(memberId, score, leaderboardKey, response =>
            {
                if (!response.success)
                {
                    Debug.LogWarning("Could not submit score: " + response.errorData);
                    return;
                }
                Debug.Log("Successfully submitted score!");
            });
        }

        public static void GetLeaderboardEntries(int count, Action<LootLockerLeaderboardMember[]> onSuccess = null)
        {
            if (!IsSessionActive) return;
            LootLockerSDKManager.GetScoreList(leaderboardKey, count, response =>
            {
                if (!response.success)
                {
                    Debug.LogWarning("Could not get score list: " + response.errorData);
                    return;
                }
                onSuccess?.Invoke(response.items);
            });
        }
    }
}
