using ColorOrCrash.Features.LootLocker.Services;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using TMPro;
using UnityEngine;

namespace ColorOrCrash.Features.LootLocker.Components
{
    public class InputPlayerNameUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelObject;
        [SerializeField] TMP_InputField inputField;

        public void SetActivePanel(bool isActive)
        {
            panelObject.SetActive(isActive);
        }

        public void OnContinueButtonClicked()
        {
            string name = inputField.text.Trim();
            if (string.IsNullOrEmpty(name)) return; // validasi jangan kosong

            // Simpan dulu ke PlayerPrefs!
            PlayerPrefs.SetString(LeaderboardService.playerNameKey, name);
            PlayerPrefs.Save();

            LeaderboardService.SetPlayerName(name, _ =>
            {
                ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.MainMenuScene).Forget();
            });
        }
    }
}
