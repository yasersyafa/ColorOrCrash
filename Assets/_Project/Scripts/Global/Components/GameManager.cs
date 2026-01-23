using ColorOfCrash.Features.Ball.Components;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOfCrash.Global.Components
{
    [Service]
    public class GameManager : MonoBehaviour, IGameService
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var ballSpawner = ServiceLocator.Get<BallSpawner>();
                
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
