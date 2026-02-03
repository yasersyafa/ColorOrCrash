using ColorOrCrash.Features.SaveSystem.Models;
using UnityEngine;

namespace ColorOrCrash.Features.SaveSystem.Services
{
    public class PlayerPrefsSaveProvider : ISaveProvider
    {
        private string saveKey = "saveData";
        public bool HasSaveData()
        {
            return PlayerPrefs.HasKey(saveKey);
        }

        public PlayerData Load()
        {
             if (!HasSaveData()) return new PlayerData();

            string json = PlayerPrefs.GetString(saveKey);

            return JsonUtility.FromJson<PlayerData>(json); 
        }

        public void Save(PlayerData data)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(saveKey, json);
        }
    }
}
