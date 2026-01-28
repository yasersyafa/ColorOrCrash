using UnityEngine;

namespace ColorOrCrash
{
    public class ServiceContainer : MonoBehaviour
    {
        private static ServiceContainer _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
    }
}
