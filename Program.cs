using System;
using System.Windows.Forms;
using Telefact.Config;

namespace Telefact
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Load config first
            ConfigManager.LoadConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
