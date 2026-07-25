using Game;
using Game.Prefabs;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace DisableHover
{
    /*
     * This system hides the blue hover highlight without Harmony.
     *
     * It does not block raycasts.
     * It does not modify tool behavior.
     * It only changes the alpha/transparency of the game's outline colors.
     *
     * When DisableBlueHighlight is true:
     *   - hover outline alpha becomes 0
     *   - owner/selection hover alpha becomes 0
     *   - outline material outer/inner alpha becomes 0
     *
     * When DisableBlueHighlight is false:
     *   - original vanilla colors are restored
     */
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
#if DEBUG
            Mod.log.Info("[DisableHover] Outline system created");
#endif
        }

        protected override void OnUpdate()
        {
            bool disabled = Mod.DisableBlueHighlight;

            // Before changing anything, capture the original game colors.
            // If the rendering data or outline material is not ready yet, wait.
            if (!TryCaptureVanillaValues())
                return;

            // Only react when the setting changes.
            // This keeps the system mostly idle during normal gameplay.
            if (disabled == _lastDisabledState)
                return;

            if (disabled)
                ApplyInvisibleHighlight();
            else
                RestoreVanillaHighlight();

            _lastDisabledState = disabled;
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
            CustomPassVolume[] volumes = Object.FindObjectsOfType<CustomPassVolume>();

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