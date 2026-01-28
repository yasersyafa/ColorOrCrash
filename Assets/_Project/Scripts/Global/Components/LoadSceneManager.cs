using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorOrCrash.Global.Components
{
    [Service]
    public class LoadSceneManager : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private CanvasGroup loadingCanvas;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float minLoadingTime = 2f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (loadingCanvas != null) loadingCanvas.gameObject.SetActive(false);
        }

        private async UniTask ShowLoadingScreen()
        {
            loadingCanvas.gameObject.SetActive(true);
            loadingCanvas.alpha = 0;
            await loadingCanvas.DOFade(1f, fadeDuration).AsyncWaitForCompletion();
        }

        private async UniTask HideLoadingScreen()
        {
            await loadingCanvas.DOFade(0f, fadeDuration).AsyncWaitForCompletion();
            loadingCanvas.gameObject.SetActive(false);
        }

        public async UniTask LoadSceneAsync(string sceneName)
        {
            await ShowLoadingScreen();

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            float startTime = Time.realtimeSinceStartup;

            while(op.progress < 0.9f)
            {
                await UniTask.Yield();
            }

            float elapsedTime = Time.realtimeSinceStartup - startTime;
            if (elapsedTime < minLoadingTime)
            {
                float remainingTime = minLoadingTime - elapsedTime;
                await UniTask.Delay(TimeSpan.FromSeconds(remainingTime), ignoreTimeScale: true);
            }

            op.allowSceneActivation = true;

            await UniTask.WaitUntil(() => op.isDone);

            await HideLoadingScreen();
        }

        void OnDestroy()
        {
            ServiceLocator.Unregister<LoadSceneManager>();
        }
    }
}
