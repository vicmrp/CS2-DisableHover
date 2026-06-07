using Game.UI;
using Colossal.UI.Binding;

namespace DisableHover.Patches.Tooltips
{
    public partial class TooltipSystem : UISystemBase
    {

        private static ValueBinding<bool> _binding;

        protected override void OnCreate()
        {
            base.OnCreate();
#if DEBUG
            Mod.log.Info("[TooltipSystem] OnCreate");
#endif
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
#if DEBUG
            Mod.log.Info($"[TooltipSystem] SET ← {value}");
#endif
            var settings = Mod.Instance.Settings;

            settings.DisableUIToolTips = value;
            settings.ApplyAndSave();

            if (_binding != null)
            {
                _binding.Update(value);
            }
            else
            {
#if DEBUG
                Mod.log.Warn("[TooltipSystem] Binding not ready yet");
#endif
            }
        }

        protected override void OnUpdate() { }
    }
}

