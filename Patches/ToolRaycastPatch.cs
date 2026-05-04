using HarmonyLib;
using Game.Tools;
using Game;
using Game.Common;
using Unity.Entities;
using Game.Simulation;


namespace DisableHover.Patches
{
    [HarmonyPatch(typeof(ToolRaycastSystem), nameof(ToolRaycastSystem.GetRaycastResult))]
    public static class ToolRaycastPatch
    {
        static bool Prefix(ref bool __result, out RaycastResult result)
        {
            result = default;   // no hit data
            __result = false;   // method returns false

            return false;       // skip original
        }
    }
}