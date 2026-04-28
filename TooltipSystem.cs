using Game.UI;
using Colossal.UI.Binding;

namespace vezit.DisableHover
{
    public partial class TooltipSystem : UISystemBase
    {

        private static ValueBinding<bool> _binding;

        protected override void OnCreate()
        {
            base.OnCreate();

            Mod.log.Info("[TooltipSystem] OnCreate");

            // This sends the signal to UI
            _binding = new ValueBinding<bool>(
                "DisableHover",
                "GetDisableUIToolTips",
                Mod.Instance.Settings.DisableUIToolTips
            );

            AddBinding(_binding);
        }

        public static void SetTooltipsEnabled(bool value)
        {
            Mod.log.Info($"[TooltipSystem] SET ← {value}");

            var settings = Mod.Instance.Settings;

            settings.DisableUIToolTips = value;
            settings.ApplyAndSave();

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

