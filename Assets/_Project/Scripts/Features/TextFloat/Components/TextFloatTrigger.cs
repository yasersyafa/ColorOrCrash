using ColorOrCrash.Features.Ball.Components;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Global.Components;
using NocturneThree.EventSystem;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.TextFloat.Components
{
    public class TextFloatTrigger : MonoBehaviour
    {
        private GameManager _manager;
        private BallController _ball;
        private int scoreAmount;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _manager = ServiceLocator.Get<GameManager>();
            _ball = GetComponent<BallController>();
        }

        private int CalculateScore()
        {
            return scoreAmount = Random.Range(100, 1001);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                if(collision.gameObject.TryGetComponent<PlayerController>(out var player))
                {
                    if(player.CurrentType == _ball.BallColor)
                    {
                        _manager.AddScore(CalculateScore());
                        EventBus.Publish(new TextFloatEvent(transform.position, scoreAmount.ToString()));
                    }
                    
                }

            }
        }
    }
}
