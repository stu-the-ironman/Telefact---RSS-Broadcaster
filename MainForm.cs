using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Telefact
{
    public partial class MainForm : Form
    {
        private PrivateFontCollection fontCollection = new PrivateFontCollection();
        private Font? teletextFont;
        private TeletextPage mockPage = null!;

        private readonly int cols = 40; // Number of columns per line
        private readonly int rows = 24; // Number of rows per page
        private readonly int cellWidth = 20; // Width of each cell (character)
        private readonly int cellHeight = 25; // Height of each cell (line)
        private readonly int contentSidePadding = 1; // characters of left/right padding

        private readonly string currentPage = "P100";
        private readonly string serviceName = "Telefact";
        private readonly Color serviceTextColor = TeletextColors.Yellow;
        private readonly Color serviceBackgroundColor = TeletextColors.Red;

        private readonly System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer subpageTimer = new System.Windows.Forms.Timer();

        private int currentSubpageIndex = 0;

        public MainForm()
        {
            InitializeComponent();
            LoadCustomFont();
            InitMockPage();

            this.DoubleBuffered = true;
            this.Paint += MainForm_Paint;

            clockTimer.Interval = 1000 / 60; // Set to 60fps
            clockTimer.Tick += (s, e) => this.Invalidate();
            clockTimer.Start();

            subpageTimer.Interval = 5000;
            subpageTimer.Tick += SubpageTimer_Tick;
            subpageTimer.Start();
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
                MessageBox.Show("Modeseven.ttf not found in Fonts folder. Using Courier New font.");
                teletextFont = new Font("Courier New", 20, FontStyle.Regular); // Default to Courier New
            }
        }

        private void InitMockPage()
        {
            string longStory = @"Teletext was a broadcast information service in the UK that began in the 1970s and became a staple of television culture. It provided news, weather, sports results, and TV listings through numbered pages. It was eventually discontinued, but its legacy lives on in modern digital services and retro fan projects like this one.";

            var words = longStory.Split(' ');
            var lines = new List<string>(); // Holds the current page lines

            string currentLine = "";
            foreach (var word in words)
            {
                if ((currentLine + " " + word).Trim().Length > (cols - 2 * contentSidePadding)) // padding on both sides
                {
                    lines.Add(currentLine.Trim().PadRight(cols)); // Add the padded line (right padding)
                    currentLine = word; // Start a new line with the current word
                }
                else
                {
                    currentLine += " " + word;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
            {
                lines.Add(currentLine.Trim().PadRight(cols)); // Add the last line
            }

            var subpages = new List<List<string>>();
            for (int i = 0; i < lines.Count; i += (rows - 1))
            {
                subpages.Add(lines.Skip(i).Take(rows - 1).ToList()); // Each subpage contains 23 lines of content
            }

            mockPage = new TeletextPage
            {
                PageNumber = 100,
                Subpages = subpages
            };
        }

        private void SubpageTimer_Tick(object? sender, EventArgs e)
        {
            if (mockPage.Subpages.Count > 1)
            {
                currentSubpageIndex = (currentSubpageIndex + 1) % mockPage.Subpages.Count;
                Invalidate();
            }
        }

        private void MainForm_Paint(object? sender, PaintEventArgs e)
        {
            if (teletextFont == null)
            {
                MessageBox.Show("Font is not loaded properly.");
                return; // Exit early if font is null
            }

            Graphics g = e.Graphics;
            g.Clear(TeletextColors.Black); // Background color

            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            // === HEADER ===
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");

            string left = " " + currentPage;
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

            // === SUBHEADER ===
            string dashLine = new string('-', cols);

            RenderDashLine(g, dashLine, 1 * cellHeight);

            string subheaderText = "Subheader Text Here"; // Example subheader text
            int availableSubheaderSpace = cols - (contentSidePadding * 2); // Space minus padding
            int subheaderPadding = (availableSubheaderSpace - subheaderText.Length) / 2; // Calculate padding for centering

            subheaderText = subheaderText.PadLeft(subheaderText.Length + subheaderPadding); // Apply padding to the left

            for (int i = 0; i < subheaderText.Length; i++)
            {
                char c = subheaderText[i];
                float x = (contentSidePadding + i) * cellWidth; // X position adjusted with padding
                float y = 2 * cellHeight; // Middle row for subheader

                g.DrawString(c.ToString(), teletextFont, Brushes.White, x, y);
            }

            RenderDashLine(g, dashLine, 3 * cellHeight);

            // === PAGE CONTENT ===
            var content = mockPage.Subpages[currentSubpageIndex];
            for (int i = 0; i < content.Count && i < (rows - 4); i++) // Start at line 6
            {
                float y = (i + 4) * cellHeight;
                float x = contentSidePadding * cellWidth;

                string rowContent = content[i].PadRight(cols - contentSidePadding * 2);

                for (int j = 0; j < rowContent.Length; j++)
                {
                    char c = rowContent[j];
                    float charX = x + j * cellWidth;
                    g.DrawString(c.ToString(), teletextFont, Brushes.White, charX, y);
                }
            }
        }

        private void RenderDashLine(Graphics g, string dashLine, float yPosition)
        {
            for (int i = 0; i < dashLine.Length && i < cols; i++)
            {
                char c = dashLine[i];
                float x = i * cellWidth;
                g.DrawString(c.ToString(), teletextFont, Brushes.White, x, yPosition);
            }
        }
    }
}
