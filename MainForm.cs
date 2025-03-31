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

        // Header settings
        private readonly string currentPage = "P100";
        private readonly string serviceName = "Telefact";
        private readonly Color serviceTextColor = TeletextColors.Black;
        private readonly Color serviceBackgroundColor = TeletextColors.Yellow;

        private System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();

        public MainForm()
        {
            InitializeComponent();
            LoadCustomFont();

            this.DoubleBuffered = true;
            this.Paint += MainForm_Paint;

            clockTimer.Interval = 1000;
            clockTimer.Tick += (s, e) => this.Invalidate();
            clockTimer.Start();
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

            // === HEADER ===
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");

            string left = "" + currentPage;
            string centerLabel = serviceName;
            string centerPadded = $"  {centerLabel}  ";
            string center = $"{centerPadded} {currentPage}";
            int leftSpace = 0;
            int rightSpace = 0;

            int availableSpace = cols - (left.Length + timestamp.Length);
            if (center.Length < availableSpace)
            {
                int totalPad = availableSpace - center.Length;
                leftSpace = totalPad / 2;
                rightSpace = totalPad - leftSpace;
            }

            string headerLine = left + new string(' ', leftSpace) + center + new string(' ', Math.Max(0, rightSpace - 1)) + timestamp;

            int serviceStartCol = left.Length + leftSpace;
            int bgStartX = serviceStartCol * cellWidth;
            int bgWidth = centerPadded.Length * cellWidth;
            g.FillRectangle(new SolidBrush(serviceBackgroundColor), bgStartX, 0, bgWidth, cellHeight);

            for (int i = 0; i < headerLine.Length && i < cols; i++)
            {
                char c = headerLine[i];
                float x = i * cellWidth;
                float y = 0;

                bool isInServiceBlock = (i >= serviceStartCol) && (i < serviceStartCol + centerPadded.Length);
                Brush brush = isInServiceBlock ? new SolidBrush(serviceTextColor) : Brushes.White;

                g.DrawString(c.ToString(), teletextFont, brush, x, y);
            }

            // === GRID CONTENT ===
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

            for (int row = 2; row < rows; row++) // Start content from row 3
            {
                Color textColor = colors[(row - 2) % colors.Length];
                using Brush brush = new SolidBrush(textColor);

                string colorName = textColor.Name.ToUpper();
                string content = $"This is a test in {colorName}".PadRight(cols);

                float x = 0;
                float y = row * cellHeight;

                g.DrawString(content, teletextFont, brush, x, y);
            }

            // === SUBPAGE INDICATOR ===
            string subpage = "1/3"; // Placeholder
            int subpageCol = cols - subpage.Length;
            float subpageX = subpageCol * cellWidth;
            float subpageY = 1 * cellHeight; // Row directly below header

            g.DrawString(subpage, teletextFont, Brushes.White, subpageX, subpageY);
        }
    }
}
