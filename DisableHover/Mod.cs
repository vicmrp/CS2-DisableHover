using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;

namespace DisableHover
{
    public class Mod : IMod
    {
        public static ILog log = LogManager
            .GetLogger($"{nameof(DisableHover)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
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
                        // Extra proof signals
#if VERBOSE
                log.Info("[VERBOSE] Extra noisy logging enabled");
#endif
#if DEBUG
                        log.Debug("[DEBUG] Debug-level message visible only in DEBUG/VERBOSE");
#endif
#if !DEBUG && !VERBOSE
                log.Info("[RELEASE] Minimal logging");
#endif
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}