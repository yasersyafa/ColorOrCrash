using System;
using ColorOrCrash.Features.SaveSystem.Models;
using UnityEngine;

namespace ColorOrCrash.Features.SaveSystem.Services
{
    public class PlayerPrefsSaveProvider : ISaveProvider
    {
        private string saveKey = "saveData";

        public bool HasSaveData()
        {
            try
            {
                return PlayerPrefs.HasKey(saveKey);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PlayerData Load()
        {
            try
            {
                if (!HasSaveData()) return new PlayerData();

                string json = PlayerPrefs.GetString(saveKey);
                return JsonUtility.FromJson<PlayerData>(json) ?? new PlayerData();
            }
            catch (Exception)
            {
                return new PlayerData();
            }
        }

        public void Save(PlayerData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(saveKey, json);
            }
            catch (Exception)
            {
                // Silently fail in incognito mode
            }
        }
    }
}
