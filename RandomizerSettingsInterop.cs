using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using RandoSettingsManager.SettingsManagement.Versioning.Comparators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreEndings
{
    //provides integration with RandomizerSettingsManager
    internal class RandomizerSettingsInterop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new SettingsProxy());
        }
    }

    internal class SettingsProxy : RandoSettingsProxy<SaveData, string>
    {
        public override string ModKey => DreamerShardRando.ModName;

        public override VersioningPolicy<string> VersioningPolicy { get; }

        public SettingsProxy()
        {
            VersioningPolicy = new StrictModVersioningPolicy(DreamerShardRando.Instance);
        }

        public override void ReceiveSettings(SaveData settings)
        {
            DreamerShardRando.saveSettings = settings; 
            Menu.Instance.UpdateSettings();
        }

        public override bool TryProvideSettings(out SaveData settings)
        {
            settings = DreamerShardRando.saveSettings;
            return true;
        }
    }
}
