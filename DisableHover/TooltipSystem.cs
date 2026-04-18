using Game.UI;
using Colossal.UI.Binding;

namespace DisableHover
{
    public partial class TooltipSystem : UISystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            Mod.log.Info("[TooltipSystem] OnCreate");

            Settings.Load();
            Mod.log.Info($"[TooltipSystem] Loaded value = {Settings.TooltipsDisabled}");

            AddBinding(new TriggerBinding<bool>(
                "DisableHover",
                "SetTooltipsDisabled",
                SetTooltipsDisabled
            ));

            AddBinding(new GetterValueBinding<bool>(
                "DisableHover",
                "GetTooltipsDisabled",
                () =>
                {
                    Mod.log.Info($"[TooltipSystem] GET → {Settings.TooltipsDisabled}");
                    return Settings.TooltipsDisabled;
                }
            ));
        }

        private void SetTooltipsDisabled(bool value)
        {
            Mod.log.Info($"[TooltipSystem] SET ← {value}");

            Settings.TooltipsDisabled = value;
            Settings.Save();

            Mod.log.Info($"[TooltipSystem] SAVED = {Settings.TooltipsDisabled}");
        }

        protected override void OnUpdate() { }
    }
}