using System.Collections.Generic;

namespace CustomSpawns.RewardSystem.Models
{
    public class PartyReward
    {
        public string PartyId { get; set; }
        public List<Reward> Rewards { get; set; }
        
        public PartyReward(string partyId, List<Reward> rewards)
        {
            this.Rewards = rewards;
            this.PartyId = partyId;
        }
    }
}