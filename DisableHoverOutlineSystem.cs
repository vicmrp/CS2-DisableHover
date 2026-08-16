using Game;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace DisableHover
{
    /*
     * This system hides the blue hover highlight without Harmony.
     *
     * It does not block raycasts.
     * It does not modify tool behavior.
     * It changes only native rendering state; it does not block raycasts.
     *
     * When DisableBlueHighlight is true:
     *   - hover outline alpha becomes 0
     *   - owner/selection hover alpha becomes 0
     *   - outline material outer/inner alpha becomes 0
     *   - the DefaultToolSystem projected overlay draw count is temporarily set to 0
     *     for the render frame, removing the flat blue 2D hover projection
     *
     * When DisableBlueHighlight is false:
     *   - original vanilla colors are restored
     *   - projected overlays are left completely untouched
     */
    [UpdateAfter(typeof(OverlayRenderSystem))]
    public partial class DisableHoverOutlineSystem : GameSystemBase
    {
        // Query for the game's rendering settings singleton.
        // RenderingSettingsData contains colors used by the game's outline/highlight system.
        private EntityQuery _renderSettingsQuery;

        // Cached material used by the HDRP outline pass.
        // This controls the actual visible outline/fill shader colors.
        private Material _outlineMaterial;

        // Used to avoid scanning the scene every frame while searching for the material.
        private float _nextMaterialSearchTime;

        // True after we have stored the original vanilla colors.
        private bool _captured;

        // Original game colors, saved so we can restore them later.
        private Color _vanillaHoveredColor;
        private Color _vanillaOwnerColor;
        private Color _vanillaOuterColor;
        private Color _vanillaInnerColor;

        // Tracks the last enabled/disabled state.
        // This prevents applying the same change every frame.
        private bool _lastDisabledState;

        // Native OverlayRenderSystem projected category. The blue flat building
        // hover mesh was isolated experimentally to m_ProjectedInstanceCount.
        // We deliberately do NOT use RenderingSystem.hideOverlay because that
        // also hides notification/selection pins and other unrelated overlays.
        private OverlayRenderSystem _overlayRenderSystem;
        private ToolSystem _toolSystem;
        private FieldInfo _projectedInstanceCountField;

        // Per-render-frame restoration state. We only suppress projected overlays
        // while DefaultToolSystem is active, leaving road/transit/placement tool
        // projected guides available.
        private bool _projectedSuppressionActive;
        private int _projectedCountBeforeSuppression;



        private EntityQuery _guideLineSettingsQuery;

        private bool _capturedGuidelines;
        private Color _vanillaVeryLowGuidelineColor;
        private Color _vanillaLowGuidelineColor;
        private Color _vanillaMediumGuidelineColor;
        private Color _vanillaHighGuidelineColor;


        protected override void OnCreate()
        {
            base.OnCreate();

            // Find the singleton entity that stores rendering settings.
            _renderSettingsQuery = GetEntityQuery(
                ComponentType.ReadWrite<RenderingSettingsData>()
            );

            _guideLineSettingsQuery = GetEntityQuery(
                ComponentType.ReadWrite<GuideLineSettingsData>()
            );

            // The isolation test proved that the remaining flat blue 2D building
            // highlight is OverlayRenderSystem's Projected draw category.
            _overlayRenderSystem = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            _toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            _projectedInstanceCountField = typeof(OverlayRenderSystem).GetField(
                "m_ProjectedInstanceCount",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            if (_projectedInstanceCountField == null)
            {
                Mod.log.Error("[DisableHover] Could not resolve OverlayRenderSystem.m_ProjectedInstanceCount; projected hover suppression disabled");
            }

            RenderPipelineManager.endContextRendering += OnEndContextRendering;
#if DEBUG
            Mod.log.Info("[DisableHover] Outline/projected-overlay system created");
#endif
        }

        protected override void OnUpdate()
        {
            bool disabled = Mod.DisableBlueHighlight;

            // Suppress only the projected overlay produced by the normal/default
            // selection tool. This removes the flat blue 2D hover mesh without
            // globally hiding notification pins or tool overlays.
            UpdateProjectedOverlaySuppression(disabled);

            // Before changing outline colors, capture the original game colors.
            // If the rendering data or outline material is not ready yet, wait.
            if (!TryCaptureVanillaValues())
                return;

            // Only react to outline state changes. Overlay suppression above is
            // still checked every frame because the game may rewrite hideOverlay.
            if (disabled == _lastDisabledState)
                return;

            if (disabled)
                ApplyInvisibleHighlight();
            else
                RestoreVanillaHighlight();

            _lastDisabledState = disabled;
        }

        private void UpdateProjectedOverlaySuppression(bool disabled)
        {
            // If a render callback was skipped for some reason, never carry a
            // previous-frame suppression into a new update.
            RestoreProjectedOverlay("next rendering update");

            if (!disabled ||
                _overlayRenderSystem == null ||
                _projectedInstanceCountField == null ||
                _toolSystem == null)
            {
                return;
            }

            // Safety boundary: only remove the projected overlay while the game's
            // normal selection/inspection tool is active. Other tools (roads,
            // tracks, public transport, placement, zoning, etc.) keep their own
            // projected guidance untouched.
            if (!(_toolSystem.activeTool is DefaultToolSystem))
                return;

            try
            {
                int count = (int)_projectedInstanceCountField.GetValue(_overlayRenderSystem);

                if (count <= 0)
                    return;

                _projectedCountBeforeSuppression = count;
                _projectedInstanceCountField.SetValue(_overlayRenderSystem, 0);
                _projectedSuppressionActive = true;
#if DEBUG
                Mod.log.Debug($"[DisableHover] Suppressed projected hover overlay count={count}");
#endif
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "[DisableHover] Failed to suppress projected hover overlay");
                _projectedSuppressionActive = false;
            }
        }

        private void OnEndContextRendering(
            ScriptableRenderContext context,
            List<Camera> cameras)
        {
            RestoreProjectedOverlay("endContextRendering");
        }

        private void RestoreProjectedOverlay(string reason)
        {
            if (!_projectedSuppressionActive ||
                _overlayRenderSystem == null ||
                _projectedInstanceCountField == null)
            {
                return;
            }

            try
            {
                // Restore only if our zero is still present. If the game changed
                // the value itself during rendering, do not overwrite that state.
                int current = (int)_projectedInstanceCountField.GetValue(_overlayRenderSystem);
                if (current == 0)
                {
                    _projectedInstanceCountField.SetValue(
                        _overlayRenderSystem,
                        _projectedCountBeforeSuppression
                    );
                }
#if DEBUG
                else
                {
                    Mod.log.Debug($"[DisableHover] Projected overlay restore skipped ({reason}); game changed count to {current}");
                }
#endif
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "[DisableHover] Failed to restore projected overlay count");
            }
            finally
            {
                _projectedSuppressionActive = false;
                _projectedCountBeforeSuppression = 0;
            }
        }


        private bool TryCaptureVanillaValues()
        {
            if (_captured)
                return true;

            // RenderingSettingsData may not exist immediately on startup.
            if (_renderSettingsQuery.IsEmptyIgnoreFilter)
                return false;

            Entity entity = _renderSettingsQuery.GetSingletonEntity();
            RenderingSettingsData data =
                EntityManager.GetComponentData<RenderingSettingsData>(entity);

            // Save vanilla ECS highlight colors.
            _vanillaHoveredColor = data.m_HoveredColor;
            _vanillaOwnerColor = data.m_OwnerColor;

            // Also need the actual HDRP outline material.
            if (!TryResolveOutlineMaterial())
                return false;

            if (!_guideLineSettingsQuery.IsEmptyIgnoreFilter && !_capturedGuidelines)
            {
                Entity guidelineEntity = _guideLineSettingsQuery.GetSingletonEntity();
                GuideLineSettingsData guideData =
                    EntityManager.GetComponentData<GuideLineSettingsData>(guidelineEntity);

                _vanillaVeryLowGuidelineColor = guideData.m_VeryLowPriorityColor;
                _vanillaLowGuidelineColor = guideData.m_LowPriorityColor;
                _vanillaMediumGuidelineColor = guideData.m_MediumPriorityColor;
                _vanillaHighGuidelineColor = guideData.m_HighPriorityColor;

                _capturedGuidelines = true;
            }

            // Save vanilla material colors.
            _vanillaOuterColor = _outlineMaterial.GetColor("_OuterColor");
            _vanillaInnerColor = _outlineMaterial.GetColor("_InnerColor");

            _captured = true;
#if DEBUG
            Mod.log.Info("[DisableHover] Captured vanilla outline colors");
#endif
            return true;
        }

        private void ApplyInvisibleHighlight()
        {
            // Change ECS rendering settings.
            if (!_renderSettingsQuery.IsEmptyIgnoreFilter)
            {
                Entity entity = _renderSettingsQuery.GetSingletonEntity();
                RenderingSettingsData data =
                    EntityManager.GetComponentData<RenderingSettingsData>(entity);

                // Keep RGB the same, but make the color fully transparent.
                Color hovered = data.m_HoveredColor;
                hovered.a = 0f;

                Color owner = data.m_OwnerColor;
                owner.a = 0f;

                data.m_HoveredColor = hovered;
                data.m_OwnerColor = owner;

                if (!_guideLineSettingsQuery.IsEmptyIgnoreFilter)
                {
                    Entity guidelineEntity = _guideLineSettingsQuery.GetSingletonEntity();
                    GuideLineSettingsData guideData =
                        EntityManager.GetComponentData<GuideLineSettingsData>(guidelineEntity);

                    Color veryLow = guideData.m_VeryLowPriorityColor;
                    Color low = guideData.m_LowPriorityColor;
                    Color medium = guideData.m_MediumPriorityColor;
                    Color high = guideData.m_HighPriorityColor;

                    veryLow.a = 0f;
                    low.a = 0f;
                    medium.a = 0f;
                    high.a = 0f;

                    guideData.m_VeryLowPriorityColor = veryLow;
                    guideData.m_LowPriorityColor = low;
                    guideData.m_MediumPriorityColor = medium;
                    guideData.m_HighPriorityColor = high;

                    EntityManager.SetComponentData(guidelineEntity, guideData);
                }

                EntityManager.SetComponentData(entity, data);
            }

            // Change the actual outline shader material.
            if (TryResolveOutlineMaterial())
            {
                Color outer = _outlineMaterial.GetColor("_OuterColor");
                Color inner = _outlineMaterial.GetColor("_InnerColor");

                // Outer = visible outline edge.
                // Inner = fill/overlay inside the object silhouette.
                outer.a = 0f;
                inner.a = 0f;

                _outlineMaterial.SetColor("_OuterColor", outer);
                _outlineMaterial.SetColor("_InnerColor", inner);
            }
#if DEBUG
            Mod.log.Info("[DisableHover] Blue hover highlight hidden");
#endif
        }

        private void RestoreVanillaHighlight()
        {
            if (!_captured)
                return;

            // Restore ECS rendering settings.
            if (!_renderSettingsQuery.IsEmptyIgnoreFilter)
            {
                Entity entity = _renderSettingsQuery.GetSingletonEntity();
                RenderingSettingsData data =
                    EntityManager.GetComponentData<RenderingSettingsData>(entity);

                data.m_HoveredColor = _vanillaHoveredColor;
                data.m_OwnerColor = _vanillaOwnerColor;

                EntityManager.SetComponentData(entity, data);
            }

            if (_capturedGuidelines && !_guideLineSettingsQuery.IsEmptyIgnoreFilter)
            {
                Entity guidelineEntity = _guideLineSettingsQuery.GetSingletonEntity();
                GuideLineSettingsData guideData =
                    EntityManager.GetComponentData<GuideLineSettingsData>(guidelineEntity);

                guideData.m_VeryLowPriorityColor = _vanillaVeryLowGuidelineColor;
                guideData.m_LowPriorityColor = _vanillaLowGuidelineColor;
                guideData.m_MediumPriorityColor = _vanillaMediumGuidelineColor;
                guideData.m_HighPriorityColor = _vanillaHighGuidelineColor;

                EntityManager.SetComponentData(guidelineEntity, guideData);
            }
            
            // Restore HDRP material colors.
            if (TryResolveOutlineMaterial())
            {
                _outlineMaterial.SetColor("_OuterColor", _vanillaOuterColor);
                _outlineMaterial.SetColor("_InnerColor", _vanillaInnerColor);
            }
#if DEBUG
            Mod.log.Info("[DisableHover] Blue hover highlight restored");
#endif            
        }

        protected override void OnDestroy()
        {
            RenderPipelineManager.endContextRendering -= OnEndContextRendering;

            // Never leave per-frame projected overlay state modified when the
            // mod/system is unloaded.
            RestoreProjectedOverlay("system destroy");

            if (_captured && _lastDisabledState)
            {
                RestoreVanillaHighlight();
                _lastDisabledState = false;
            }

            base.OnDestroy();
        }

        private bool TryResolveOutlineMaterial()
        {
            if (_outlineMaterial != null)
                return true;

            // Do not search every frame.
            // Object.FindObjectsOfType can be expensive, so this only runs twice per second.
            float now = UnityEngine.Time.realtimeSinceStartup;
            if (now < _nextMaterialSearchTime)
                return false;

            _nextMaterialSearchTime = now + 0.5f;

            // Search HDRP custom pass volumes in the scene.
            // CS2 uses an OutlinesWorldUIPass for world UI outlines/highlights.
            CustomPassVolume[] volumes = UnityEngine.Object.FindObjectsOfType<CustomPassVolume>();

            foreach (CustomPassVolume volume in volumes)
            {
                if (volume == null || volume.customPasses == null)
                    continue;

                foreach (var customPass in volume.customPasses)
                {
                    // Find the outline pass and cache its fullscreen outline material.
                    if (customPass is OutlinesWorldUIPass pass &&
                        pass.m_FullscreenOutline != null)
                    {
                        _outlineMaterial = pass.m_FullscreenOutline;
#if DEBUG
                        Mod.log.Info("[DisableHover] Cached outline material");
#endif
                        return true;
                    }
                }
            }

            return false;
        }
    }
}