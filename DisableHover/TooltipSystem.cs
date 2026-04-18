using Game.UI;
using Colossal.UI.Binding;

namespace DisableHover
{
    public partial class TooltipSystem : UISystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            Settings.Load();

            AddBinding(new TriggerBinding<bool>(
                "DisableHover",
                "SetTooltipsDisabled",
                SetTooltipsDisabled
            ));

            AddBinding(new GetterValueBinding<bool>(
                "DisableHover",
                "GetTooltipsDisabled",
                () => Settings.TooltipsDisabled
            ));
        }

        private void SetTooltipsDisabled(bool value)
        {
            Settings.TooltipsDisabled = value;
            Settings.Save();

            Mod.log.Info($"TooltipsDisabled = {value}");
        }

        protected override void OnUpdate() { }
    }
}