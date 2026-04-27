using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace DisableHover
{
    [Serializable]
    public class ConfigData
    {
        public bool tooltipsEnabled = true;
    }

    public static class Settings
    {
        private static string FolderPath =>
            Path.Combine(
                Application.persistentDataPath,
                ".cache",
                "Mods",
                "mods_unmanaged",
                "DisableHover"
            );

        private static string FilePath =>
            Path.Combine(FolderPath, "config.json");

        public static ConfigData Data = new ConfigData();

        public static void Load()
        {
            try
            {
                Mod.log.Info($"[Settings] Load from: {FilePath}");

                if (!Directory.Exists(FolderPath))
                {
                    Mod.log.Info($"[Settings] Creating folder: {FolderPath}");
                    Directory.CreateDirectory(FolderPath);
                }

                if (!File.Exists(FilePath))
                {
                    Mod.log.Warn("[Settings] File missing → creating default config");

                    Save(); // create default file
                    return;
                }

                string json = File.ReadAllText(FilePath);
                Mod.log.Info($"[Settings] Raw JSON: {json}");

                Data = JsonConvert.DeserializeObject<ConfigData>(json) ?? new ConfigData();

                Mod.log.Info($"[Settings] Parsed tooltipsEnabled = {Data.tooltipsEnabled}");
            }
            catch (Exception ex)
            {
                Mod.log.Error($"[Settings] Load failed: {ex}");
                Data = new ConfigData();
            }
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string json = JsonConvert.SerializeObject(Data, Formatting.Indented);

                File.WriteAllText(FilePath, json);

                Mod.log.Info($"[Settings] Saved JSON");
            }
            catch (Exception ex)
            {
                Mod.log.Error($"[Settings] Save failed: {ex}");
            }
        }
    }
}