using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.InputSystem;
using SandBox.View.Map;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CustomSpawns.HarmonyPatches
{
    //The tick seems to get called whenever on the campaign map view, which makes sense. we can have some hotkeys here!
    [HarmonyPatch(typeof(MapScreen), "TaleWorlds.CampaignSystem.IMapStateHandler.Tick")]
    public class MapScrenPatch
    {

        static bool TrueSight = false;

        static void Postfix()
        {

            try
            {
                ProcessTrueSightControls();
                ProcessAdditionalPartySpottingRange();
            }
            catch
            {

            }
        }

        static void ProcessTrueSightControls()
        {
            var MapInput = MapScreen.Instance.Input;

            if (MapInput == null)
            {
                return;
            }

            if (MapInput.IsKeyReleased(InputKey.T) && MapInput.IsControlDown())
            {
                TrueSight = !TrueSight;
            }

            if (ConfigLoader.Instance.Config.IsDebugMode)
            {
                Campaign.Current.TrueSight = TrueSight;
            }
        }

        static void ProcessAdditionalPartySpottingRange()
        {
            if (!MBGameManager.Current.CheatMode)
            {
                return;
            }

            var MapInput = MapScreen.Instance.Input;

            if (MapInput == null)
            {
                return;
            }

            if (MapInput.IsKeyReleased(InputKey.R) && MapInput.IsControlDown())
            {
                Gameplay.PartySpottingRangePatch.AdditionalSpottingRange -= 2;
                InformationManager.DisplayMessage(
                    new InformationMessage($"Additional Spotting Range is Now: {Gameplay.PartySpottingRangePatch.AdditionalSpottingRange}", Colors.Green));
            }


            if (MapInput.IsKeyReleased(InputKey.Y) && MapInput.IsControlDown())
            {
                Gameplay.PartySpottingRangePatch.AdditionalSpottingRange += 2;
                InformationManager.DisplayMessage(
                    new InformationMessage($"Additional Spotting Range is Now: {Gameplay.PartySpottingRangePatch.AdditionalSpottingRange}", Colors.Green));
            }
        }

    }
}

//This is what the TW code looks like 1.5.9

//private void RegisterCheatHotkey(string id, InputKey hotkeyKey, HotKey.Modifiers modifiers, HotKey.Modifiers negativeModifiers = HotKey.Modifiers.None)
//{
//    base.RegisterHotKey(new HotKey(id, "Cheats", hotkeyKey, modifiers, negativeModifiers), true);
//}