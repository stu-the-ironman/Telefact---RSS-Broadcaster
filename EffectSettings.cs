namespace Telefact
{
    public class EffectSettings
    {
        // Whether the static effect is enabled
        public bool StaticEnabled { get; set; } = false;

        // Strength of the static effect (between 0.0 and 1.0)
        public float StaticStrength { get; set; } = 0.05f;

        // Whether the scanlines effect is enabled
        public bool ScanlinesEnabled { get; set; } = false;

        // Strength of the scanlines effect (between 0.0 and 1.0)
        public float ScanlineStrength { get; set; } = 0.3f;
    }
}
