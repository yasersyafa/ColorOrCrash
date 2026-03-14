using System;
using ColorOrCrash.Features.Tutorial.Components;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Tutorial.States
{
    public class HoldJumpState : IState<TutorialManager>
    {
        public bool CanMove { get; set; }
        private PlayerTutorial _player;
        private bool _isTaskCompleted;
        
        // Tentukan ambang batas tinggi lompatan. 
        // Sesuaikan nilainya dengan jumpForce di PlayerConfig kamu.
        private const float TARGET_HEIGHT_THRESHOLD = 2.5f; 
        private float _startHeight;

        public void Enter(TutorialManager manager)
        {
            CanMove = false;
            _isTaskCompleted = false;
            
            _player = manager.player;
            if (_player != null)
            {
                _player.ResetTutorialFlags();
                _startHeight = _player.transform.position.y;
            }

            AddTutorial(manager).Forget();
        }

        public void Execute(TutorialManager manager)
        {
            if (CanMove && !_isTaskCompleted && _player != null)
            {
                float relativeHeight = _player.transform.position.y - _startHeight;

                if (relativeHeight >= TARGET_HEIGHT_THRESHOLD)
                {
                    _isTaskCompleted = true;
                    CompleteHoldJumpTutorial(manager).Forget();
                }
                
                if (_player.IsGrounded)
                {
                    _startHeight = _player.transform.position.y;
                }
            }
        }

        private async UniTaskVoid CompleteHoldJumpTutorial(TutorialManager manager)
        {
            manager.tutorialText.SetText("SO HIGH! WELL DONE!");
            ServiceLocator.Get<AudioManager>().PlaySFX("Score");
            
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), ignoreTimeScale: true);
            
            // Lanjut ke tutorial terakhir: Double Jump
            manager.ChangeState(TutorialState.DoubleJump);
        }

        public void Exit(TutorialManager manager) { }

        public async UniTask AddTutorial(TutorialManager manager)
        {
            // ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            manager.tutorialText.SetText("DID YOU KNOW?");
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), ignoreTimeScale: true);
            
            await manager.TypeEffectText(manager.player.IsMobileDevice() ? manager.holdJumpTextMobile : manager.holdJumpText);
            
            CanMove = true;
        }
    }
}