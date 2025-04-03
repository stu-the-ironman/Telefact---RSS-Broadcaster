using System;
using System.IO;
using System.Text.Json;

namespace Telefact
{
    public class ConfigSettings
    {
        public bool EnableMusic { get; set; } = true;
    }

    public static class ConfigManager
    {
        public static ConfigSettings Settings { get; private set; } = new();

        public static void LoadConfig(string configFileName = "config.json")
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(basePath, configFileName);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[ConfigManager] WARNING: {configFileName} not found at: {fullPath}. Using defaults.");
                    return;
                }

                string json = File.ReadAllText(fullPath);
                ConfigSettings? loadedSettings = JsonSerializer.Deserialize<ConfigSettings>(json);

                if (loadedSettings != null)
                {
                    Settings = loadedSettings;
                    Console.WriteLine("[ConfigManager] INFO: Configuration loaded successfully.");
                }
                else
                {
                    Console.WriteLine("[ConfigManager] WARNING: Configuration file was empty or invalid. Using defaults.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConfigManager] ERROR: Failed to load config: {ex.Message}");
            }
        }
    }
}
