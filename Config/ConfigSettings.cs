using Telefact.Config;

namespace Telefact.Config
{
    public class ConfigSettings
    {
        public int CellWidth { get; set; } = 20;
        public int CellHeight { get; set; } = 26;
        public EffectSettings Effects { get; set; } = new EffectSettings();
        public bool EnableMusic { get; set; } = true;
    }
}
