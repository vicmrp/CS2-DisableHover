using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using DisableHover.Settings;
using DisableHover.Systems.Tooltips;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace DisableHover
{
    public class Mod : IMod
    {
        public static ILog log = LogManager
            .GetLogger($"{nameof(DisableHover)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public static bool DisableBlueHighlight { get; private set; }
        public static Mod Instance { get; private set; }
        internal ModSettings Settings { get; set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            Instance = this;
            log.Info("DisableHover loading (native projected-hover release candidate - DH highlights, DT tooltips)");

#if VERBOSE
            log.SetEffectiveness(Level.All);
            log.SetShowsErrorsInUI(true);
#elif DEBUG
            log.SetEffectiveness(Level.Debug);
            log.SetShowsErrorsInUI(true);
#else
            log.SetEffectiveness(Level.Info);
#endif

            // Use the same settings lifecycle/order as the original working mod.
            Settings = new ModSettings(this);
            Settings.RegisterInOptionsUI();

            AssetDatabase.global.LoadSettings(
                nameof(DisableHover),
                Settings,
                new ModSettings(this)
            );

            DisableBlueHighlight = Settings.DisableBlueHighLightOnBuildings;
            log.Info($"Settings loaded. TooltipsDisabled={Settings.DisableUIToolTips}, BlueHighlightsDisabled={Settings.DisableBlueHighLightOnBuildings}");

            updateSystem.UpdateAt<TooltipSystem>(SystemUpdatePhase.UIUpdate);

            GameManager.instance.localizationManager.AddSource(
                "en-US",
                new ModSettingsDefaultLocale(Settings)
            );
            log.Info("Default locale loaded");

            updateSystem.UpdateAt<DisableHoverOutlineSystem>(SystemUpdatePhase.Rendering);

            log.Info("DisableHover loaded successfully");
        }

        public static void SetTooltipsDisabled(bool disabled)
        {
            Mod mod = Instance;
            if (mod?.Settings == null)
            {
                log.Warn("SetTooltipsDisabled ignored because settings are not ready");
                return;
            }

            mod.Settings.DisableUIToolTips = disabled;
            mod.Settings.ApplyAndSave();
            TooltipSystem.UpdateTooltipBinding(disabled);
            log.Info($"Disable Tooltips = {disabled}");
        }

        public static void SetBlueHighlightsDisabled(bool disabled)
        {
            Mod mod = Instance;
            if (mod?.Settings == null)
            {
                log.Warn("SetBlueHighlightsDisabled ignored because settings are not ready");
                return;
            }

            mod.Settings.DisableBlueHighLightOnBuildings = disabled;
            mod.Settings.ApplyAndSave();
            DisableBlueHighlight = disabled;
            TooltipSystem.UpdateBlueHighlightBinding(disabled);
            log.Info($"Disable Blue Highlights = {disabled}");
        }

        public static void ToggleTooltips()
        {
            Mod mod = Instance;
            if (mod?.Settings == null)
            {
                log.Warn("Toolbar DT ignored because settings are not ready");
                return;
            }

            bool disabled = !mod.Settings.DisableUIToolTips;
            SetTooltipsDisabled(disabled);
            log.Info($"Toolbar DT clicked. Disable Tooltips = {disabled}");
        }

        public static void ToggleBlueHighlights()
        {
            Mod mod = Instance;
            if (mod?.Settings == null)
            {
                log.Warn("Toolbar DH ignored because settings are not ready");
                return;
            }

            bool disabled = !mod.Settings.DisableBlueHighLightOnBuildings;
            SetBlueHighlightsDisabled(disabled);
            log.Info($"Toolbar DH clicked. Disable Blue Highlights = {disabled}");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));

            // Match the original mod's disposal behavior. The game owns the
            // registered settings UI lifetime while the mod is active/unloaded.
            Instance = null;
        }
    }
}
