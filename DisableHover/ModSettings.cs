using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace vezit.DisableHover
{
    [FileLocation("ModsSettings/DisableHover/DisableHover")]
    [SettingsUIShowGroupName(MainGroup)]
    public sealed class ModSettings : ModSetting
    {
        public const string MainGroup = "My First Group";

        [SettingsUISection(MainGroup)]
        [SettingsUISetter(typeof(ModSettings), nameof(ToggleDisableUIToolTips))]
        public bool DisableUIToolTips { get; set; }

        public ModSettings(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        public override void SetDefaults()
        {
        }

        private void ToggleDisableUIToolTips(bool disabled)
        {
            Mod.log.Info($"button clicked! Disabled state: {disabled}");
            TooltipSystem.SetTooltipsEnabled(disabled);
        }
    }
}