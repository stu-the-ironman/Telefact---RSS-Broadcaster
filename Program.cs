using System;
using System.Windows.Forms;

namespace Telefact
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Safely attempt to set the console title
            try
            {
                Console.Title = "Telefact v0.2.0";
            }
            catch (IOException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Program] WARNING: Unable to set console title. Possibly running without a terminal.");
                Console.ResetColor();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
