using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Fizzler.Systems.HtmlAgilityPack;
using VBot.MonitorServices;

namespace VBot
{
    public partial class MinerMonitor : Form
    {
        const int SLEEP_TIME = 7000;
        private Thread thMonitor;
        WebClient client = new WebClient();
        MonitorServicesSoapClient monitorService = new MonitorServicesSoapClient("MonitorServicesSoap");
        private string DataFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data.dll";

        public MinerMonitor()
        {
            InitializeComponent();

            thMonitor = new Thread(GetData) { IsBackground = true };
        }

        delegate void SetGridViewDataSourceDelegate(string result);

        private void SetTextBoxData(string result)
        {
            if (InvokeRequired)
                Invoke(new SetGridViewDataSourceDelegate(SetTextBoxData), result);
            else
            {
                txtContent.AppendText(Environment.NewLine + result);
            }
        }

        private void GetData()
        {
            try
            {
                var i = 1;
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
                        var minerId = monitorService.InsertMiner(miner);
                        SetTextBoxData($"{i}. Sent miner '{miner.Name}' infomation successful at {DateTime.Now} ^_^");
                        linkLabel1.Text = @"http://trau.buiducanh.net/miners/" + minerId;
                    }

                    i++;
                    Thread.Sleep(SLEEP_TIME);
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
            try
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
                    txtContent.Text = string.Empty;
                    thMonitor = new Thread(GetData) { IsBackground = true };
                    thMonitor.Start();
                    txtHost.Enabled = false;
                    txtPort.Enabled = false;
                    btnMonitor.Enabled = false;
                    txtEmail.Enabled = false;
                    btnStop.Enabled = true;
                    lblMonitor.Visible = true;
                    linkLabel1.Visible = true;

                }
                else
                {
                    MessageBox.Show(this, @"Bạn chưa nhập Email !!!", @"Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(File.Exists(DataFilePath).ToString());
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
