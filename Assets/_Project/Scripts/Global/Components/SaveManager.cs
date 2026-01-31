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
            if(PlayerPrefs.HasKey(keyTutorial))
            {
                if(PlayerPrefs.GetInt(keyTutorial) == 1)
                {
                    return true;
                }
                else
                {
                    PokiUnitySDK.Instance.logError("The value of key tutorial is 0");
                    return false;
                }
            } 
            else
            {
                PokiUnitySDK.Instance.logError("The key of tutoial does not exist");
                return false;    
            }
        }  
        private void Awake()
        {
            ServiceLocator.Register<SaveManager>(this);
        }

        public static void Save()
        {
            PlayerPrefs.SetInt(keyTutorial, 1);
            PlayerPrefs.Save();
            if(PlayerPrefs.HasKey(keyTutorial) && PlayerPrefs.GetInt(keyTutorial) == 1)
            {
                PokiUnitySDK.Instance.logError("Success stored data. The value of key tutorial is 1");
            }
        }

        void OnDestroy()
        {
            ServiceLocator.Unregister<SaveManager>();
        }
    }
}
