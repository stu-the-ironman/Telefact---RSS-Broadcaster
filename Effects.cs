using System;
using System.Drawing;

namespace Telefact
{
    public class Effects
    {
        private readonly EffectSettings _settings;
        private readonly int _cols;
        private readonly int _rows;
        private readonly int _cellWidth;
        private readonly int _cellHeight;
        private readonly int _refreshRate;

        private readonly Random _random = new Random();

        public Effects(EffectSettings settings, int cols, int rows, int cellWidth, int cellHeight, int refreshRate)
        {
            _settings = settings;
            _cols = cols;
            _rows = rows;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
            _refreshRate = refreshRate;
        }

        private Color[,] staticPixels;
        private DateTime lastStaticUpdate;
        private readonly TimeSpan staticUpdateInterval = TimeSpan.FromSeconds(1); // Update interval for static effect

        // Apply static effect method (overlay static effect on top of the content)
        public void ApplyStaticEffect(Graphics g)
        {
            if (!_settings.StaticEnabled) return;

            // Update static effect at regular intervals
            if (DateTime.Now - lastStaticUpdate > staticUpdateInterval)
            {
                UpdateStaticPixels();
                lastStaticUpdate = DateTime.Now;
            }

            // Iterate through the rows and columns of the grid to render static as an overlay
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    // Get the color for this grid cell
                    Color staticColor = staticPixels[x, y];

                    // Only draw static where we have assigned a color
                    if (staticColor != Color.Empty)
                    {
                        // Semi-transparent brush for static effect
                        Brush brush = new SolidBrush(Color.FromArgb(128, staticColor));
                        g.FillRectangle(brush, x * _cellWidth, y * _cellHeight, _cellWidth, _cellHeight);
                    }
                }
            }
        }

        // Method to update the static effect at regular intervals
        private void UpdateStaticPixels()
        {
            staticPixels = new Color[_cols, _rows];

            // Randomly assign colors to the static pixels with the specified static strength
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    if (_random.NextDouble() < _settings.StaticStrength)
                    {
                        // Random color for static
                        staticPixels[x, y] = Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255));
                    }
                    else
                    {
                        staticPixels[x, y] = Color.Empty; // No static for this cell
                    }
                }
            }
        }

        // Apply scanline effect method
        public void ApplyScanlinesEffect(Graphics g)
        {
            if (!_settings.ScanlinesEnabled) return;

            // Define the strength of the scanline
            int scanlineHeight = (int)(_cellHeight * _settings.ScanlineStrength);

            for (int y = 0; y < _rows; y++)
            {
                // Apply scanline on every other row
                if (y % 2 == 0)
                {
                    Brush brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)); // Darker scanline color
                    g.FillRectangle(brush, 0, y * _cellHeight, _cols * _cellWidth, scanlineHeight);
                }
            }
        }
    }
}
