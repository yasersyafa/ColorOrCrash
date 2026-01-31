using ColorOrCrash.Global.Models;
using UnityEngine;

namespace ColorOrCrash.Global.Components
{
    public class ServiceContainer : MonoBehaviour
    {
        private static ServiceContainer _instance;
        public static ServiceContainer Instance => _instance;

        [field: SerializeField] public SceneDatabase Scenes { get; private set;}

        private void Awake()
        {
            PokiUnitySDK.Instance.init();
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PokiUnitySDK.Instance.gameLoadingFinished();
        }
    }
}
