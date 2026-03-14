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
                    Debug.Log("error starting guest session");
                    return;
                }

                PlayerPrefs.SetString(LeaderboardService.memberKey, response.player_id.ToString());

                string savedName = PlayerPrefs.GetString(LeaderboardService.playerNameKey, "");

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
