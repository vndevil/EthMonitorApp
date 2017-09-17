using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace EthMonitorApp
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://ethmonitor.net"));
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.buiducanh.net"));
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.facebook.com/vndevil"));
        }
    }
}