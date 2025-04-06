
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
        private readonly Font font;
        private readonly PrivateFontCollection fontCollection; // To keep font alive
        private readonly Brush defaultBrush = Brushes.White;
        private readonly Brush serviceBrush = Brushes.Yellow;
        private readonly Brush serviceBackgroundBrush = Brushes.Red;
        private readonly Brush timestampBrush = Brushes.Yellow;
        private readonly Brush pageNumberBrush = Brushes.White;
        private readonly Brush contentBrush = Brushes.Cyan;
        private readonly int cellWidth = 20;
        private readonly int cellHeight = 26;
        private readonly int pageWidth = 38; // visible characters, padding excluded
        private readonly int leftPadding = 1;
        private readonly int rightPadding = 1;
        private readonly int topMargin = 18;

        public Renderer(ConfigSettings config)
        {
            this.config = config;

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

        public void Render(Graphics g, Size clientSize)
        {
            g.Clear(Color.Black);
            int totalGridWidth = (pageWidth + leftPadding + rightPadding) * cellWidth;
            int startX = (clientSize.Width - totalGridWidth) / 2;

            DrawTeletextHeader(g, startX);
            DrawContent(g, startX, topMargin + cellHeight * 2);
            DrawTeletextFooter(g, startX, clientSize.Height);
        }


        private void DrawTeletextHeader(Graphics g, int startX)
        {
            string pageNumber = "  " + "P100";
            string serviceName = "Telefact";
            string paddedService = $"  {serviceName}  ";
            string timestamp = DateTime.Now.ToString(" " + "MMM dd HH:mm:ss");

            int fullWidth = pageWidth + leftPadding + rightPadding;
            int gap = 2;

            int pnWidth = pageNumber.Length;
            int svcWidth = paddedService.Length;
            int tsWidth = timestamp.Length;

            int totalOccupied = pnWidth + svcWidth + tsWidth + (2 * gap);
            int svcStart = (fullWidth - totalOccupied) / 2 + pnWidth + gap;
            int tsStart = svcStart + svcWidth + gap;

            for (int i = 0; i < fullWidth; i++)
            {
                float x = startX + (i * cellWidth);
                float y = topMargin;

                if (i < pnWidth)
                {
                    SafeDrawString(g, pageNumber[i].ToString(), font, pageNumberBrush, x, y);
                }
                else if (i >= svcStart && i < svcStart + svcWidth)
                {
                    int idx = i - svcStart;
                    g.FillRectangle(serviceBackgroundBrush, x, y, cellWidth, cellHeight);
                    SafeDrawString(g, paddedService[idx].ToString(), font, serviceBrush, x, y);
                }
                else if (i >= tsStart && i < tsStart + tsWidth)
                {
                    int idx = i - tsStart;
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

            // Calculate Y position for the final row
            int rowIndex = (clientHeight - topMargin) / cellHeight - 1;
            float y = topMargin + (rowIndex * cellHeight);

            // Draw background only inside the content area (excluding padding)
            for (int col = contentStartColumn; col < contentEndColumn; col++)
            {
                float x = startX + (col * cellWidth);
                g.FillRectangle(Brushes.White, x, y, cellWidth, cellHeight);
            }

            // Footer text, shifted 1 cell inside the content area
            string footerText = $"Footer Line - Row {rowIndex}";
            Brush footerBrush = Brushes.Red;

            int textStartColumn = contentStartColumn + 1; // 1 cell padding inside content area

            for (int i = 0; i < footerText.Length && (textStartColumn + i) < contentEndColumn; i++)
            {
                float x = startX + ((textStartColumn + i) * cellWidth);
                g.DrawString(footerText[i].ToString(), font, footerBrush, x, y);
            }
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
