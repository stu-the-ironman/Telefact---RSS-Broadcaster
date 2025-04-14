using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using Telefact.Config;

namespace Telefact.Rendering
{
    public class Renderer
    {
        private readonly ConfigSettings config;
        private readonly Effects effects;
        private readonly Font font;
        private readonly PrivateFontCollection? fontCollection; // To keep font alive

        private readonly Brush defaultBrush = Brushes.White;
        private readonly Brush serviceBrush = Brushes.Yellow;
        private readonly Brush serviceBackgroundBrush = Brushes.Red;
        private readonly Brush timestampBrush = Brushes.Yellow;
        private readonly Brush pageNumberBrush = Brushes.White;
        private readonly Brush contentBrush = Brushes.Cyan;

        private readonly int cellWidth;
        private readonly int cellHeight;
        private readonly int pageWidth = 38; // visible characters, padding excluded
        private readonly int leftPadding = 1;
        private readonly int rightPadding = 1;
        private readonly int topMargin = 18;

        private int rollingScanlineY = 0;
        private int frameCount = 0;

        public Renderer(ConfigSettings config, Effects effects, int cellWidth, int cellHeight)
        {
            this.config = config;
            this.effects = effects;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

            try
            {
                fontCollection = new PrivateFontCollection();
                string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Fonts", "Modeseven.ttf");
                fontCollection.AddFontFile(fontPath);
                font = new Font(fontCollection.Families[0], 20f, FontStyle.Regular);
                Log("Loaded custom font: Modeseven.ttf");
            }
            catch (Exception ex)
            {
                font = new Font(FontFamily.GenericMonospace, 20f, FontStyle.Regular);
                Log($"Failed to load custom font, using fallback. Reason: {ex.Message}", isError: true);
            }
        }

        public void Render(Graphics g, Size clientSize)
        {
            // Clear the screen with a black background
            g.Clear(Color.Black);

            // Draw the Teletext header
            DrawTeletextHeader(g, startX: 0);

            // Draw the Teletext content with padding below the header
            DrawContent(g, startX: 0, startY: topMargin + (2 * cellHeight)); // Add one line of padding

            // Draw the Teletext footer
            DrawTeletextFooter(g, startX: 0, clientHeight: clientSize.Height);

            // Apply visual effects (AFTER text and background are drawn)
            effects.ApplyStaticEffect(g, clientSize.Width, clientSize.Height);
            effects.ApplyScanlinesEffect(g, clientSize.Width, clientSize.Height);
            effects.ApplyBandingFlickerEffect(g, clientSize.Width, clientSize.Height, frameCount);
            effects.ApplyRollingScanlineEffect(g, clientSize.Width, clientSize.Height, ref rollingScanlineY);

            // Increment frame count for flicker effect
            frameCount++;
        }

        private void DrawTeletextHeader(Graphics g, int startX)
        {
            string pageNumber = "  P100";
            string serviceName = "  Telefact  ";
            string pageIndicator = "100";
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");

            int fullWidth = pageWidth + leftPadding + rightPadding;

            // Calculate the total width of all text sections
            int totalTextWidth = pageNumber.Length + serviceName.Length + pageIndicator.Length + timestamp.Length;

            // Calculate the remaining space for gaps
            int totalGaps = 3; // Gaps between pageNumber, serviceName, pageIndicator, and timestamp
            int gapWidth = (fullWidth - totalTextWidth) / totalGaps;

            // Calculate starting positions for each section
            int pageNumberStart = 0;
            int serviceNameStart = pageNumberStart + pageNumber.Length + gapWidth;
            int pageIndicatorStart = serviceNameStart + serviceName.Length + gapWidth;
            int timestampStart = pageIndicatorStart + pageIndicator.Length + gapWidth;

            for (int i = 0; i < fullWidth; i++)
            {
                float x = startX + (i * cellWidth);
                float y = topMargin;

                if (i >= pageNumberStart && i < pageNumberStart + pageNumber.Length)
                {
                    int idx = i - pageNumberStart;
                    SafeDrawString(g, pageNumber[idx].ToString(), font, pageNumberBrush, x, y);
                }
                else if (i >= serviceNameStart && i < serviceNameStart + serviceName.Length)
                {
                    int idx = i - serviceNameStart;
                    g.FillRectangle(serviceBackgroundBrush, x, y, cellWidth, cellHeight);
                    SafeDrawString(g, serviceName[idx].ToString(), font, serviceBrush, x, y);
                }
                else if (i >= pageIndicatorStart && i < pageIndicatorStart + pageIndicator.Length)
                {
                    int idx = i - pageIndicatorStart;
                    SafeDrawString(g, pageIndicator[idx].ToString(), font, pageNumberBrush, x, y);
                }
                else if (i >= timestampStart && i < timestampStart + timestamp.Length)
                {
                    int idx = i - timestampStart;
                    SafeDrawString(g, timestamp[idx].ToString(), font, timestampBrush, x, y);
                }
                else
                {
                    SafeDrawString(g, " ", font, defaultBrush, x, y);
                }
            }
        }

        private void DrawContent(Graphics g, int startX, int startY)
        {
            string rawText = "Teletext was a television service used in the UK and other countries from the 1970s to the 2010s. It allowed viewers to access additional information, such as news, sports, weather, and program guides, through their TV sets. The service was transmitted as a series of pages, each containing text-based information. Viewers accessed pages by entering numbers using their remote controls. Despite being replaced by digital services, Teletext played an important role in the development of interactive TV.";

            List<string> lines = WrapTextToLines(rawText, pageWidth);

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                float y = startY + (i * cellHeight);

                for (int j = 0; j < line.Length; j++)
                {
                    float x = startX + ((leftPadding + j) * cellWidth);
                    SafeDrawString(g, line[j].ToString(), font, contentBrush, x, y);
                }
            }
        }

        private void DrawTeletextFooter(Graphics g, int startX, int clientHeight)
        {
            int totalColumns = pageWidth + leftPadding + rightPadding;
            int contentStartColumn = leftPadding;
            int contentEndColumn = contentStartColumn + pageWidth;

            int rowIndex = (clientHeight - topMargin) / cellHeight - 1;
            float y = topMargin + (rowIndex * cellHeight);

            for (int col = contentStartColumn; col < contentEndColumn; col++)
            {
                float x = startX + (col * cellWidth);
                g.FillRectangle(Brushes.White, x, y, cellWidth, cellHeight);
            }

            string footerText = $"Footer Line - Row {rowIndex}";
            Brush footerBrush = Brushes.Red;
            int textStartColumn = contentStartColumn + 1;

            for (int i = 0; i < footerText.Length && (textStartColumn + i) < contentEndColumn; i++)
            {
                float x = startX + ((textStartColumn + i) * cellWidth);
                SafeDrawString(g, footerText[i].ToString(), font, footerBrush, x, y);
            }
        }

        private List<string> WrapTextToLines(string text, int maxLineLength)
        {
            List<string> lines = new();
            string[] words = text.Split(' ');
            string currentLine = "";

            foreach (string word in words)
            {
                if ((currentLine.Length + word.Length + 1) <= maxLineLength)
                {
                    currentLine += (currentLine.Length > 0 ? " " : "") + word;
                }
                else
                {
                    lines.Add(currentLine.PadRight(maxLineLength));
                    currentLine = word;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
                lines.Add(currentLine.PadRight(maxLineLength));

            return lines;
        }

        private void SafeDrawString(Graphics g, string text, Font font, Brush brush, float x, float y)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    g.DrawString(text, font, brush, x, y);
                }
            }
            catch (Exception ex)
            {
                Log($"DrawString failed: {ex.Message}", true);
            }
        }

        private void Log(string message, bool isError = false)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Cyan;
            Console.WriteLine($"[Renderer] {(isError ? "ERROR" : "INFO")}: {message}");
            Console.ResetColor();
        }
    }
}
