using System;
using System.Collections.Generic;
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

            clockTimer.Interval = 1000;
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
                // Custom font found, load it
                fontCollection.AddFontFile(fontPath);
                teletextFont = new Font(fontCollection.Families[0], 20, FontStyle.Regular);
            }
            else
            {
                // If custom font is not found, fall back to "Courier New"
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
                // Check if adding the word would exceed the column width (40 columns - padding on both sides)
                if ((currentLine + " " + word).Trim().Length > (cols - 2 * contentSidePadding)) // padding on both sides
                {
                    // If it would, save the current line and start a new one
                    lines.Add(currentLine.Trim().PadRight(cols)); // Add the padded line (right padding)
                    currentLine = word; // Start a new line with the current word
                }
                else
                {
                    // Otherwise, add the word to the current line
                    currentLine += " " + word;
                }
            }

            // Add the last line
            if (!string.IsNullOrWhiteSpace(currentLine))
            {
                lines.Add(currentLine.Trim().PadRight(cols)); // Pad to make sure it’s 40 chars wide
            }

            // Now paginate the content into subpages (maximum 23 lines per subpage)
            var subpages = new List<List<string>>();
            for (int i = 0; i < lines.Count; i += (rows - 1)) // (rows - 1) because we reserve one line for the header
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
            // Ensure that the font is properly loaded before rendering
            if (teletextFont == null)
            {
                MessageBox.Show("Font is not loaded properly.");
                return; // Exit early if font is null
            }

            Graphics g = e.Graphics;
            g.Clear(TeletextColors.Black);
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            // === HEADER ===
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");

            string left = " " + currentPage;
            string centerLabel = serviceName;
            string centerPadded = $"  {centerLabel}  ";
            string center = $"{centerPadded} {currentPage}";
            int leftSpace = 0;
            int rightSpace = 0;

            // Calculate available space for the header
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

            // Render the header
            for (int i = 0; i < headerLine.Length && i < cols; i++)
            {
                char c = headerLine[i];
                float x = i * cellWidth;
                float y = 0;

                bool isInServiceBlock = (i >= serviceStartCol) && (i < serviceStartCol + centerPadded.Length);
                Brush brush = isInServiceBlock ? new SolidBrush(serviceTextColor) : Brushes.White;

                g.DrawString(c.ToString(), teletextFont!, brush, x, y);  // Use the null-forgiving operator
            }

            // === SUBHEADER === (Reserved for 3 rows)
            string dashLine = new string('-', cols);

            // Render the top dashed line
            RenderDashLine(g, dashLine, 1 * cellHeight);

            // Middle Row: Subheader text (centered in the row)
            string subheaderText = "Subheader Text Here"; // Example subheader text

            // Center the subheader text within the available space
            int availableSubheaderSpace = cols - (contentSidePadding * 2); // Space minus padding
            int subheaderPadding = (availableSubheaderSpace - subheaderText.Length) / 2; // Calculate padding for centering

            subheaderText = subheaderText.PadLeft(subheaderText.Length + subheaderPadding); // Apply padding to the left

            // Render each character of subheader text with appropriate padding
            for (int i = 0; i < subheaderText.Length; i++)
            {
                char c = subheaderText[i];
                float x = (contentSidePadding + i) * cellWidth; // X position adjusted with padding
                float y = 2 * cellHeight; // Middle row for subheader

                g.DrawString(c.ToString(), teletextFont!, Brushes.White, x, y); // Use the null-forgiving operator
            }

            // Bottom Row: Dashes across the whole row (fitting grid)
            RenderDashLine(g, dashLine, 3 * cellHeight);

            // === SUBPAGE INDICATOR ===
            int totalSubpages = mockPage.Subpages.Count;
            if (totalSubpages > 1)
            {
                string subpageText = $"{currentSubpageIndex + 1}/{totalSubpages}";
                int subpageCol = cols - subpageText.Length;
                float subpageX = subpageCol * cellWidth;
                float subpageY = 4 * cellHeight;

                g.DrawString(subpageText, teletextFont!, Brushes.White, subpageX, subpageY); // Use the null-forgiving operator
            }

            // === PAGE CONTENT === (Content starts at line 6)
            var content = mockPage.Subpages[currentSubpageIndex];
            for (int i = 0; i < content.Count && i < (rows - 4); i++) // Start at line 6
            {
                float y = (i + 4) * cellHeight; // Start at line 4 (after header and subheader)
                float x = contentSidePadding * cellWidth; // Apply left padding

                string rowContent = content[i].PadRight(cols - contentSidePadding * 2); // Apply right padding

                // Render each character individually
                for (int j = 0; j < rowContent.Length; j++)
                {
                    char c = rowContent[j];
                    float charX = x + j * cellWidth; // Calculate X position for each character
                    g.DrawString(c.ToString(), teletextFont!, Brushes.White, charX, y); // Use the null-forgiving operator
                }
            }
        }

        // Helper method to render dashed line
        private void RenderDashLine(Graphics g, string dashLine, float yPosition)
        {
            // Split dash line into individual characters
            for (int i = 0; i < dashLine.Length && i < cols; i++)
            {
                char c = dashLine[i];
                float x = i * cellWidth; // X position for each dash
                g.DrawString(c.ToString(), teletextFont, Brushes.White, x, yPosition);
            }
        }
    }
}
