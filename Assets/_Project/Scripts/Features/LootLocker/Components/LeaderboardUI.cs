using ColorOrCrash.Features.LootLocker.Services;
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

        [Header("Colors")]
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private Color normalColor = Color.white;

        [Space]
        [SerializeField] private GameObject menuButtonContainer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            // Bersihkan entry lama
            foreach (Transform child in entriesContainer)
                Destroy(child.gameObject);

            string myMemberId = PlayerPrefs.GetString(LeaderboardService.memberKey, "");

            LeaderboardService.GetLeaderboardEntries(5, entries =>
            {
                foreach (var entry in entries)
                {
                    var row = Instantiate(entryPrefab, entriesContainer);
                    var texts = row.GetComponentsInChildren<TMP_Text>();

                    texts[0].text = $"#{entry.rank}";
                    texts[1].text = entry.player.name;
                    texts[2].text = entry.score.ToString();

                    // Highlight kalau ini player sendiri
                    bool isMe = entry.member_id == myMemberId;
                    foreach (var t in texts)
                        t.color = isMe ? highlightColor : normalColor;
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
            var keyboard = Keyboard.current;

            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame && leaderboardPanel.activeInHierarchy)
            {
                leaderboardPanel.SetActive(false);
                // TODO: show menu buttons
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
