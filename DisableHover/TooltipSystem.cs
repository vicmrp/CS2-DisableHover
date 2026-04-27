using Game.UI;
using Colossal.UI.Binding;

namespace DisableHover
{
    public partial class TooltipSystem : UISystemBase
    {

        private static ValueBinding<bool> _binding;

        protected override void OnCreate()
        {
            base.OnCreate();

            Mod.log.Info("[TooltipSystem] OnCreate");

            Settings.Load();
            Mod.log.Info($"[TooltipSystem] Loaded value = {Settings.Data.tooltipsEnabled}");

            // This sends the signal to UI
            _binding = new ValueBinding<bool>(
                "DisableHover",
                "GetTooltipsEnabled",
                Settings.Data.tooltipsEnabled
            );;

            AddBinding(_binding);

            
            // AddBinding(new TriggerBinding<bool>(
            //     "DisableHover",
            //     "SetTooltipsEnabled",
            //     SetTooltipsEnabled
            // ));
        }

        public static void SetTooltipsEnabled(bool value)
        {
            Mod.log.Info($"[TooltipSystem] SET ← {value}");

            Settings.Data.tooltipsEnabled = value;
            Settings.Save();

            Mod.log.Info($"[TooltipSystem] SAVED = {Settings.Data.tooltipsEnabled}");
            if (_binding != null)
            {
                _binding.Update(value);
            }
            else
            {
                Mod.log.Warn("[TooltipSystem] Binding not ready yet");
            }
        }

        protected override void OnUpdate() { }
    }
}

