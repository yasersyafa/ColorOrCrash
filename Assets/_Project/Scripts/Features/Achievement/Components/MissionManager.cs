using System.Collections.Generic;
using ColorOrCrash.Features.Achievement.Models;
using ColorOrCrash.Features.SaveSystem.Models;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Components
{
    [Service]
    public class MissionManager : MonoBehaviour, IGameService
    {
        public List<AchievementAsset> achievementList = new();

        private void Start()
        {
            LoadProgressFromSave();
        }

        public void LoadProgressFromSave()
        {
            var saveManager = ServiceLocator.Get<SaveManager>();
            if (saveManager.Data == null) return;

            foreach (var ach in achievementList)
            {
                var savedEntry = saveManager.Data.achievementProgress.Find(x => x.title == ach.title);
                if (savedEntry != null)
                {
                    ach.currentAmount = savedEntry.currentAmount;
                    ach.currentTierIndex = savedEntry.currentTierIndex;
                    ach.allTiersCompleted = savedEntry.allTiersCompleted;
                }
                else
                {
                    ach.ResetProgress();
                    ach.currentTierIndex = 0;
                    ach.allTiersCompleted = false;
                }
            }
        }

        public void SyncToSaveData()
        {
            var saveManager = ServiceLocator.Get<SaveManager>();
            saveManager.Data.achievementProgress.Clear();

            foreach (var ach in achievementList)
            {
                saveManager.Data.achievementProgress.Add(new AchievementEntry
                {
                    title = ach.title,
                    currentAmount = ach.currentAmount,
                    currentTierIndex = ach.currentTierIndex,
                    allTiersCompleted = ach.allTiersCompleted
                });
            }
        }
        
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

            SyncToSaveData();
        }
    }
}
