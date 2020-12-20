using System;

namespace CustomSpawns.RewardSystem.Models
{
    public class Reward
    {
        public RewardType Type { get; set; }
        public string ItemId { get; set; }
        public Nullable<int> RenownInfluenceMoneyAmount { get; set; }
    }
}