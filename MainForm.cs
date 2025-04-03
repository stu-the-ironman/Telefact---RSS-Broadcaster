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
        private readonly PrivateFontCollection fontCollection = new();
        private Font? teletextFont;
        private TeletextPage mockPage = null!;

        private readonly int cols = 40;
        private readonly int rows = 24;
        private readonly int cellWidth = 20;
        private readonly int cellHeight = 25;
        private readonly int contentSidePadding = 1;

        private readonly string currentPage = "P100";
        private readonly string serviceName = "Telefact";
        private readonly Color serviceTextColor = TeletextColors.Yellow;
        private readonly Color serviceBackgroundColor = TeletextColors.Red;

        private readonly System.Windows.Forms.Timer clockTimer = new();
        private readonly System.Windows.Forms.Timer subpageTimer = new();

        private int currentSubpageIndex = 0;
        private MusicManager musicManager;

        public MainForm()
        {
            InitializeComponent();
            Console.WriteLine("[MainForm] DEBUG: MainForm initialized.");

            LoadCustomFont();
            InitMockPage();

            musicManager = new MusicManager();

            DoubleBuffered = true;
            Paint += MainForm_Paint;

            clockTimer.Interval = 1000 / 60;
            clockTimer.Tick += ClockTimer_Tick;
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
                MessageBox.Show("Modeseven.ttf not found. Falling back to Courier New.");
                teletextFont = new Font("Courier New", 20, FontStyle.Regular);
            }
        }

        private void InitMockPage()
        {
            string longStory = @"Teletext was a broadcast information service in the UK that began in the 1970s and became a staple of television culture. It provided news, weather, sports results, and TV listings through numbered pages. It was eventually discontinued, but its legacy lives on in modern digital services and retro fan projects like this one.";

            var words = longStory.Split(' ');
            var lines = new List<string>();
            string currentLine = "";

            foreach (string word in words)
            {
                if ((currentLine + " " + word).Trim().Length > (cols - 2 * contentSidePadding))
                {
                    lines.Add(currentLine.Trim().PadRight(cols));
                    currentLine = word;
                }
                else
                {
                    currentLine += " " + word;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
                lines.Add(currentLine.Trim().PadRight(cols));

            var subpages = new List<List<string>>();
            for (int i = 0; i < lines.Count; i += (rows - 4))
                subpages.Add(lines.Skip(i).Take(rows - 4).ToList());

            mockPage = new TeletextPage
            {
                PageNumber = 100,
                Subpages = subpages
            };
        }

        private void SubpageTimer_Tick(object? senderObject, EventArgs eventArguments)
        {
            if (mockPage.Subpages.Count > 1)
            {
                currentSubpageIndex = (currentSubpageIndex + 1) % mockPage.Subpages.Count;
                Invalidate();
            }
        }

        private void ClockTimer_Tick(object? senderObject, EventArgs eventArguments)
        {
            Invalidate();
        }

        private void MainForm_Paint(object? senderObject, PaintEventArgs paintEventArguments)
        {
            if (teletextFont == null)
            {
                MessageBox.Show("Font failed to load.");
                return;
            }

            Graphics graphics = paintEventArguments.Graphics;
            graphics.Clear(TeletextColors.Black);
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            // === HEADER ===
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");
            string pageLabelLeft = " P100";
            string currentPageNumberOnly = "100";
            string paddedServiceName = $"  {serviceName}  ";
            string centerBlock = $"{paddedServiceName} {currentPageNumberOnly}";

            int totalAvailableSpace = cols - (pageLabelLeft.Length + timestamp.Length);
            int leftPadding = 0;
            int rightPadding = 0;

            if (centerBlock.Length < totalAvailableSpace)
            {
                int remainingSpace = totalAvailableSpace - centerBlock.Length;
                leftPadding = remainingSpace / 2;
                rightPadding = remainingSpace - leftPadding;
            }

            string headerLine = pageLabelLeft + new string(' ', leftPadding) + centerBlock + new string(' ', rightPadding);

            int serviceStartColumn = pageLabelLeft.Length + leftPadding;
            int serviceBlockLength = paddedServiceName.Length;
            int serviceBackgroundX = serviceStartColumn * cellWidth;
            int serviceBackgroundWidth = serviceBlockLength * cellWidth;

            // Draw background block for service name
            graphics.FillRectangle(new SolidBrush(serviceBackgroundColor), serviceBackgroundX, 0, serviceBackgroundWidth, cellHeight);

            // Draw header characters
            for (int columnIndex = 0; columnIndex < headerLine.Length && columnIndex < cols; columnIndex++)
            {
                char characterToDraw = headerLine[columnIndex];
                float xPosition = columnIndex * cellWidth;
                float yPosition = 0;

                bool isInServiceBlock = (columnIndex >= serviceStartColumn) && (columnIndex < serviceStartColumn + serviceBlockLength);
                Brush fontBrush = isInServiceBlock ? new SolidBrush(serviceTextColor) : Brushes.White;

                graphics.DrawString(characterToDraw.ToString(), teletextFont!, fontBrush, xPosition, yPosition);
            }

            // Draw timestamp in yellow
            Brush timestampBrush = new SolidBrush(TeletextColors.Yellow);
            int timestampLength = timestamp.Length;
            int timestampStartColumn = cols - timestampLength;

            for (int characterIndex = 0; characterIndex < timestampLength; characterIndex++)
            {
                char character = timestamp[characterIndex];
                float xPosition = (timestampStartColumn + characterIndex) * cellWidth;
                float yPosition = 0;
                graphics.DrawString(character.ToString(), teletextFont!, timestampBrush, xPosition, yPosition);
            }

            // === SUBHEADER ===
            string dashLine = new string('-', cols);
            RenderDashLine(graphics, dashLine, 1 * cellHeight);

            string subheaderText = "Subheader Text Here";
            int availableSubheaderSpace = cols - (contentSidePadding * 2);
            int subheaderPadding = (availableSubheaderSpace - subheaderText.Length) / 2;
            subheaderText = subheaderText.PadLeft(subheaderText.Length + subheaderPadding);

            for (int characterIndex = 0; characterIndex < subheaderText.Length; characterIndex++)
            {
                char character = subheaderText[characterIndex];
                float x = (contentSidePadding + characterIndex) * cellWidth;
                float y = 2 * cellHeight;
                graphics.DrawString(character.ToString(), teletextFont, Brushes.White, x, y);
            }

            RenderDashLine(graphics, dashLine, 3 * cellHeight);

            // === CONTENT ===
            var content = mockPage.Subpages[currentSubpageIndex];
            for (int rowIndex = 0; rowIndex < content.Count && rowIndex < (rows - 4); rowIndex++)
            {
                float y = (rowIndex + 4) * cellHeight;
                float x = contentSidePadding * cellWidth;

                string rowContent = content[rowIndex].PadRight(cols - contentSidePadding * 2);

                for (int columnIndex = 0; columnIndex < rowContent.Length; columnIndex++)
                {
                    char character = rowContent[columnIndex];
                    float charX = x + columnIndex * cellWidth;
                    graphics.DrawString(character.ToString(), teletextFont, Brushes.White, charX, y);
                }
            }
        }

        private void RenderDashLine(Graphics graphics, string dashLine, float yPosition)
        {
            for (int columnIndex = 0; columnIndex < dashLine.Length && columnIndex < cols; columnIndex++)
            {
                char character = dashLine[columnIndex];
                float x = columnIndex * cellWidth;
                graphics.DrawString(character.ToString(), teletextFont, Brushes.White, x, yPosition);
            }
        }
    }
}
