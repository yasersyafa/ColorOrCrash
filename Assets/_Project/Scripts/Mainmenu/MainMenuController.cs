using System.Collections;
using System.Collections.Generic;
using ColorOfCrash.Utils;
using ColorOrCrash.Global.Components;
using DG.Tweening;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash
{
    public class MainMenuController : MonoBehaviour
    {   
        [Header("UI References")]
        [SerializeField] private GameObject mainMenuContainer; // GameObject kosong yang berisi UI menu utama
        [SerializeField] private GameObject creditsPanel; // Panel untuk credits
        [SerializeField] private List<GameObject> menuButtonList = new();

        [Header("Animation Objects Configurations")]
        [SerializeField] private RectTransform planetObject;
        [SerializeField] private RectTransform titleObject;
        [SerializeField] private List<RectTransform> alienList = new();
        [SerializeField] private GameObject textStartObject;
        [Tooltip("Duration for each tweening")]
        [SerializeField] private float duration = .5f;
        [SerializeField] private float delayDuration = .25f;

        // --------- SEQUENCES ---------------- //
        
        private Sequence hideSequence;
        
        private void Start()
        {
            ServiceLocator.Get<AudioManager>().PlayBGM("MenuMusic");
            // Pastikan credits panel tidak aktif saat mulai
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }
            
            // Pastikan main menu container aktif saat mulai
            if (mainMenuContainer != null)
            {
                mainMenuContainer.SetActive(true);
            }

            ShowAnimationObject();
        }

        /// <summary>
        /// Showing objects such as planet, alien, and text start by sequence
        /// </summary>
        public void ShowAnimationObject()
        {
            ResetAnimationObjects();

            Sequence showSequence = DOTween.Sequence();

            showSequence.AppendInterval(2f).SetUpdate(true);

            showSequence.Append(planetObject.DOScale(Vector3.one, duration)).SetUpdate(true);
            showSequence.AppendInterval(delayDuration);
            
            showSequence.Append(titleObject.DOScale(Vector3.one, duration)).SetUpdate(true);

            foreach(var alien in alienList)
                showSequence.Join(alien.DOScale(Vector3.one, duration)).SetUpdate(true);
            
            showSequence.AppendInterval(delayDuration);

            showSequence.OnComplete(() =>
            {
                textStartObject.SetActive(true);
                StartCoroutine(WaitMenuForClick());
            });

        }

        private IEnumerator WaitMenuForClick()
        {
            yield return YieldCollection.WaitForMouseClick;

            HideAnimationObjects();
        }

        public void HideAnimationObjects()
        {
            hideSequence = DOTween.Sequence();

            textStartObject.SetActive(false);

            hideSequence.Append(planetObject.DOAnchorPosY(1000, duration).SetEase(Ease.InOutBounce)).SetUpdate(true);
            hideSequence.Append(titleObject.DOScale(Vector3.zero, duration)).SetUpdate(true);

            foreach (var alien in alienList)
                hideSequence.Join(alien.DOAnchorPosY(1000, duration).SetEase(Ease.InOutBounce)).SetUpdate(true);

            hideSequence.OnComplete(() =>
            {
                ResetAnimationObjects();
                ShowMenuButton();
            });
        }

        public void ShowMenuButton()
        {
            foreach (var button in menuButtonList)
                button.SetActive(true);
        }

        private void ResetAnimationObjects()
        {
            planetObject.localScale = Vector3.zero;
            titleObject.localScale = Vector3.zero;

            foreach (var alien in alienList)
                alien.localScale = Vector3.zero;
            
            textStartObject.SetActive(false);
        }
        
        private void Update()
        {
            // Jika credits panel aktif dan tombol ESC ditekan, kembali ke main menu
            if (creditsPanel != null && creditsPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    HideCredits();
                }
            }
        }
        
        /// <summary>
        /// Fungsi untuk button Start - Pindah ke scene loading screen
        /// </summary>
        public async void StartGame()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            ServiceLocator.Get<AudioManager>().StopBGM(true);
            await ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(
                SaveManager.HasTutorial() ? 
                ServiceContainer.Instance.Scenes.GameScene : 
                ServiceContainer.Instance.Scenes.TutorialScene
            );
        }
        
        /// <summary>
        /// Fungsi untuk button Credits - Toggle antara main menu dan credits
        /// </summary>
        public void ShowCredits()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            if (mainMenuContainer != null)
            {
                mainMenuContainer.SetActive(false);
            }
            
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(true);
            }
        }
        
        /// <summary>
        /// Fungsi untuk kembali dari credits ke main menu
        /// </summary>
        public void HideCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }
            
            if (mainMenuContainer != null)
            {
                mainMenuContainer.SetActive(true);
            }
        }
        
        /// <summary>
        /// Fungsi untuk button Exit - Keluar dari game
        /// </summary>
        public void ExitGame()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            
            #if UNITY_EDITOR
                // Jika di Unity Editor, stop play mode
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                // Jika di build, quit application
                Application.Quit();
            #endif
        }
    }
}
