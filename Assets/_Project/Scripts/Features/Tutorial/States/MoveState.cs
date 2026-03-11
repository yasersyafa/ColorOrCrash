using System;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Features.Tutorial.Components;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Tutorial.States
{
    public class MoveState : IState<TutorialManager>
    {
        public bool CanMove { get; set; }
        private PlayerTutorial player;
        private bool isTaskCompleted;

        public void Enter(TutorialManager manager)
        {
            ServiceLocator.Get<AudioManager>().PlayBGM("TutorialMusic");
            isTaskCompleted = false;
            player = manager.player;
            CanMove = false;
            AddTutorial(manager).Forget();
        }

        public void Execute(TutorialManager manager)
        {
            if(CanMove && !isTaskCompleted && player != null)
            {
                if(player.HasMovedLeft && player.HasMovedRight)
                {
                    isTaskCompleted = true;
                    CompleteTutorial(manager).Forget();
                }
            }
        }

        public void Exit(TutorialManager manager)
        {
            
        }

        private async UniTask CompleteTutorial(TutorialManager manager)
        {
            manager.tutorialText.SetText("GREAT MOVE!");
            ServiceLocator.Get<AudioManager>().PlaySFX("Score");
            await UniTask.Delay(TimeSpan.FromSeconds(manager.delayShowTutorial), ignoreTimeScale: true);
            manager.ChangeState(TutorialState.Jump);
        }

        public async UniTask AddTutorial(TutorialManager manager)
        {
            manager.tutorialText.SetText("WELCOME TO TUTORIAL");
            await UniTask.Delay(TimeSpan.FromSeconds(manager.delayShowTutorial), ignoreTimeScale: true);
            // ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            // manager.tutorialText.SetText(manager.moveText);
            await manager.TypeEffectText(manager.player.IsMobileDevice() ? manager.moveTextMobile : manager.moveText);
            CanMove = true;
        }
    }
}
