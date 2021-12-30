using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.AiBehaviors;

/* Patch might be useful as the bug might not be resolved. Disabled for now */
namespace CustomSpawns.HarmonyPatches
{
    // [HarmonyPatch(typeof(AiBanditPatrollingBehavior), "AiHourlyTick")]
    public class PreventNullPointerExceptionAiBanditPatrollingBehavior
    {
        private static readonly MethodInfo GetBanditPartyComponentMethodInfo = typeof(MobileParty).GetMethod("get_BanditPartyComponent", AccessTools.all);
        
        private static readonly MethodInfo SetHomeHideoutMethodInfo = typeof(BanditPartyComponent).GetMethod("SetHomeHideout", AccessTools.all);
            
        private static readonly FieldInfo SettlementHideoutFieldInfo = typeof(Settlement).GetField("Hideout", AccessTools.all);
        
        private static readonly MethodInfo SetBanditPartyComponentHideoutMethodInfo = typeof(PreventNullPointerExceptionAiBanditPatrollingBehavior).GetMethod("SetBanditPartyComponentHideout", AccessTools.all);

        public PreventNullPointerExceptionAiBanditPatrollingBehavior()
        {
            if (GetBanditPartyComponentMethodInfo == null)
            {
                throw new TechnicalException("Cannot get method get_BanditPartyComponent from MobileParty class via reflection");
            }
            
            if (SetHomeHideoutMethodInfo == null)
            {
                throw new TechnicalException("Cannot get method SetHomeHideout from BanditPartyComponent class via reflection");
            }
            
            if (SettlementHideoutFieldInfo == null)
            {
                throw new TechnicalException("Cannot get field Hideout from Settlement class via reflection");
            }
        }
        
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var codes = instructions.ToList();
            for (var index = 0; index < codes.Count; index++) {
                if (IsBanditPartyComponentSetHomeHideoutCalled(codes, index))
                {
                    /* SOLUTION 1 */
                    codes[index] = codes[index + 3];
                    codes[index + 1] = new CodeInstruction(OpCodes.Ldarg_1);
                    codes[index + 2] = new CodeInstruction(OpCodes.Call, SetBanditPartyComponentHideoutMethodInfo);
                    codes[index + 3] = new CodeInstruction(OpCodes.Nop);
                    codes[index + 4] = new CodeInstruction(OpCodes.Nop);
                    codes[index + 5] = new CodeInstruction(OpCodes.Nop);

                    /* SOLUTION 2 */
                    // CodeInstruction settlement = codes[index + 3];
                    // Label nonNullSettlementBranch = generator.DefineLabel();
                    // var handleNullSettlement = new List<CodeInstruction>(3)
                    // {
                    //     settlement,
                    //     new CodeInstruction(OpCodes.Brtrue_S, nonNullSettlementBranch),
                    //     new CodeInstruction(OpCodes.Ldarg_1),
                    //     new CodeInstruction(OpCodes.Callvirt, GetBanditPartyComponentMethodInfo),
                    //     new CodeInstruction(OpCodes.Ldnull),
                    //     new CodeInstruction(OpCodes.Callvirt, SetHomeHideoutMethodInfo),
                    //     new CodeInstruction(OpCodes.Br_S, codes[index - 15].operand),
                    //     new CodeInstruction(OpCodes.Ldarg_1).WithLabels(nonNullSettlementBranch),
                    //     new CodeInstruction(OpCodes.Callvirt, GetBanditPartyComponentMethodInfo),
                    //     settlement,
                    //     new CodeInstruction(OpCodes.Ldfld, SettlementHideoutFieldInfo),
                    //     new CodeInstruction(OpCodes.Callvirt, SetHomeHideoutMethodInfo)
                    // };
                    //
                    // codes.RemoveRange(index, 6);
                    // codes.InsertRange(index, handleNullSettlement);
                    
                    return codes.AsEnumerable();
                }
            }
            return codes.AsEnumerable();
        }

        private static bool IsBanditPartyComponentSetHomeHideoutCalled(List<CodeInstruction> instructions, int index)
        {
            return index >= 15 && index + 7 <= instructions.Count
                                 && instructions[index].IsLdloc()
                                 && instructions[index + 1].opcode == OpCodes.Ldfld
                                 && instructions[index + 2].Calls(GetBanditPartyComponentMethodInfo)
                                 && instructions[index + 3].opcode == OpCodes.Ldloc_S
                                 && instructions[index + 4].LoadsField(SettlementHideoutFieldInfo)
                                 && instructions[index + 5].Calls(SetHomeHideoutMethodInfo)
                                 && instructions[index + 6].opcode == OpCodes.Ldloca_S
                                 && instructions[index - 15].operand is Label;
        }
        
        private static void SetBanditPartyComponentHideout(Settlement settlement, MobileParty mobileParty)
        {
            if (settlement == null)
            {
                Settlement settlement1 = SettlementHelper.FindNearestHideout((Settlement x) => x.Culture == mobileParty.MapFaction.Culture && x.Hideout.IsInfested, null) ?? SettlementHelper.FindNearestHideout((Settlement x) => x.Culture == mobileParty.MapFaction.Culture, null);
                 mobileParty.BanditPartyComponent.SetHomeHideout(null);   
            }
            else
            {
                mobileParty.BanditPartyComponent.SetHomeHideout(settlement.Hideout);
            }
        }
    }
}