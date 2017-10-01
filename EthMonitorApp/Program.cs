using System;
using System.Windows.Forms;

namespace EthMonitorApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MinerMonitor());

            StatsHelper.StatsDate = DateTime.Now;
        }
    }
}
