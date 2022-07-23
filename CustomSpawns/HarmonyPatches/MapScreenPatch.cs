using CustomSpawns.Config;
using CustomSpawns.HarmonyPatches.Gameplay;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.InputSystem;
using SandBox.View.Map;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CustomSpawns.HarmonyPatches
{
    //The tick seems to get called whenever on the campaign map view, which makes sense. we can have some hotkeys here!
    [HarmonyPatch(typeof(MapScreen), "TaleWorlds.CampaignSystem.GameState.IMapStateHandler.Tick")]
    public class MapScreenPatch
    {

        private static bool _trueSight;

        static void Postfix()
        {
                ProcessTrueSightControls();
                ProcessAdditionalPartySpottingRange();
        }

        static void ProcessTrueSightControls()
        {
            var mapInput = MapScreen.Instance.Input;

            if (mapInput == null)
            {
                return;
            }

            if (mapInput.IsKeyReleased(InputKey.T) && mapInput.IsControlDown())
            {
                _trueSight = !_trueSight;
            }

            if (ConfigLoader.Instance.Config.IsDebugMode)
            {
                Campaign.Current.TrueSight = _trueSight;
            }
        }

        static void ProcessAdditionalPartySpottingRange()
        {
            if (!MBGameManager.Current.CheatMode)
            {
                return;
            }

            var mapInput = MapScreen.Instance.Input;

            if (mapInput == null)
            {
                return;
            }

            if (mapInput.IsKeyReleased(InputKey.R) && mapInput.IsControlDown())
            {
                PartySpottingRangePatch.AdditionalSpottingRange -= 2;
                InformationManager.DisplayMessage(
                    new InformationMessage($"Additional Spotting Range is Now: {PartySpottingRangePatch.AdditionalSpottingRange}", Colors.Green));
            }


            if (mapInput.IsKeyReleased(InputKey.Y) && mapInput.IsControlDown())
            {
                PartySpottingRangePatch.AdditionalSpottingRange += 2;
                InformationManager.DisplayMessage(
                    new InformationMessage($"Additional Spotting Range is Now: {PartySpottingRangePatch.AdditionalSpottingRange}", Colors.Green));
            }
        }

    }
}

//This is what the TW code looks like 1.5.9

//private void RegisterCheatHotkey(string id, InputKey hotkeyKey, HotKey.Modifiers modifiers, HotKey.Modifiers negativeModifiers = HotKey.Modifiers.None)
//{
//    base.RegisterHotKey(new HotKey(id, "Cheats", hotkeyKey, modifiers, negativeModifiers), true);
//}