using Colossal;
using System.Collections.Generic;

namespace DisableHover.Settings
{
    public class ModSettingsDefaultLocale : IDictionarySource
    {
        private readonly Dictionary<string, string> _entries;

        public ModSettingsDefaultLocale(ModSettings settings)
        {
            _entries = LoadSettingsLocale(settings);
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return _entries;
        }

        public void Unload() { }

        private static Dictionary<string, string> LoadSettingsLocale(ModSettings settings)
        {
            return new Dictionary<string, string>
            {
                { settings.GetSettingsLocaleID(), "DisableHover" },
                { settings.GetOptionGroupLocaleID(ModSettings.MainGroup), "DisableHover" },

                { settings.GetOptionLabelLocaleID(nameof(ModSettings.DisableUIToolTips)), "Disable Tooltips" },
                { settings.GetOptionDescLocaleID(nameof(ModSettings.DisableUIToolTips)), "Disables distracting UI tooltips without changing normal selection or tool input." },

                { settings.GetOptionLabelLocaleID(nameof(ModSettings.DisableBlueHighLightOnBuildings)), "Disable Blue Highlights" },
                { settings.GetOptionDescLocaleID(nameof(ModSettings.DisableBlueHighLightOnBuildings)), "Hides blue hover/highlight rendering without intercepting selection or raycasts." },
            };
        }
    }
}
