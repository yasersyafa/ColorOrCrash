using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash
{
    [Service]
    public class SaveManager : MonoBehaviour, IGameService
    {
        public static string keyTutorial = "tutorial";
        public static bool HasTutorial()
        {
           return PlayerPrefs.HasKey(keyTutorial) && PlayerPrefs.GetInt(keyTutorial) == 1; 
        }  
        private void Awake()
        {
            ServiceLocator.Register<SaveManager>(this);
        }

        public static void Save() => PlayerPrefs.SetInt(keyTutorial, 1);

        void OnDestroy()
        {
            ServiceLocator.Unregister<SaveManager>();
        }
    }
}
