using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Models
{
    [CreateAssetMenu(fileName = "UnlockableReward", menuName = "Scriptable Objects/Rewards/UnlockableReward")]
    public class UnlockableReward : RewardBase
    {
        public enum UnlockType { Arena, Character, Theme }
        public UnlockType category;
        public string unlockID;

        public override void GiveReward()
        {
            var data = NocturneThree.ServiceLocator.ServiceLocator.Get<SaveManager>().Data;

            if (!data.unlockedIds.Contains(unlockID))
            {
                data.unlockedIds.Add(unlockID);
                Debug.Log($"Unlocked {category}: {unlockID}");
            }
        }
    }
}
