using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
#endif

namespace ColorOrCrash.Features.PlaygamaBridge.Components
{
    /// <summary>
    /// Replaces the old LootLocker guest-session flow: player identity now comes from the
    /// host platform via Bridge.player, so there's no session to start or name to prompt for.
    /// Authorize() is fired off in the background (not awaited) so it never blocks the
    /// transition to MainMenu on the Editor's manual "Authorize Player" mock popup.
    /// </summary>
    public class GuestPanelBootstrap : MonoBehaviour
    {
        void Start()
        {
#if UNITY_WEBGL && PLAYGAMA_BUILD
            try
            {
                if (Bridge.player.isAuthorizationSupported)
                {
                    Bridge.player.Authorize();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[GuestPanelBootstrap] Authorize failed: {e.Message}");
            }
#endif
            GoToMainMenu();
        }

        private void GoToMainMenu()
        {
            ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.MainMenuScene).Forget();
        }
    }
}
