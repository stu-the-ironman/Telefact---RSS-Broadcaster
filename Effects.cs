using System;
using System.Drawing;
using Telefact.Config;

namespace Telefact
{
    public class Effects
    {
        private readonly EffectSettings _settings;
        private readonly int _cols;
        private readonly int _rows;
        private readonly int _cellWidth;
        private readonly int _cellHeight;

        private readonly Random _random = new Random();
        private static readonly int StaticAlpha = 128;

        // Define staticPixels
        private readonly Color[,] staticPixels;

        // Customizable variables
        public int RollingScanlineSpeed { get; set; } = 2; // Default speed for rolling scanlines
        public int BandingFlickerSpeed { get; set; } = 30; // Default interval for banding flicker
        public int StaticScanlineSpacing { get; set; } = 4; // Default spacing for static scanlines

        public Effects(EffectSettings settings, int cols, int rows, int cellWidth, int cellHeight)
        {
            Console.WriteLine("[Effects] DEBUG: Constructor called.");
            _settings = settings;
            _cols = cols;
            _rows = rows;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;

            staticPixels = new Color[cols, rows];
            UpdateStaticPixels(); // Initial fill
        }

        // Implement UpdateStaticPixels
        private void UpdateStaticPixels()
        {
            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    staticPixels[x, y] = Color.FromArgb(
                        StaticAlpha,
                        _random.Next(256),
                        _random.Next(256),
                        _random.Next(256)
                    );
                }
            }
        }

        public void ApplyStaticEffect(Graphics g, int width, int height)
        {
            // Example implementation for static effect
            for (int i = 0; i < 100; i++) // Adjust particle count as needed
            {
                int x = _random.Next(width);
                int y = _random.Next(height);
                using (Brush brush = new SolidBrush(Color.FromArgb(128, _random.Next(256), _random.Next(256), _random.Next(256))))
                {
                    g.FillRectangle(brush, x, y, 2, 2); // Draw small static particles
                }
            }
        }

        public void ApplyScanlinesEffect(Graphics g, int width, int height)
        {
            // Example implementation for static scanlines effect
            using (Brush brush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                for (int y = 0; y < height; y += StaticScanlineSpacing) // Use customizable spacing
                {
                    g.FillRectangle(brush, 0, y, width, 2); // Draw scanlines
                }
            }
        }

        public void ApplyBandingFlickerEffect(Graphics g, int width, int height, int frameCount)
        {
            // Example implementation for banding flicker effect
            if (frameCount % BandingFlickerSpeed != 0) return; // Control flicker speed
            using (Brush brush = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
            {
                int bandHeight = _random.Next(10, 30); // Random band height
                int bandY = _random.Next(0, height - bandHeight);
                g.FillRectangle(brush, 0, bandY, width, bandHeight);
            }
        }

        public void ApplyRollingScanlineEffect(Graphics g, int width, int height, ref int rollingY)
        {
            // Example implementation for rolling scanline effect
            using (Brush brush = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
            {
                g.FillRectangle(brush, 0, rollingY, width, 2); // Draw rolling scanline
            }
            rollingY = (rollingY + RollingScanlineSpeed) % height; // Use customizable speed
        }
    }
}
