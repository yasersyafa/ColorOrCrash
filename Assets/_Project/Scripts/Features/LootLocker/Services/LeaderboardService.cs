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

        public static bool HasPlayerName => PlayerPrefs.HasKey(playerNameKey) && !string.IsNullOrEmpty(PlayerPrefs.GetString(playerNameKey));

        public static void SetPlayerName(string playerName, Action<PlayerNameResponse> onComplete = null)
        {
            LootLockerSDKManager.SetPlayerName(playerName, onComplete);
        }
        
        public static void TrySubmitScore(string memberId, int score)
        {
            LootLockerSDKManager.SubmitScore(memberId, score, leaderboardKey, response =>
            {
                if (!response.success) {
                    Debug.Log("Could not submit score!");
                    Debug.Log(response.errorData.ToString());
                    return;
                }

                Debug.Log("Successfully submitted score!");
            });
        }

        public static void GetLeaderboardEntries(int count, Action<LootLockerLeaderboardMember[]> onSuccess = null)
        {
            LootLockerSDKManager.GetScoreList(leaderboardKey, count, response =>
            {
                if (!response.success) {
                    Debug.Log("Could not get score list!");
                    Debug.Log(response.errorData.ToString());
                    return;
                } 

                onSuccess?.Invoke(response.items);
                Debug.Log("Successfully get score!");
            });
        }
    }
}
