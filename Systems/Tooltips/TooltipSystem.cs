using Colossal.UI.Binding;
using Game.UI;

namespace DisableHover.Systems.Tooltips
{
    // Native C# <-> UI bindings. No Harmony and no raycast/input interception.
    public partial class TooltipSystem : UISystemBase
    {
        private static ValueBinding<bool> _tooltipBinding;
        private static ValueBinding<bool> _blueHighlightBinding;
        private static ValueBinding<bool> _backendReadyBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            bool tooltips = Mod.Instance?.Settings?.DisableUIToolTips ?? false;
            bool highlights = Mod.Instance?.Settings?.DisableBlueHighLightOnBuildings ?? false;

            _tooltipBinding = new ValueBinding<bool>(
                "DisableHover",
                "GetDisableUIToolTips",
                tooltips
            );
            AddBinding(_tooltipBinding);

            _blueHighlightBinding = new ValueBinding<bool>(
                "DisableHover",
                "GetDisableBlueHighlights",
                highlights
            );
            AddBinding(_blueHighlightBinding);

            _backendReadyBinding = new ValueBinding<bool>(
                "DisableHover",
                "BackendReady",
                true
            );
            AddBinding(_backendReadyBinding);

            AddBinding(new TriggerBinding("DisableHover", "ToggleTooltips", Mod.ToggleTooltips));
            AddBinding(new TriggerBinding("DisableHover", "ToggleBlueHighlights", Mod.ToggleBlueHighlights));

            Mod.log.Info("DisableHover UI bindings registered");
        }

        public static void UpdateTooltipBinding(bool disabled)
        {
            _tooltipBinding?.Update(disabled);
        }

        public static void UpdateBlueHighlightBinding(bool disabled)
        {
            _blueHighlightBinding?.Update(disabled);
        }

        protected override void OnDestroy()
        {
            _backendReadyBinding?.Update(false);
            _tooltipBinding = null;
            _blueHighlightBinding = null;
            _backendReadyBinding = null;
            base.OnDestroy();
        }

        protected override void OnUpdate() { }
    }
}
