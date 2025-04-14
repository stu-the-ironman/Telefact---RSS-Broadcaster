namespace Telefact.Config
{
    public class TeletextEffects
    {
        private readonly EffectSettings settings;
        private readonly int cols;
        private readonly int rows;
        private readonly int cellWidth;
        private readonly int cellHeight;

        public TeletextEffects(EffectSettings settings, int cols, int rows, int cellWidth, int cellHeight)
        {
            this.settings = settings;
            this.cols = cols;
            this.rows = rows;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

            Console.WriteLine("[TeletextEffects] INFO: Effects initialized with grid dimensions.");
        }

        public void ApplyStaticEffect(Graphics g, int width, int height)
        {
            // Lower alpha value to make the static effect barely visible
            using (Brush brush = new SolidBrush(Color.FromArgb(2, Color.White))) // Previously 20
            {
                g.FillRectangle(brush, 0, 0, width, height);
            }
        }

        public void ApplyScanlinesEffect(Graphics g, int width, int height)
        {
            // Lower alpha value for scanlines
            using (Pen pen = new Pen(Color.FromArgb(60, Color.Black))) // Previously 30
            {
                for (int y = 0; y < height; y += 2)
                {
                    g.DrawLine(pen, 0, y, width, y);
                }
            }
        }

        public void ApplyBandingFlickerEffect(Graphics g, int width, int height, int frameCount)
        {
            if (frameCount % 2 == 0) // Flicker every other frame
            {
                // Lower alpha value for the flicker effect
                using (Brush brush = new SolidBrush(Color.FromArgb(5, Color.Black))) // Previously 15
                {
                    g.FillRectangle(brush, 0, 0, width, height / 2);
                }
            }
        }

        public void ApplyRollingScanlineEffect(Graphics g, int width, int height, ref int rollingScanlineY)
        {
            // Lower alpha value for the rolling scanline
            using (Brush brush = new SolidBrush(Color.FromArgb(10, Color.White))) // Previously 50
            {
                g.FillRectangle(brush, 0, rollingScanlineY, width, 2);
            }

            rollingScanlineY += 2;
            if (rollingScanlineY > height)
            {
                rollingScanlineY = 0;
            }
        }
    }
}