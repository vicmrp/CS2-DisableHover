using Colossal;
using System.Collections.Generic;

namespace vezit.DisableHover
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
                { settings.GetSettingsLocaleID(), "Victor's Toolbox" },
                { settings.GetOptionGroupLocaleID(ModSettings.MainGroup), "My First Group" },
                { settings.GetOptionLabelLocaleID(nameof(ModSettings.DisableUIToolTips)), "Disable Tooltips" },
                { settings.GetOptionDescLocaleID(nameof(ModSettings.DisableUIToolTips)), "Disables annoying and distacting UI tooltips." },
            };
        }
    }
}