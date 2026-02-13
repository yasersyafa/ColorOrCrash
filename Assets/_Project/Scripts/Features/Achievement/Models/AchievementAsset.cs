using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Models
{
    [Serializable]
    public struct AchievementTier
    {
        [Header("Display")]
        [Tooltip("Nama tingkatan, contoh: Bronze, Silver, Gold, atau Tier 1")]
        public string tierName;

        [Header("Requirements")]
        [Tooltip("Jumlah target yang harus dicapai untuk menyelesaikan tier ini")]
        public int goalAmount;

        [Header("Rewards")]
        [Tooltip("Daftar hadiah yang didapat. Bisa kombinasi koin, skin, arena, dll.")]
        public List<RewardBase> rewards;
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
        [Header("Identity")]
        public string achievementId; // ID Unik untuk Save System
        public string title;        // Nama yang muncul di UI
        public MissionType type;
        public string targetId;     // Spesifik warna atau item ID

        [Header("Behavior")]
        [Tooltip("Jika true, progress akan kembali ke 0 jika kalah/quit.")]
        public bool isSingleRun;

        [Header("Tiers")]
        public List<AchievementTier> tiers;

        [Header("Runtime Status (Don't Edit)")]
        public int currentAmount;
        public int currentTierIndex;
        public bool allTiersCompleted;

        /// <summary>
        /// Fungsi utama untuk menambah progress
        /// </summary>
        public void UpdateProgress(int amount, string incomingId)
        {
            if (allTiersCompleted) return;

            // Filter berdasarkan ID (misal: hanya obstacle warna "Green")
            if (!string.IsNullOrEmpty(targetId) && incomingId != targetId) return;

            // Logic khusus untuk ReachScore (kita ambil nilai tertinggi, bukan akumulasi)
            if (type == MissionType.ScoreReach)
            {
                if (amount > currentAmount) currentAmount = amount;
            }
            else
            {
                currentAmount += amount;
            }

            CheckTierCompletion();
        }

        private void CheckTierCompletion()
        {
            if (allTiersCompleted || currentTierIndex >= tiers.Count) return;

            AchievementTier activeTier = tiers[currentTierIndex];

            if (currentAmount >= activeTier.goalAmount)
            {
                // 1. Eksekusi Hadiah
                if (activeTier.rewards != null)
                {
                    foreach (var reward in activeTier.rewards)
                    {
                        reward.GiveReward();
                        
                        // 2. Kirim notifikasi ke UI Toast
                        // NocturneThree.ServiceLocator.ServiceLocator.Get<NotificationManager>()
                        //    .ShowNotification("Tier Unlocked!", $"{title}: {activeTier.tierName}");
                    }
                }

                // 3. Naikkan ke Tier selanjutnya
                if (currentTierIndex < tiers.Count - 1)
                {
                    currentTierIndex++;
                    // Rekursif cek: jika progress melompat jauh (misal lsg dapet 100 jump)
                    CheckTierCompletion(); 
                }
                else
                {
                    allTiersCompleted = true;
                }
            }
        }

        /// <summary>
        /// Digunakan oleh MissionManager saat GameOver jika isSingleRun = true
        /// </summary>
        public void ResetProgress()
        {
            currentAmount = 0;
            // Kita tidak meriset currentTierIndex karena tier yang sudah unlock tetap unlock
        }
    }
}
