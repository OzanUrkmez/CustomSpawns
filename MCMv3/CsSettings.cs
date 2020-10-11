using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.FluentBuilder.Implementation;
using MCM.Abstractions.Ref;
using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.Global;

namespace CustomSpawns.MCMv3
{
    public class CsSettings
    {
        public static bool SpawnSoundEnabled { get; set;  } = false;

        private static CsSettings _instance = null;
        
        private CsSettings()
        {
            var builder = new DefaultSettingsBuilder("CsSettings", $"CustomSpawn{Main.version}")
                .SetFormat("xml")
                .SetFolderName("CustomSpawn")
                .SetSubFolder("")
                .CreateGroup("Suond", groupBuilder => groupBuilder
                    .AddBool("notification_sound", "Spawn Notification Sound",
                        new ProxyRef<bool>(() => SpawnSoundEnabled, o => SpawnSoundEnabled = o), boolBuilder =>
                            boolBuilder.SetHintText("Enable a notification sound when an important party spawn")));
            
            var globalSettings = builder.BuildAsGlobal();
            globalSettings.Register();
        }

        public static CsSettings GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CsSettings();
            }
            return _instance;
        }
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
    }
}