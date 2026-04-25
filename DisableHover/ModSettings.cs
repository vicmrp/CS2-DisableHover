using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;


namespace DisableHover
{
    [FileLocation("ModsSettings/DisableHover/DisableHover")]
    [SettingsUIShowGroupName(MainGroup)]
    public sealed class ModSettings : ModSetting
    {
        public const string MainGroup = "General";

        public ModSettings(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        public override void SetDefaults()
        {
        }

        // 👇 This creates a button in the UI
        [SettingsUIButtonGroup(MainGroup)]
        [SettingsUIButton]
        public bool DummyButton
        {
            set => OnDummyPressed(value);
        }

        private void OnDummyPressed(bool pressed)
        {
            if (!pressed) return;

            Mod.log.Info("=== DUMMY BUTTON CLICKED ===");
        }
    }
}