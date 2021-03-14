//using System;
//using System.Collections.Generic;
//using System.Xml.Serialization;

//namespace CustomSpawns.MCMv3
//{
//    public class CsSettings
//    {
//        public static bool SpawnSoundEnabled { get; set;  } = false;
//        public static bool IsDebugMode { get; set; } = false;
//        public static bool ShowAIDebug { get; set; } = false;
//        public static bool ShowDeathTrackDebug { get; set; } = false;
//        public static bool SpawnAtOneHideout { get; set; } = false;
//        public static bool IsAllSpawnMode { get; set; } = false;
//        public static bool ModifyPartySpeeds { get; set; } = false;
//        public static bool IsRemovalMode { get; set; } = false;
//        public static int UpdatePartyRedundantDataPerHour { get; set; } = 2;
//        public static int SameErrorShowUntil { get; set; } = 2;

//        private static CsSettings _instance = null;
        
//        private CsSettings()
//        {
//            var builder = new DefaultSettingsBuilder("CsSettings", $"CustomSpawn{Main.version}")
//                .SetFormat("xml")
//                .SetFolderName("CustomSpawn")
//                .SetSubFolder("")
//                .CreateGroup("Suond", groupBuilder => groupBuilder.SetGroupOrder(1)
//                    .AddBool("notification_sound", "Spawn Notification Sound",
//                        new ProxyRef<bool>(() => SpawnSoundEnabled, o => SpawnSoundEnabled = o), boolBuilder =>
//                            boolBuilder.SetHintText("Enable a notification sound when an important party spawn")))
//                .CreateGroup("Debug", groupBuilder => groupBuilder.SetGroupOrder(2)
//                    .AddBool("isDebugMode", "Enable Debug", new ProxyRef<bool>(() => IsDebugMode, o => IsDebugMode = o), boolBuilder => 
//                        boolBuilder.SetHintText("Enable the debug mode"))
//                    .AddBool("showAIDebug", "Show AI Debug", new ProxyRef<bool>(() => ShowAIDebug, o => ShowAIDebug = o), boolBuilder => 
//                        boolBuilder.SetHintText("Show AI debug messages"))
//                    .AddBool("showDeathTrackDebug", "Show Death Track Debug", new ProxyRef<bool>(() => ShowDeathTrackDebug, o => ShowDeathTrackDebug = o), boolBuilder => 
//                        boolBuilder.SetHintText("Show messages related to party destruction"))
//                    .AddBool("isAllSpawnMode", "Is All Spawn Mode", new ProxyRef<bool>(() => IsAllSpawnMode, o => IsAllSpawnMode = o), boolBuilder => 
//                        boolBuilder.SetHintText("Enable the spawning of all party in one night"))
//                    .AddBool("spawnAtOneHideout", "Spawn At One Hideout", new ProxyRef<bool>(() => SpawnAtOneHideout, o => SpawnAtOneHideout = o), boolBuilder => 
//                        boolBuilder.SetHintText("Spawn player at one hideout"))
//                    .AddBool("modifyPartySpeeds", "Modify Party Speeds", new ProxyRef<bool>(() => ModifyPartySpeeds, o => ModifyPartySpeeds = o), boolBuilder => 
//                        boolBuilder.SetHintText("Modify the party speeds"))
//                    .AddBool("isRemovalMode", "Is Removal Mode", new ProxyRef<bool>(() => IsRemovalMode, o => IsRemovalMode = o), boolBuilder => 
//                        boolBuilder.SetHintText("Enable the removal mode"))
//                    .AddInteger("updatePartyRedundantDataPerHour", "Update Party Redundant Data Per Hour", 1, 5, new ProxyRef<int>(() => UpdatePartyRedundantDataPerHour, o => UpdatePartyRedundantDataPerHour = o), boolBuilder => 
//                        boolBuilder.SetHintText("Do Something"))
//                    .AddInteger("sameErrorShowUntil", "Same Error Show Until", 1, 5, new ProxyRef<int>(() => SameErrorShowUntil, o => SameErrorShowUntil = o), boolBuilder => 
//                        boolBuilder.SetHintText("The same error will be shown in the log if it exceeds this number in one play time"))
//                );
            
//            var globalSettings = builder.BuildAsGlobal();
//            globalSettings.Register();
//        }

//        public static CsSettings GetInstance()
//        {
//            if (_instance == null)
//            {
//                _instance = new CsSettings();
//            }
//            return _instance;
//        }
        /*

        public override string Id { get; } = "CsSettings";
        public override string DisplayName { get; } = $"CustomSpawn{Main.version}";
        public override string FolderName { get;  } = "Custom Spawn";
        public override string Format { get; } = "xml";
        
        [SettingPropertyBool("Enable Spawn Sound", RequireRestart = false,
            HintText = "Enable a notification sound when important party spawn")]
        [SettingPropertyGroup("Sound")]
        public bool SpawnSoundEnabled
        {
            get => this._spawnSoundEnabled;
            set
            {
                if (_spawnSoundEnabled != value)
                {
                    _spawnSoundEnabled = value;
                    OnPropertyChanged();
                }
            }
        }*/
        
        /**
         * Define the default settings to use wen restore
         */
        /*public override IDictionary<string, Func<BaseSettings>> GetAvailablePresets()
        {
            //var basePresets = base.GetAvailablePresets(); // include the 'Default' preset that MCM provides
            var basePresets = new Dictionary<string, Func<BaseSettings>>();
            basePresets.Add("Default", () => new CsSettings()
            {
                SpawnSoundEnabled = false,
            });
            return basePresets;
        }*/
//    }
//}