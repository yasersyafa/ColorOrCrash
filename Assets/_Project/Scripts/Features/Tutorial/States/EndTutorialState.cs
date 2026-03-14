using ColorOrCrash.Features.Tutorial.Components;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NocturneThree.ServiceLocator;
using UnityEngine.InputSystem;

namespace ColorOrCrash.Features.Tutorial.States
{
    public class EndTutorialState : IState<TutorialManager>
    {
        public bool CanMove { get; set; }

        public async UniTask AddTutorial(TutorialManager manager)
        {
            await UniTask.Delay(manager.delayShowTutorial);
            ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            manager.tutorialText.SetText(manager.endText);
            
            ShowTextStart(manager).Forget();
        }

        private async UniTask ShowTextStart(TutorialManager manager)
        {
            manager.startText.gameObject.SetActive(true);
            CanMove = true;
            SaveManager.Save();
            await manager.startText.DOFade(1, 0.5f).SetUpdate(true).From(0).SetLoops(-1, LoopType.Yoyo).ToUniTask();
        }

        public void Enter(TutorialManager manager)
        {
            CanMove = false;
            AddTutorial(manager).Forget();
        }

        public void Execute(TutorialManager manager)
        {
            if(CanMove)
            {
                if(Keyboard.current.enterKey.isPressed || Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                {
                    CanMove = false;
                    ServiceLocator.Get<AudioManager>().StopBGM();
                    PokiService.Instance.CommercialBreak(() =>
                    {
                        ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.GameScene).Forget();
                    });
                }
            }
        }

        public void Exit(TutorialManager manager)
        {
            
        }
    }
}
