using ColorOrCrash.Features.LootLocker.Services;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using LootLocker.Requests;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Events;

namespace ColorOrCrash.Features.LootLocker.Components
{
    public class SessionManager : MonoBehaviour
    {
        public UnityEvent OnPlayerNameEmpty;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            LootLockerSDKManager.StartGuestSession( response =>
            {
                if (!response.success)
                {
                    Debug.LogWarning("LootLocker session failed, skipping leaderboard");
                    ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.MainMenuScene).Forget();
                    return;
                }

                LeaderboardService.IsSessionActive = true;

                try { PlayerPrefs.SetString(LeaderboardService.memberKey, response.player_id.ToString()); }
                catch (System.Exception) { }

                string savedName = "";
                try { savedName = PlayerPrefs.GetString(LeaderboardService.playerNameKey, ""); }
                catch (System.Exception) { }

                if (!string.IsNullOrEmpty(savedName))
                {
                    LeaderboardService.SetPlayerName(savedName, _ =>
                    {
                        ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.MainMenuScene).Forget();
                    });
                }
                else
                {
                    OnPlayerNameEmpty?.Invoke();
                }

                Debug.Log($"success guest session with player id: {response.player_id}");
            });
        }
    }
}
