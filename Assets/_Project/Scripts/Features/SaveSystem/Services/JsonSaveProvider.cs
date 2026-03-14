using System.IO;
using ColorOrCrash.Features.SaveSystem.Models;
using UnityEngine;

namespace ColorOrCrash.Features.SaveSystem.Services
{
    public class JsonSaveProvider : ISaveProvider
    {
        private string SavePath => Path.Combine(Application.persistentDataPath, "player_profile.json");
        
        public bool HasSaveData()
        {
            return File.Exists(SavePath);
        }

        public PlayerData Load()
        {
            if(!HasSaveData()) return new PlayerData();

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }

        public void Save(PlayerData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }
    }
}
