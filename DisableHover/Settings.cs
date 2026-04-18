using System;
using System.IO;
using UnityEngine;

namespace DisableHover
{
    public static class Settings
    {
        private static string FilePath =>
            Path.Combine(Application.persistentDataPath, "disablehover.txt");

        public static bool TooltipsDisabled = false;

        public static void Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    TooltipsDisabled = false;
                    return;
                }

                string text = File.ReadAllText(FilePath).Trim();

                if (bool.TryParse(text, out bool value))
                    TooltipsDisabled = value;
                else
                    TooltipsDisabled = false;
            }
            catch (Exception ex)
            {
                Mod.log.Error($"Load failed: {ex}");
                TooltipsDisabled = false;
            }
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(FilePath, TooltipsDisabled.ToString());
            }
            catch (Exception ex)
            {
                Mod.log.Error($"Save failed: {ex}");
            }
        }
    }
}