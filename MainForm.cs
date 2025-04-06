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
        private System.Windows.Forms.Timer renderTimer;
        private Renderer renderer;
        private MusicManager? musicManager;

    public MainForm()
    {
        InitializeComponent();
        Console.WriteLine("[MainForm] DEBUG: Initializing MainForm...");

        config = ConfigManager.Settings;
        renderer = new Renderer(config);

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

        DoubleBuffered = true;

        renderTimer = new System.Windows.Forms.Timer();
        renderTimer.Interval = 1000;
        renderTimer.Tick += (sender, e) => Invalidate();
        renderTimer.Start();
    }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            renderer.Render(e.Graphics, ClientSize);
        }
    }
}
