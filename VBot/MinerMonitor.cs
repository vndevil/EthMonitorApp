using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Fizzler.Systems.HtmlAgilityPack;
using Newtonsoft.Json;
using VBot.buiducanh;

namespace VBot
{
    public partial class MinerMonitor : Form
    {
        private Thread thMonitor;
        WebClient client = new WebClient();
        IndexSoapClient buiducanhWebservie = new IndexSoapClient("IndexSoap12");
        private string DataFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data.dll";

        public MinerMonitor()
        {
            InitializeComponent();

            thMonitor = new Thread(GetData) { IsBackground = true };
        }

        private void GetData()
        {
            try
            {
                while (true)
                {
                    var host = txtHost.Text.Replace("http://", string.Empty);
                    var port = txtPort.Text;
                    var content = client.DownloadString("http://" + host + ":" + port);
                    //var content = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "test.txt");
                    var html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(content);
                    var doc = html.DocumentNode;

                    // Lấy dữ liệu MinerInfo
                    // Lấy danh sách dòng
                    var arrTrs = doc.QuerySelectorAll("tr").ToList();

                    // Xóa cột th
                    arrTrs.RemoveAt(0);

                    foreach (var node in arrTrs)
                    {
                        // Lấy danh sách cột
                        var arrCols = node.QuerySelectorAll("td").ToList();

                        // Thêm mới Miner
                        var miner = new MinerInfo
                        {
                            EmailId = txtEmail.Text.Trim(),
                            Name = arrCols[0].InnerText,
                            Ip = arrCols[1].InnerText,
                            RunningTime = arrCols[2].InnerText,
                            EthereumStats = arrCols[3].InnerText,
                            DcrInfo = arrCols[4].InnerText,
                            GpuTemperature = arrCols[5].InnerText,
                            Pool = arrCols[6].InnerText,
                            Version = arrCols[7].InnerText,
                            Comments = arrCols[8].InnerText
                        };
                        buiducanhWebservie.InsertMiner(miner);
                        //MessageBox.Show(JsonConvert.SerializeObject(miner));
                    }

                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                thMonitor?.Abort();
                MessageBox.Show(ex.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(linkLabel1.Text));
        }

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            var email = txtEmail.Text.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                // Ghi nhớ Email
                if (!File.Exists(DataFilePath))
                {
                    File.Create(DataFilePath).Close();
                }
                File.WriteAllText(DataFilePath, email);

                // Monitoring
                thMonitor = new Thread(GetData) { IsBackground = true };
                thMonitor.Start();
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                btnMonitor.Enabled = false;
                txtEmail.Enabled = false;
                btnStop.Enabled = true;
                lblMonitor.Visible = true;
                linkLabel1.Visible = true;
                linkLabel1.Text = @"https://www.buiducanh.net/Pages/theodoitrau.aspx?Email=" + email;
            }
            else
            {
                MessageBox.Show(this, @"Bạn chưa nhập Email !!!", @"Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MinerMonitor_Load(object sender, EventArgs e)
        {
            if (File.Exists(DataFilePath) && !string.IsNullOrEmpty(File.ReadAllText(DataFilePath)))
            {
                var email = File.ReadAllText(DataFilePath);

                txtEmail.Text = email;
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                txtEmail.Enabled = false;
                btnMonitor.Enabled = false;
                btnStop.Enabled = true;
                lblMonitor.Visible = true;
                linkLabel1.Visible = true;
                linkLabel1.Text = @"https://www.buiducanh.net/Pages/theodoitrau.aspx?Email=" + email;

                thMonitor.Start();
            }
            else
            {
                txtHost.Enabled = true;
                txtPort.Enabled = true;
                txtEmail.Enabled = true;
                btnMonitor.Enabled = true;
                lblMonitor.Visible = false;
                linkLabel1.Visible = false;
                btnStop.Enabled = false;

                thMonitor.Abort();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            txtHost.Enabled = true;
            txtPort.Enabled = true;
            txtEmail.Enabled = true;
            btnMonitor.Enabled = true;
            lblMonitor.Visible = false;
            linkLabel1.Visible = false;
            thMonitor.Abort();
        }

        private void MinerMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            thMonitor?.Abort();
        }
    }
}
