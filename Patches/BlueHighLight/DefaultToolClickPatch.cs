using Game.Input;
using Game.Tools;
using HarmonyLib;


namespace DisableHover.Patches.BlueHighLight
{
    [HarmonyPatch(typeof(DefaultToolSystem), "OnUpdate")]
    public static class DefaultToolClickPatch
    {
        static void Prefix(DefaultToolSystem __instance)
        {
            IProxyAction applyAction = ToolRaycastPatch.GetApplyAction(__instance);

            if (applyAction == null)
                return;

            if (applyAction.WasPressedThisFrame())
            {
                ToolRaycastPatch.SelectedBuildingEntity =
                    ToolRaycastPatch.LastBuildingEntity;
#if DEBUG
                Mod.log.Info(
                    $"[ClickDebug] Click detected. SelectedBuildingEntity={ToolRaycastPatch.SelectedBuildingEntity}"
                );
#endif
            }
        }
    }
}