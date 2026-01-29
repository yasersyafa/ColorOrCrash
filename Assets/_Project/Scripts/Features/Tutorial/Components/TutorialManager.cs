using System;
using ColorOrCrash.Features.Tutorial.States;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorOrCrash.Features.Tutorial.Components
{
    public interface IState<T> where T : MonoBehaviour
    {
        bool CanMove { get; set; }
        void Enter(T manager);
        void Execute(T manager);
        void Exit(T manager);
        UniTask AddTutorial(T manager);  
    }

    public enum TutorialState
    {
        Move,
        Jump,
        HoldJump,
        DoubleJump,
        EndTutorial
    }

    public class TutorialManager : MonoBehaviour
    {
        public IState<TutorialManager> CurrentState { get; private set; }
        public TutorialState currentState;
        
        [Header("Tutorial Configuration")]
        public TMP_Text tutorialText;
        public TMP_Text startText;
        public int delayShowTutorial = 2;

        [Space]
        [Header("Text Tutorial")]
        [TextArea(3, 10)] public string moveText;
        [TextArea(3, 10)] public string jumpText, holdJumpText, doubleJumpText;
        [TextArea(3, 10)] public string endText;

        #region States
        public MoveState moveState = new();
        public JumpState jumpState = new();
        public HoldJumpState holdJumpState = new();
        public DoubleJumpState doubleJumpState = new();
        public EndTutorialState endTutorialState = new();
        #endregion

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ChangeState(TutorialState.Move);
        }

        // Update is called once per frame
        void Update()
        {
            CurrentState?.Execute(this);
        }

        private void ChangeState(IState<TutorialManager> newState)
        {
            CurrentState?.Exit(this);
            CurrentState = newState;
            CurrentState?.Enter(this);    
        }

        #region Public API Methods

        public void ChangeState(TutorialState newState)
        {
            currentState = newState;
            switch (currentState)
            {
                case TutorialState.Move:
                    ChangeState(moveState);
                    break;
                case TutorialState.Jump:
                    ChangeState(jumpState);
                    break;
                case TutorialState.HoldJump:
                    ChangeState(holdJumpState);
                    break;
                case TutorialState.DoubleJump:
                    ChangeState(doubleJumpState);
                    break;
                case TutorialState.EndTutorial:
                    ChangeState(endTutorialState);
                    break;
                default:
                    break;
            }
        }

        public async UniTask TypeEffectText(string text)
        {
            tutorialText.text = string.Empty;

            foreach(char c in text)
            {
                tutorialText.text += c;
                ServiceLocator.Get<AudioManager>().PlaySFX("Type");
                await UniTask.Delay(TimeSpan.FromSeconds(0.03f), ignoreTimeScale: true);
            }
        }


        #endregion
    }
}
