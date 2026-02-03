using ColorOrCrash.Features.SaveSystem.Models;
using ColorOrCrash.Features.SaveSystem.Services;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash
{
    [Service]
    public class SaveManager : MonoBehaviour, IGameService
    {
        ISaveProvider _saveProvider;
        public PlayerData Data { get; private set; }

        public static string keyTutorial = "tutorial"; 

        private void Awake()
        {
            ServiceLocator.Register<SaveManager>(this);
            
            _saveProvider = new PlayerPrefsSaveProvider();

            LoadGame();
        }

        private void Start()
        {
            _saveProvider.Load();
        }

        public void LoadGame()
        {
            Data = _saveProvider.Load();
            Data ??= new PlayerData();
        }

        public void SaveGame()
        {
            _saveProvider.Save(Data);
            PlayerPrefs.Save();
        }

        public static void Save() => PlayerPrefs.SetInt(keyTutorial, 1);
        public static bool HasTutorial() => PlayerPrefs.HasKey(keyTutorial) && PlayerPrefs.GetInt(keyTutorial) == 1;

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) SaveGame();
        }

        void OnDestroy()
        {
            SaveGame();
            ServiceLocator.Unregister<SaveManager>();
        }
    }
}
