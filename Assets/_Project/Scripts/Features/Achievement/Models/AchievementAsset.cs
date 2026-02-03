using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Models
{
    [Serializable]
    public struct AchievementTier
    {
        public string tierName;
        public int goalAmount;
        // TODO: add reward
    }

    public enum MissionType
    {
        ScoreReach,
        EatObstacle,
        Jump,
        DoubleJump,
        UseItem
    }

    /// <summary>
    /// Achievement Asset for conatiner data
    /// </summary>
    [CreateAssetMenu(fileName = "Ach_", menuName = "Chroma Krash/AchievementAsset")]
    public class AchievementAsset : ScriptableObject
    {
        [Tooltip("achievement title")]
        public string title;
        public MissionType type;
        public string targetId;

        [Header("Behavior")]
        [Tooltip("if true, progress will be back to 0 after player lose/quit before achieve the target")]
        public bool isSingleRun;

        [Header("Tiers")]
        public List<AchievementTier> tiers;

        [Header("Runtime Status")]
        public int currentAmount;
        public int currentTierIndex;
        public bool allTiersCompleted;

        public void UpdateProgress(int amount, string incomingId)
        {
            if(allTiersCompleted) return;

            if (!string.IsNullOrEmpty(targetId) && incomingId != targetId) return;

            currentAmount += amount;
            CheckTierCompletion();
        }

        public void CheckTierCompletion()
        {
            AchievementTier activeTier = tiers[currentTierIndex];

            if (currentAmount >= activeTier.goalAmount)
            {
                #if UNITY_EDITOR
                Debug.Log($"Tier {activeTier.tierName} Completed for {title}!");
                #endif
                
                // Berikan reward di sini
                
                if (currentTierIndex < tiers.Count - 1)
                {
                    currentTierIndex++;
                }
                else
                {
                    allTiersCompleted = true;
                }
            }
        }

        public void ResetProgress()
        {
            currentAmount = 0;
        }
    }
}
