using System;

namespace CustomSpawns.RewardSystem.Models
{
    public class Reward
    {
        public RewardType Type { get; set; }
        public string ItemId { get; set; }
        public int? RenownInfluenceMoneyAmount { get; set; }
        public decimal? Chance { get; set; }
    }
}