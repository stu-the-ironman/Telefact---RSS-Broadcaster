using System;
using System.Drawing;
using System.Windows.Forms;
using Telefact.Config;
using Telefact.Music;
using Telefact.Rendering;

namespace Telefact
{
    public partial class MainForm : Form
    {
        private ConfigSettings config;
        private MusicManager? musicManager;
        private Renderer renderer;
        private System.Windows.Forms.Timer renderTimer;

        public MainForm()
        {
            InitializeComponent();
            Console.WriteLine("[MainForm] DEBUG: Initializing MainForm...");

            // Load configuration
            ConfigManager.LoadConfig();
            config = ConfigManager.Settings;

            // Define Teletext grid dimensions (can be retrieved from config or set explicitly)
            int cellWidth = config.CellWidth; // Retrieve from config
            int cellHeight = config.CellHeight; // Retrieve from config

            // Initialize Effects
            var effects = new Effects(
                config.Effects,
                cols: 40, // Number of columns
                rows: 24, // Number of rows
                cellWidth: cellWidth,
                cellHeight: cellHeight
            );

            // Initialize Renderer with configurable cell dimensions
            renderer = new Renderer(config, effects, cellWidth, cellHeight);

            // Initialize MusicManager if music is enabled
            if (config.EnableMusic)
            {
                Console.WriteLine("[MainForm] DEBUG: Music is enabled, initializing MusicManager...");
                musicManager = new MusicManager(config);
                musicManager.Play();
            }
            else
            {
                Console.WriteLine("[MainForm] DEBUG: Music is disabled via config.");
            }

            // Enable double buffering for smoother rendering
            DoubleBuffered = true;

            // Initialize and start the render timer
            renderTimer = new System.Windows.Forms.Timer();
            renderTimer.Interval = 33; // ~30 FPS || 16ms for 60 FPS
            renderTimer.Tick += (s, e) => Invalidate();
            renderTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            renderer.Render(e.Graphics, ClientSize);
        }
    }
}
