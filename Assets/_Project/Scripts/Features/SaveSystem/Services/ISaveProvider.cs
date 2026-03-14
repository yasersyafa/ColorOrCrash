using ColorOrCrash.Features.SaveSystem.Models;
using UnityEngine;

namespace ColorOrCrash.Features.SaveSystem.Services
{
    public interface ISaveProvider
    {
        void Save(PlayerData data);
        PlayerData Load();
        bool HasSaveData();
    }
}
