using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace Telefact
{
    public partial class MainForm : Form
    {
        private PrivateFontCollection fontCollection = new PrivateFontCollection();
        private Font? teletextFont;
        private readonly int cols = 40;
        private readonly int rows = 24;
        private readonly int cellWidth = 20;
        private readonly int cellHeight = 25;

        public MainForm()
        {
            InitializeComponent();
            LoadCustomFont();
            this.DoubleBuffered = true;
            this.Paint += MainForm_Paint;
        }

        private void LoadCustomFont()
        {
            string fontPath = Path.Combine(Application.StartupPath, "Fonts", "Modeseven.ttf");

            if (File.Exists(fontPath))
            {
                fontCollection.AddFontFile(fontPath);
                teletextFont = new Font(fontCollection.Families[0], 20, FontStyle.Regular);
            }
            else
            {
                MessageBox.Show("Modeseven.ttf not found in Fonts folder.");
            }
        }

        private void MainForm_Paint(object? sender, PaintEventArgs e)
        {
            if (teletextFont == null) return;

            Graphics g = e.Graphics;
            g.Clear(TeletextColors.Black);
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            // Header (first row)
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");
            string header = $"Page 301  Teletext  301  {timestamp}";
            g.DrawString(header.PadRight(cols), teletextFont, Brushes.White, 0, 0);

            // Dummy content starting from row 2 (index 1)
            Color[] colors = new Color[]
            {
                TeletextColors.Red,
                TeletextColors.Green,
                TeletextColors.Yellow,
                TeletextColors.Blue,
                TeletextColors.Magenta,
                TeletextColors.Cyan,
                TeletextColors.White
            };

            for (int row = 1; row < rows; row++)
            {
                Color textColor = colors[(row - 1) % colors.Length];
                using Brush brush = new SolidBrush(textColor);

                string colorName = textColor.Name.ToUpper();
                string content = $"This is a test in {colorName}".PadRight(cols);

                float x = 0;
                float y = row * cellHeight;

                g.DrawString(content, teletextFont, brush, x, y);
            }

        }
    }
}
