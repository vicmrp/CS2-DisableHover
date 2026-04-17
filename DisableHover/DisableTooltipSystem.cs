using HarmonyLib;
using UnityEngine;

namespace DisableHover
{
    // [HarmonyPatch(typeof(GUI), "set_tooltip")]
    // public static class DisableGUITooltip
    // {
    //     static bool Prefix(ref string value)
    //     {
    //         value = null; // wipe tooltip
    //         return true;
    //     }
    // }
}