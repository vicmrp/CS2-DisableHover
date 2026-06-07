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
            // Reset hover state every raycast.
            IsHoveringBuilding = false;
            LastOwnerEntity = Entity.Null;
            LastHitEntity = Entity.Null;
            LastBuildingEntity = Entity.Null;

            // If the mod setting is off, or the raycast failed, do nothing.
            if (!Mod.DisableBlueHighlight || !__result)
                return;

            // Let bulldozer work normally.
            if (IsBulldozerActive())
                return;

            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

            // The owner is often the main object.
            LastOwnerEntity = result.m_Owner;

            // The hit entity is the exact thing under the mouse.
            LastHitEntity = result.m_Hit.m_HitEntity;

            // Try to resolve both entities into a real Building entity.
            Entity ownerBuilding = ResolveBuilding(em, LastOwnerEntity);
            Entity hitBuilding = ResolveBuilding(em, LastHitEntity);

            // Prefer the owner building, otherwise use the hit building.
            LastBuildingEntity = ownerBuilding != Entity.Null
                ? ownerBuilding
                : hitBuilding;

            // If we found a building, the mouse is hovering a building.
            IsHoveringBuilding = LastBuildingEntity != Entity.Null;

            // Not a building? Do not block the raycast.
            if (!IsHoveringBuilding)
                return;

            // If this building was clicked/selected, allow its highlight.
            if (SelectedBuildingEntity != Entity.Null &&
                LastBuildingEntity == SelectedBuildingEntity)
            {
#if VERBOSE
                Mod.log.Info(
                    $"[HoverBlock] ALLOW selected building highlight. Building={LastBuildingEntity}"
                );
#endif
                return;
            }

#if VERBOSE
            Mod.log.Info(
                $"[HoverBlock] BLOCK building hover. Owner={LastOwnerEntity}, Hit={LastHitEntity}, Building={LastBuildingEntity}"
            );
#endif
            
            // Otherwise block the raycast result.
            // This prevents the blue hover highlight.
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
#if DEBUG
                    Mod.log.Info("[ClickDebug] Could not find ToolBaseSystem.applyAction");
#endif
                    return null;
                }

                return property.GetValue(tool) as IProxyAction;
            }
            catch (Exception ex)
            {
#if DEBUG
                Mod.log.Info($"[ClickDebug] GetApplyAction failed: {ex.Message}");
#endif
                return null;
            }
        }

        public static bool IsBulldozerActive()
        {
            try
            {
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                    return false;

                ToolSystem toolSystem = world.GetExistingSystemManaged<ToolSystem>();
                if (toolSystem == null)
                    return false;

                PropertyInfo activeToolProperty =
                    AccessTools.Property(typeof(ToolSystem), "activeTool");

                if (activeToolProperty == null)
                    return false;

                object activeTool = activeToolProperty.GetValue(toolSystem);

                if (activeTool == null)
                    return false;

                return activeTool.GetType().Name.Contains("Bulldoze");
            }
            catch
            {
                return false;
            }
        }
    }
}