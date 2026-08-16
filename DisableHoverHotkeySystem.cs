using System;
using Game;
using Game.Input;

namespace DisableHover
{
    /// <summary>
    /// Uses CS2's native mod input actions. Actions are resolved lazily so a
    /// key-binding problem can never abort Mod.OnLoad and unload the whole mod.
    /// </summary>
    public partial class DisableHoverHotkeySystem : GameSystemBase
    {
        private ProxyAction _toggleTooltipsAction;
        private ProxyAction _toggleBlueHighlightsAction;
        private bool _actionsReady;
        private bool _reportedFailure;
        private int _retryFrames;

        protected override void OnCreate()
        {
            base.OnCreate();
            Mod.log.Info("DisableHover hotkey system created (lazy binding)");
        }

        protected override void OnUpdate()
        {
            if (!_actionsReady)
            {
                // Do not try during the same startup frame as Mod.OnLoad.
                // Settings/input registration may not yet be fully available.
                if (_retryFrames-- > 0)
                    return;

                _retryFrames = 120;

                if (!TryResolveActions())
                    return;
            }

            try
            {
                if (_toggleTooltipsAction != null && _toggleTooltipsAction.WasPressedThisFrame())
                    Mod.ToggleTooltips();

                if (_toggleBlueHighlightsAction != null && _toggleBlueHighlightsAction.WasPressedThisFrame())
                    Mod.ToggleBlueHighlights();
            }
            catch (Exception ex)
            {
                // A runtime input failure should disable only hotkeys, never the mod.
                Mod.log.Warn($"DisableHover hotkey polling failed; will retry later: {ex.GetType().Name}: {ex.Message}");
                DisableActions();
                _actionsReady = false;
                _retryFrames = 120;
            }
        }

        private bool TryResolveActions()
        {
            try
            {
                var mod = Mod.Instance;
                var settings = mod?.Settings;
                if (settings == null)
                    return false;

                _toggleTooltipsAction = settings.GetAction(
                    Settings.ModSettings.ToggleTooltipsAction
                );
                _toggleBlueHighlightsAction = settings.GetAction(
                    Settings.ModSettings.ToggleBlueHighlightsAction
                );

                if (_toggleTooltipsAction == null || _toggleBlueHighlightsAction == null)
                {
                    if (!_reportedFailure)
                    {
                        Mod.log.Warn("DisableHover native hotkey actions are not available yet; toolbar/options remain active.");
                        _reportedFailure = true;
                    }
                    return false;
                }

                _toggleTooltipsAction.shouldBeEnabled = true;
                _toggleBlueHighlightsAction.shouldBeEnabled = true;
                _actionsReady = true;
                _reportedFailure = false;
                Mod.log.Info("DisableHover native hotkeys ready");
                return true;
            }
            catch (Exception ex)
            {
                if (!_reportedFailure)
                {
                    Mod.log.Warn($"DisableHover hotkeys could not be initialized yet; toolbar/options remain active: {ex.GetType().Name}: {ex.Message}");
                    _reportedFailure = true;
                }
                return false;
            }
        }

        private void DisableActions()
        {
            try
            {
                if (_toggleTooltipsAction != null)
                    _toggleTooltipsAction.shouldBeEnabled = false;
            }
            catch { }

            try
            {
                if (_toggleBlueHighlightsAction != null)
                    _toggleBlueHighlightsAction.shouldBeEnabled = false;
            }
            catch { }

            _toggleTooltipsAction = null;
            _toggleBlueHighlightsAction = null;
        }

        protected override void OnDestroy()
        {
            DisableActions();
            base.OnDestroy();
        }
    }
}
