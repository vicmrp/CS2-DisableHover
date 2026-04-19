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
            Mod.log.Info($"[TooltipSystem] Loaded value = {Settings.Data.tooltipsEnabled}");

            AddBinding(new TriggerBinding<bool>(
                "DisableHover",
                "SetTooltipsEnabled",
                SetTooltipsEnabled
            ));

            AddBinding(new GetterValueBinding<bool>(
                "DisableHover",
                "GetTooltipsEnabled",
                () =>
                {
                    Mod.log.Info($"[TooltipSystem] GET → {Settings.Data.tooltipsEnabled}");
                    return Settings.Data.tooltipsEnabled;
                }
            ));
        }

        private void SetTooltipsEnabled(bool value)
        {
            Mod.log.Info($"[TooltipSystem] SET ← {value}");

            Settings.Data.tooltipsEnabled = value;
            Settings.Save();

            Mod.log.Info($"[TooltipSystem] SAVED = {Settings.Data.tooltipsEnabled}");
        }

        protected override void OnUpdate() { }
    }
}