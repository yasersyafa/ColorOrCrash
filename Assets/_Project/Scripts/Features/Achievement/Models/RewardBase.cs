using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Models
{
    public abstract class RewardBase : ScriptableObject
    {
        public string rewardName;
        public Sprite rewardIcon;

        public abstract void GiveReward();
    }
}
