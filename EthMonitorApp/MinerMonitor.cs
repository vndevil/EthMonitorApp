using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using EthMonitorApp.MonitorService;
using Fizzler.Systems.HtmlAgilityPack;
using Newtonsoft.Json;

namespace EthMonitorApp
{
    public partial class MinerMonitor : Form
    {
        #region Fields

        const int SLEEP_TIME = 30000;
        private Thread thMonitor;
        WebClient client = new WebClient();
        MonitorServicesSoapClient monitorService = new MonitorServicesSoapClient("MonitorServicesSoap");
        private string DataFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data.dll";

        #endregion

        #region Contructors

        public MinerMonitor()
        {
            InitializeComponent();

            thMonitor = new Thread(GetData) { IsBackground = true };
            MaximizeBox = false;
        }

        #endregion

        #region Methods

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

        public void EnableApp(bool isStart)
        {
            txtContent.Text = string.Empty;

            if (isStart)
            {
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                btnMonitor.Enabled = false;
                txtWallet.Enabled = false;
                txtEmail.Enabled = false;
                btnStop.Enabled = true;
            }
            else
            {
                txtHost.Enabled = true;
                txtPort.Enabled = true;
                txtWallet.Enabled = true;
                txtEmail.Enabled = true;
                btnMonitor.Enabled = true;
                btnStop.Enabled = false;
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
                            Wallet = txtWallet.Text.ToLower().Trim(),
                            EmailId = txtEmail.Text.ToLower().Trim(),
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

                        linkLabel1.Text = @"http://ethmonitor.net/miners/" + minerId.ToLower();
                    }

                    SetTextBoxData($"{i}. Sent infomation successful at {DateTime.Now}");
                    Thread.Sleep(SLEEP_TIME);
                    i++;
                }
            }
            catch (Exception ex)
            {
                thMonitor?.Abort();
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Form Events

        private void MinerMonitor_Load(object sender, EventArgs e)
        {
            Text = @"EthMonitor.NET | buiducanh.net";
            txtEmail.Select();

            if (File.Exists(DataFilePath) && !string.IsNullOrEmpty(File.ReadAllText(DataFilePath)))
            {
                var obj = JsonConvert.DeserializeObject<MonitorObject>(ConvertHelper.ToString(File.ReadAllText(DataFilePath)));
                txtEmail.Text = obj.Email;
                txtWallet.Text = obj.Wallet;
                EnableApp(true);
                thMonitor.Start();
            }
            else
            {
                EnableApp(false);
                thMonitor.Abort();
            }
        }

        private void MinerMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            thMonitor?.Abort();
        }

        #endregion

        #region User Events

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(linkLabel1.Text));
        }

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            try
            {
                var obj = new MonitorObject
                {
                    Wallet = StringHelper.RemoveSign4VietnameseString(txtWallet.Text.ToLower().Trim()),
                    Email = StringHelper.RemoveSign4VietnameseString(txtEmail.Text.ToLower().Trim())
                };

                if (!string.IsNullOrEmpty(obj.Email) || !string.IsNullOrEmpty(obj.Wallet))
                {
                    // Ghi nhớ Email
                    if (!File.Exists(DataFilePath))
                    {
                        File.Create(DataFilePath).Close();
                    }
                    File.WriteAllText(DataFilePath, JsonConvert.SerializeObject(obj));

                    // Monitoring
                    thMonitor = new Thread(GetData) { IsBackground = true };
                    thMonitor.Start();

                    EnableApp(true);
                }
                else
                {
                    EnableApp(false);
                    txtEmail.Select();
                    MessageBox.Show(this, @"Bạn chưa nhập Email hoặc Wallet !!!", @"Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            EnableApp(false);
            thMonitor.Abort();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            var aboutForm = new About { MaximizeBox = false };
            aboutForm.ShowDialog();
        }

        #endregion
    }
}
