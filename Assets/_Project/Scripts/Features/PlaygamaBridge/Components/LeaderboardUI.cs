using System.Collections.Generic;
using ColorOrCrash.Features.PlaygamaBridge.Services;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ColorOrCrash
{
    public class LeaderboardUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject leaderboardPanel;

        [Header("Entries")]
        [SerializeField] private Transform entriesContainer;
        [SerializeField] private GameObject entryPrefab;
        [SerializeField] private GameObject notAvailableLabel;

        [Header("Colors")]
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color oddRowColor  = new Color(0.25f, 0.31f, 1f);    // #4050FF
        [SerializeField] private Color evenRowColor = new Color(0.20f, 0.25f, 0.88f); // #3241E0

        [Space]
        [SerializeField] private GameObject menuButtonContainer;

        void Start()
        {
            leaderboardPanel.SetActive(false);
        }

        public void Show()
        {
            leaderboardPanel.SetActive(true);
            LoadEntries();
        }

        public void Hide()
        {
            leaderboardPanel.SetActive(false);
        }

        private void LoadEntries()
        {
            foreach (Transform child in entriesContainer)
                Destroy(child.gameObject);

            if (notAvailableLabel != null)
                notAvailableLabel.SetActive(false);

            if (!PlaygamaLeaderboardService.IsAvailable)
            {
                if (notAvailableLabel != null)
                    notAvailableLabel.SetActive(true);
                return;
            }

            string myId = PlaygamaPlayerService.Id;

            PlaygamaLeaderboardService.GetEntries(10, (success, entries) =>
            {
                if (!success || entries == null)
                {
                    if (notAvailableLabel != null)
                        notAvailableLabel.SetActive(true);
                    return;
                }

                int index = 0;
                foreach (var entry in entries)
                {
                    var row = Instantiate(entryPrefab, entriesContainer);
                    var texts = row.GetComponentsInChildren<TMP_Text>();

                    string rank = GetValue(entry, "rank", "position");
                    string name = GetValue(entry, "name", "player_name", "playerName");
                    string score = GetValue(entry, "score", "value");
                    string id = GetValue(entry, "id", "player_id", "playerId", "memberId");

                    texts[0].text = $"#{rank}";
                    texts[1].text = name;
                    texts[2].text = score;

                    bool isMe = !string.IsNullOrEmpty(myId) && id == myId;
                    foreach (var t in texts)
                        t.color = isMe ? highlightColor : normalColor;

                    if (row.TryGetComponent<Image>(out var rowImage))
                    {
                        bool isOdd = (index % 2) == 0;
                        rowImage.color = isOdd ? oddRowColor : evenRowColor;
                    }

                    index++;
                }
            });
        }

        private static string GetValue(Dictionary<string, string> entry, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (entry.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
                    return value;
            }

            return "";
        }

        void Update()
        {
            var keyboard = Keyboard.current;

            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame && leaderboardPanel.activeInHierarchy)
            {
                leaderboardPanel.SetActive(false);
                menuButtonContainer.SetActive(true);
                ShowMenuButtons();
            }
        }

        private void ShowMenuButtons()
        {
            var menuController = FindAnyObjectByType<MainMenuController>(FindObjectsInactive.Include);

            if (menuController != null)
            {
                menuController.ShowMenuButton();
            }
        }
    }
}
