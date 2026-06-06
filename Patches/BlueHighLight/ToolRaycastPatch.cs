using Game.Buildings;
using Game.Common;
using Game.Input;
using Game.Tools;
using HarmonyLib;
using System;
using System.Reflection;
using Unity.Entities;

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

                Mod.log.Info(
                    $"[ClickDebug] Click detected. SelectedBuildingEntity={ToolRaycastPatch.SelectedBuildingEntity}"
                );
            }
        }
    }

    [HarmonyPatch(typeof(ToolRaycastSystem), nameof(ToolRaycastSystem.GetRaycastResult))]
    public static class ToolRaycastPatch
    {
        public static bool IsHoveringBuilding = false;
        public static Entity LastOwnerEntity = Entity.Null;
        public static Entity LastHitEntity = Entity.Null;
        public static Entity LastBuildingEntity = Entity.Null;

        public static Entity SelectedBuildingEntity = Entity.Null;

        static void Postfix(ref bool __result, ref RaycastResult result)
        {
            IsHoveringBuilding = false;
            LastOwnerEntity = Entity.Null;
            LastHitEntity = Entity.Null;
            LastBuildingEntity = Entity.Null;

            if (!Mod.DisableBlueHighlight || !__result)
                return;

            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

            LastOwnerEntity = result.m_Owner;
            LastHitEntity = result.m_Hit.m_HitEntity;

            Entity ownerBuilding = ResolveBuilding(em, LastOwnerEntity);
            Entity hitBuilding = ResolveBuilding(em, LastHitEntity);

            LastBuildingEntity = ownerBuilding != Entity.Null
                ? ownerBuilding
                : hitBuilding;

            IsHoveringBuilding = LastBuildingEntity != Entity.Null;

            if (!IsHoveringBuilding)
                return;

            if (SelectedBuildingEntity != Entity.Null &&
                LastBuildingEntity == SelectedBuildingEntity)
            {
                Mod.log.Info(
                    $"[HoverBlock] ALLOW selected building highlight. Building={LastBuildingEntity}"
                );

                return;
            }

            Mod.log.Info(
                $"[HoverBlock] BLOCK building hover. Owner={LastOwnerEntity}, Hit={LastHitEntity}, Building={LastBuildingEntity}"
            );

            __result = false;
            result = default;
        }

        public static Entity ResolveBuilding(EntityManager em, Entity entity)
        {
            if (entity == Entity.Null || !em.Exists(entity))
                return Entity.Null;

            if (em.HasComponent<Building>(entity))
                return entity;

            if (em.HasComponent<Owner>(entity))
            {
                Entity owner = em.GetComponentData<Owner>(entity).m_Owner;

                if (owner != Entity.Null &&
                    em.Exists(owner) &&
                    em.HasComponent<Building>(owner))
                {
                    return owner;
                }
            }

            return Entity.Null;
        }

        public static IProxyAction GetApplyAction(ToolBaseSystem tool)
        {
            try
            {
                PropertyInfo property =
                    AccessTools.Property(typeof(ToolBaseSystem), "applyAction");

                if (property == null)
                {
                    Mod.log.Info("[ClickDebug] Could not find ToolBaseSystem.applyAction");
                    return null;
                }

                return property.GetValue(tool) as IProxyAction;
            }
            catch (Exception ex)
            {
                Mod.log.Info($"[ClickDebug] GetApplyAction failed: {ex.Message}");
                return null;
            }
        }
    }
}