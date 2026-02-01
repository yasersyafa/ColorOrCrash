using System;
using ColorOrCrash.Features.Tutorial.Components;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Tutorial.States
{
    public class DoubleJumpState : IState<TutorialManager>
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
                // SYARAT LULUS: Player berhasil menekan lompat dua kali 
                // Kita tidak perlu tunggu mendarat agar sensasi "Double Jump"-nya langsung terasa dihargai
                if (_player.HasDoubleJumped)
                {
                    _isTaskCompleted = true;
                    CompleteDoubleJumpTutorial(manager).Forget();
                }
            }
        }

        private async UniTaskVoid CompleteDoubleJumpTutorial(TutorialManager manager)
        {
            manager.tutorialText.SetText("INCREDIBLE! YOU MASTERED IT!");
            ServiceLocator.Get<AudioManager>().PlaySFX("Score");
            
            // Beri waktu pemain untuk mendarat dan menikmati animasinya
            await UniTask.Delay(TimeSpan.FromSeconds(2f), ignoreTimeScale: true);
            
            // Pindah ke state akhir untuk menutup tutorial
            manager.ChangeState(TutorialState.EndTutorial);
        }

        public void Exit(TutorialManager manager) { }

        public async UniTask AddTutorial(TutorialManager manager)
        {
            // ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            manager.tutorialText.SetText("ONE LAST TRICK...");
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), ignoreTimeScale: true);
            
            await manager.TypeEffectText(manager.doubleJumpText);
            
            CanMove = true;
        }
    }
}