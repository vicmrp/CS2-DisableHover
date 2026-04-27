using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Simulation;

namespace DisableHover
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(DisableHover)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public static Mod Instance { get; private set; }

        internal ModSettings Settings { get; set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            Instance = this;
            log.Info(nameof(OnLoad));
#if VERBOSE
                log.SetEffectiveness(Level.All);
                log.SetShowsErrorsInUI(true);
                log.Info("=== BUILD MODE: VERBOSE ===");
#elif DEBUG
                        log.SetEffectiveness(Level.Debug);
                        log.SetShowsErrorsInUI(true);
                        log.Info("=== BUILD MODE: DEBUG ===");
#else
                log.SetEffectiveness(Level.Info);
                log.Info("=== BUILD MODE: RELEASE ===");
#endif
#if VERBOSE
                log.Info("[VERBOSE] Extra noisy logging enabled");
#endif
#if DEBUG
                        log.Debug("[DEBUG] Debug-level message visible only in DEBUG/VERBOSE");
#endif
#if !DEBUG && !VERBOSE
                log.Info("[RELEASE] Minimal logging");
#endif

        // Register mod settings.
        Settings = new ModSettings(this);
        Settings.RegisterInOptionsUI();
        
        updateSystem.UpdateAt<TooltipSystem>(SystemUpdatePhase.UIUpdate);
        
        GameManager.instance.localizationManager.AddSource("en-US", new ModSettingsDefaultLocale(Settings));
        log.Info("Default locale loaded.");

        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}