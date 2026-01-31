using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash
{
    [Service]
    public class SaveManager : MonoBehaviour, IGameService
    {
        public static string keyTutorial = "tutorial";
        public static string keyScore = "score";

        private void Awake()
        {
            ServiceLocator.Register<SaveManager>(this);
        }

#region public API methods
        
        public static bool HasTutorial()
        {
           return PlayerPrefs.HasKey(keyTutorial) && PlayerPrefs.GetInt(keyTutorial) == 1; 
        }

        public static void SetHighScore(int score)
        {
            if(PlayerPrefs.HasKey(keyScore))
            {
                bool highScore = score > PlayerPrefs.GetInt(keyScore);
                if(highScore)
                {
                    PlayerPrefs.SetInt(keyScore, score);
                }
            }
            else
            {
                PlayerPrefs.SetInt(keyScore, score);
            }
        }

        public static void Save() => PlayerPrefs.SetInt(keyTutorial, 1);

#endregion
        void OnDestroy()
        {
            ServiceLocator.Unregister<SaveManager>();
        }
    }
}
