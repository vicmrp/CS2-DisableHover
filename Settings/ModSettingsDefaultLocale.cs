using Colossal;
using System.Collections.Generic;

namespace DisableHover.Settings
{
    public class ModSettingsDefaultLocale : IDictionarySource
    {
        private Dictionary<string, string> Entries { get; set; }

        public ModSettingsDefaultLocale(ModSettings settings)
        {
            Entries = LoadSettingsLocale(settings);
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => Entries;

        public void Unload()
        {
            
        }

        private static Dictionary<string, string> LoadSettingsLocale(ModSettings settings)
        {
            return new Dictionary<string, string>
            {
                { settings.GetSettingsLocaleID(), "DisableHover" },
                { settings.GetOptionGroupLocaleID(ModSettings.MainGroup), "DisableHover" },

                { settings.GetOptionLabelLocaleID(nameof(ModSettings.DisableUIToolTips)), "Disable Tooltips" },
                { settings.GetOptionDescLocaleID(nameof(ModSettings.DisableUIToolTips)), "Disables annoying and distracting UI tooltips." },
                
                { settings.GetOptionLabelLocaleID(nameof(ModSettings.DisableBlueHighLightOnBuildings)), "Disable Blue Highligths" },
                { settings.GetOptionDescLocaleID(nameof(ModSettings.DisableBlueHighLightOnBuildings)), "When this is enabled, all items that normally are marked blue, will no longer be marked anything." },
            };
        }
    }
}