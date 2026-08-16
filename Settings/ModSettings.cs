using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace DisableHover.Settings
{
    // Keep a single settings group matching the original working DisableHover layout.
    [FileLocation("ModsSettings/" + nameof(DisableHover) + "/" + nameof(DisableHover))]
    [SettingsUIShowGroupName(MainGroup)]
    public sealed class ModSettings : ModSetting
    {
        public const string MainGroup = "Main Group";

        [SettingsUISection(MainGroup)]
        [SettingsUISetter(typeof(ModSettings), nameof(ToggleDisableUIToolTips))]
        public bool DisableUIToolTips { get; set; }

        [SettingsUISection(MainGroup)]
        [SettingsUISetter(typeof(ModSettings), nameof(ToggleDisableBlueHighLightOnBuildings))]
        public bool DisableBlueHighLightOnBuildings { get; set; }

        public ModSettings(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        public override void SetDefaults()
        {
            DisableUIToolTips = false;
            DisableBlueHighLightOnBuildings = false;
        }

        private void ToggleDisableUIToolTips(bool disabled)
        {
            Mod.SetTooltipsDisabled(disabled);
        }

        private void ToggleDisableBlueHighLightOnBuildings(bool disabled)
        {
            Mod.SetBlueHighlightsDisabled(disabled);
        }
    }
}
