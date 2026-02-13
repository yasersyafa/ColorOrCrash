using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorOrCrash.Features.SaveSystem.Models
{
    [Serializable]
    public class AchievementEntry
    {
        public string title;
        public int currentTierIndex;
        public int currentAmount;
        public bool allTiersCompleted;
    }

    [Serializable]
    public class PlayerData
    {
        [Header("Currency")]
        public int redCoins;
        public int greenCoins;
        public int blueCoins;

        [Header("Stats")]
        public int highScore;
        public int totalJumps;

        [Header("Unlockables")]
        [Tooltip("Menyimpan ID unik untuk Arena, Karakter, dan Theme")]
        public List<string> unlockedIds = new();

        [Header("Achievements")]
        [Tooltip("Daftar progress achievement yang sedang berjalan")]
        public List<AchievementEntry> achievementProgress = new List<AchievementEntry>();

        /// <summary>
        /// Helper untuk mencari data achievement tertentu di dalam list
        /// </summary>
        public AchievementEntry GetAchievement(string title)
        {
            return achievementProgress.Find(a => a.title == title);
        }
        
        /// <summary>
        /// Helper untuk mengecek apakah sebuah ID (Skin/Arena) sudah di-unlock
        /// </summary>
        public bool IsUnlocked(string id) => unlockedIds.Contains(id);
    }
}
