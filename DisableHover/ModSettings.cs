using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace vezit.DisableHover
{
    // using Systems;
    [FileLocation("ModsSettings/" + nameof(DisableHover) + "/" + nameof(DisableHover))]
    [SettingsUIShowGroupName(MainGroup)]
    public sealed class ModSettings : ModSetting
    {
        public const string MainGroup = "My First Group";

        [SettingsUISection(MainGroup)]
        [SettingsUISetter(typeof(ModSettings), nameof(ToggleDisableUIToolTips))]
        public bool DisableUIToolTips { get; set; }

        public ModSettings(IMod mod) : base(mod)
        {
            this.SetDefaults();
        }

        public override void SetDefaults()
        {
            this.DisableUIToolTips = false;
        }

        public void ApplySystemStates()
        {
            
        }

        private void ToggleDisableUIToolTips(bool disabled)
        {
            Mod.log.Info($"button clicked! Disabled state: {disabled}");
            TooltipSystem.SetTooltipsEnabled(disabled);
        }
    }
}