using ColorOrCrash.Global.Components;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash
{
    public class TestingLoading : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync("GameScene - Yaser");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
