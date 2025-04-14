namespace Telefact.Config
{
    public class ConfigSettings
    {
        public int CellWidth { get; set; } = 20; // Default value
        public int CellHeight { get; set; } = 26; // Default value
        public bool EnableMusic { get; set; }
        public EffectSettings Effects { get; set; }

        public ConfigSettings()
        {
            Effects = new EffectSettings();
        }
    }
}
