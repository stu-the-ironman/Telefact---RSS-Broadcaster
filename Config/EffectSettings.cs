namespace Telefact.Config
{
    public class EffectSettings
    {
        public bool StaticEnabled { get; set; } = true;
        public double StaticStrength { get; set; } = 0.05;
        public bool ScanlinesEnabled { get; set; } = false;
        public float ScanlineStrength { get; set; } = 0.3f;
        public bool BandingFlickerEnabled { get; set; } = false;
        public int BandingHeight { get; set; } = 3;
        public bool RollingScanlineEnabled { get; set; } = false;
        public int RollingScanlineSpeed { get; set; } = 2;
    }
}
