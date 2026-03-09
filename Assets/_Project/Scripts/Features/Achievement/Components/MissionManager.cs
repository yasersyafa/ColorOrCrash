using System.Collections.Generic;
using ColorOrCrash.Features.Achievement.Events;
using ColorOrCrash.Features.Achievement.Models;
using ColorOrCrash.Features.SaveSystem.Models;
using ColorOrCrash.Global.Components; // Import namespace Event Bus kamu
using NocturneThree.EventSystem;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Components
{
    [Service]
    public class MissionManager : MonoBehaviour, IGameService
    {
        public List<AchievementAsset> achievementList = new();

        private void OnEnable()
        {
            EventBus.Subscribe<ProgressUpdateEvent>(OnMissionProgressReceived);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ProgressUpdateEvent>(OnMissionProgressReceived);
        }

        private void OnMissionProgressReceived(ProgressUpdateEvent evt)
        {
            NotifyProgress(evt.Type, evt.Amount, evt.TargetId);
        }

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
                    ach.currentAmount = 0;
                    ach.currentTierIndex = 0;
                    ach.allTiersCompleted = false;
                }
            }
        }

        public void SyncToSaveData()
        {
            var saveManager = ServiceLocator.Get<SaveManager>();
            if (saveManager == null || saveManager.Data == null) return;

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
                }
            }
            
            SyncToSaveData();
        }

        public void NotifyProgress(MissionType type, int amount, string id = "")
        {
            bool hasChanged = false;

            foreach(var ach in achievementList)
            {
                if(ach.type == type) 
                {
                    ach.UpdateProgress(amount, id);
                    hasChanged = true;
                }
            }

            if(hasChanged) SyncToSaveData();
        }
    }
}