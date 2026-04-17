using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using HarmonyLib;
using System.Reflection;

namespace DisableHover
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(DisableHover)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            // if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            //     log.Info($"Current mod asset at {asset.path}");


var harmony = new Harmony("disable.hover.tooltip");
harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
