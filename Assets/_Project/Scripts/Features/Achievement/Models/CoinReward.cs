using UnityEngine;

namespace ColorOrCrash.Features.Achievement.Models
{
    [CreateAssetMenu(fileName = "CoinReward", menuName = "Scriptable Objects/Rewards/CoinReward")]
    public class CoinReward : RewardBase
    {
        public enum CoinType { Red, Green, Blue }
        public CoinType type;
        public int amount;

        public override void GiveReward()
        {
            var data = NocturneThree.ServiceLocator.ServiceLocator.Get<SaveManager>().Data;
            
            switch (type)
            {
                case CoinType.Red: data.redCoins += amount; break;
                case CoinType.Green: data.greenCoins += amount; break;
                case CoinType.Blue: data.blueCoins += amount; break;
            }

            Debug.Log($"Rewarded {amount} {type} Coins");
        }
    }
}
