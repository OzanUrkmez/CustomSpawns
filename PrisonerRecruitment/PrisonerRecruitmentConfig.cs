using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.PrisonerRecruitment
{
    [Serializable]
    class PrisonerRecruitmentConfig
    {

        public bool Enabled { get; set; }
        public float BaseRecruitChance { get; set; }
        public float BaseDevalueChance { get; set; }
        public float PrisonerLevelReverseModifierPerLevel { get; set; }
        public float PrisonerLevelDevalueModifierPerLevel { get; set; }
        public float MercifulTraitModifier { get; set; }
        public float DifferentCultureReverseModifier { get; set; }
        public float PrisonerPartyPercentageCap { get; set; }
        public float CapReverseFinalCoefficientPerCap { get; set; }
        public float GarrisonFinalCoefficient { get; set; }
        public float FinalMinimumChance { get; set; }
    }
}
