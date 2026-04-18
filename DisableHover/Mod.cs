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

            updateSystem.UpdateAt<TooltipSystem>(SystemUpdatePhase.UIUpdate);

            log.Info("TooltipSystem registered");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}