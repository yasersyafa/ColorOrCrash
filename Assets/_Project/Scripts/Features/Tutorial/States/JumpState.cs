using System;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Features.Tutorial.Components;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;

namespace ColorOrCrash.Features.Tutorial.States
{
    public class JumpState : IState<TutorialManager>
    {
        public bool CanMove { get; set; }
        private PlayerTutorial _player;
        private bool _isTaskCompleted;

        public void Enter(TutorialManager manager)
        {
            CanMove = false;
            _isTaskCompleted = false;
            
            _player = UnityEngine.Object.FindFirstObjectByType<PlayerTutorial>();
            if (_player != null) _player.ResetTutorialFlags();

            AddTutorial(manager).Forget();
        }

        public void Execute(TutorialManager manager)
        {
            if (CanMove && !_isTaskCompleted && _player != null)
            {
                // SYARAT LULUS: Player sudah menekan lompat DAN sudah kembali menyentuh tanah
                if (_player.HasJumped && _player.IsGrounded)
                {
                    _isTaskCompleted = true;
                    CompleteJumpTutorial(manager).Forget();
                }
            }
        }

        private async UniTaskVoid CompleteJumpTutorial(TutorialManager manager)
        {
            manager.tutorialText.SetText("AWESOME JUMP!");
            ServiceLocator.Get<AudioManager>().PlaySFX("Score");
            
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), ignoreTimeScale: true);
            
            // Lanjut ke tutorial berikutnya (Hold Jump)
            manager.ChangeState(TutorialState.HoldJump);
        }

        public void Exit(TutorialManager manager) { }

        public async UniTask AddTutorial(TutorialManager manager)
        {
            // ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            manager.tutorialText.SetText("NOW, TRY TO JUMP!");
            
            // Delay sedikit agar pemain bersiap
            await UniTask.Delay(TimeSpan.FromSeconds(manager.delayShowTutorial), ignoreTimeScale: true);
            
            await manager.TypeEffectText(manager.jumpText);
            
            CanMove = true;
        }
    }
}
