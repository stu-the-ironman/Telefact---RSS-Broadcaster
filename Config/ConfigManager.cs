using System;
using System.IO;
using System.Text.Json;

namespace Telefact.Config
{
    public static class ConfigManager
    {
        public static ConfigSettings Settings { get; private set; } = new();

        public static void LoadConfig()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    Settings = JsonSerializer.Deserialize<ConfigSettings>(json) ?? new ConfigSettings();
                    Log("Configuration loaded successfully.");
                }
                else
                {
                    Log("Config file not found. Using default settings.");
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to load config: {ex.Message}", isError: true);
                Settings = new ConfigSettings();
            }
        }

        private static void Log(string message, bool isError = false)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($"[ConfigManager] {(isError ? "ERROR" : "INFO")}: {message}");
            Console.ResetColor();
        }
    }
}
