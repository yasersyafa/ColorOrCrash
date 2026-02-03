using System.Collections.Generic;
using ColorOrCrash.Features.Achievement.Models;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Components
{
    [Service]
    public class MissionManager : MonoBehaviour, IGameService
    {
        public List<AchievementAsset> achievementList = new();
        
        public void OnGameOver()
        {
            foreach(var ach in achievementList)
            {
                if(ach.isSingleRun && !ach.allTiersCompleted)
                {
                    ach.ResetProgress();
                    #if UNITY_EDITOR
                    Debug.LogWarning($"Progress {ach.title} has been reset by program");
                    #endif
                }
            }
        }

        public void NotifyProgress(MissionType type, int amount, string id = "")
        {
            foreach(var ach in achievementList)
            {
                if(ach.type == type) ach.UpdateProgress(amount, id);
            }
        }
    }
}
