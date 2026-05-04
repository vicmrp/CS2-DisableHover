using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace DisableHover
{
    // using Systems;
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
            Mod.log.Info($"DisableUIToolTips button clicked! Disabled state: {disabled}");
            TooltipSystem.SetTooltipsEnabled(disabled);
        }

        private void ToggleDisableBlueHighLightOnBuildings(bool disabled)
        {
            Mod.log.Info($"DisableBlueHighLightOnBuildings: {disabled}");

            DisableBlueHighLightOnBuildings = disabled;
            Mod.DisableBlueHighlight = disabled;

            
        }




    }
}