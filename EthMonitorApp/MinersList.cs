using System;
using System.Data;
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
    public partial class MinersList : Form
    {
        #region Fields

        const string APP_VERSION = "1.2";
        /// <summary>
        /// 15 giây
        /// </summary>
        const int SLEEP_TIME_MONITOR = 15000; // Miliseconds
        const int SLEEP_TIME_COMMAND = 60000; // Miliseconds

        private Thread thMonitor, thCommand;
        readonly WebClient client = new WebClient();
        readonly MonitorServicesSoapClient monitorService = new MonitorServicesSoapClient("MonitorServicesSoap");
        private readonly string DataFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data_ETH.dll";

        #endregion

        #region Contructors

        public MinersList()
        {
            InitializeComponent();

            thMonitor = new Thread(SendData) { IsBackground = true };
            thCommand = new Thread(GetCommands) { IsBackground = true };
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
                txtEthash.AppendText(Environment.NewLine + result);
            }
        }

        public void RunningApp(bool isStart)
        {
            if (isStart)
            {
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                btnMonitor.Enabled = false;
                txtName.Enabled = false;
                txtWallet.Enabled = false;
                txtEmail.Enabled = false;
                btnStop.Enabled = true;
            }
            else
            {
                txtHost.Enabled = true;
                txtPort.Enabled = true;
                txtWallet.Enabled = true;
                txtName.Enabled = true;
                txtEmail.Enabled = true;
                btnMonitor.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private bool IsCurrentVersion(out string currentVersion)
        {
            //            // Check version
            //            currentVersion = monitorService.CheckVerion();
            //
            //            // true: đúng phiên bản
            //            // false: sai phiên bản
            //            return APP_VERSION == currentVersion;

            currentVersion = "V-Miner Monitor 1.0";
            return true;
        }

        private void AlertVersion(string currentVersion)
        {
            var message = "Your EthMonitorApp is out of date, new verion is " + currentVersion + ". Click OK to go EthMonitor.NET download new EthMonitorApp version.";
            var caption = "EthMonitorApp Update !!!";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("http://ethmonitor.net"));
            }
            else
            {
                Close();
            }
        }

        private void GetCommands()
        {

            try
            {
                while (true)
                {
                    string currentVersion;
                    if (!IsCurrentVersion(out currentVersion))
                    {
                        if (thMonitor.IsAlive)
                        {
                            thMonitor.Abort();
                            RunningApp(false);
                        }

                        AlertVersion(currentVersion);
                    }
                    Thread.Sleep(SLEEP_TIME_COMMAND);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"ERROR WHEN GetCommands: " + ex.Message, @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendData()
        {
            try
            {
                // New MinerInfo
                var miner = new MinerInfo
                {
                    Wallet = txtWallet.Text.ToLower().Trim(),
                    EmailId = txtEmail.Text.ToLower().Trim(),
                    Name = StringHelper.RemoveSign4VietnameseString(txtName.Text.Trim()),
                    CreatedDate = DateTime.Now,
                    StatisticsDate = DateTime.Now
                };

                // Stats from ethermine.org
                var obj = JsonConvert.DeserializeObject<MonitorObject>(ConvertHelper.ToString(File.ReadAllText(DataFilePath)));
                if (obj != null)
                {
                    // Ghi nhớ thời gian Stats
                    //                    obj.StatsDate = DateTime.Now;
                    //                    File.Delete(DataFilePath);
                    //                    File.Create(DataFilePath).Close();
                    //                    File.WriteAllText(DataFilePath, JsonConvert.SerializeObject(obj));

                    // Monitor Workers
                    var i = 1;
                    while (true)
                    {
                        // Lấy thông tin Stats từ Ethermine
                        var host = txtHost.Text.Replace("http://", string.Empty);
                        var port = txtPort.Text;
                        miner = StatsHelper.GetStatsFromEthermine(miner);
                        var content = client.DownloadString("http://" + host + ":" + port);
                        //var content = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "test.txt");
                        var html = new HtmlAgilityPack.HtmlDocument();

                        html.LoadHtml(content);
                        var doc = html.DocumentNode;
                        // Lấy dữ liệu MinerInfo
                        // Lấy danh sách dòng
                        if (doc?.QuerySelectorAll("tr") != null)
                        {
                            var arrTrs = doc.QuerySelectorAll("tr").ToList();

                            // Xóa cột th
                            arrTrs.RemoveAt(0);

                            var arrWorkers =
                                arrTrs.Select(node => node.QuerySelectorAll("td").ToList()).Select(arrCols => new Worker
                                {
                                    Name = ConvertHelper.ToString(arrCols[0].InnerText),
                                    Ip = ConvertHelper.ToString(arrCols[1].InnerText),
                                    RunningTime = ConvertHelper.ToString(arrCols[2].InnerText),
                                    EthereumStats = ConvertHelper.ToString(arrCols[3].InnerText),
                                    DcrInfo = ConvertHelper.ToString(arrCols[4].InnerText),
                                    GpuTemperature = ConvertHelper.ToString(arrCols[5].InnerText),
                                    Pool = ConvertHelper.ToString(arrCols[6].InnerText),
                                    Version = ConvertHelper.ToString(arrCols[7].InnerText),
                                    Comments = ConvertHelper.ToString(arrCols[8].InnerText),
                                    CreatedDate = DateTime.Now
                                }).ToList();

                            // Bind dữ liệu vào Grid
                            var dt = new DataTable();
                            dt.Columns.Add("Name");
                            dt.Columns.Add("Ip");
                            dt.Columns.Add("RunningTime");
                            dt.Columns.Add("EthereumStats");
                            dt.Columns.Add("DcrInfo");
                            dt.Columns.Add("GpuTemperature");
                            dt.Columns.Add("Pool");
                            dt.Columns.Add("Version");
                            dt.Columns.Add("Comments");
                            dt.Columns.Add("CreatedDate");

                            foreach (var worker in arrWorkers)
                            {
                                var row = dt.NewRow();

                                row["Name"] = worker.Name;
                                row["Ip"] = worker.Ip;
                                row["RunningTime"] = worker.RunningTime;
                                row["EthereumStats"] = worker.EthereumStats;
                                row["DcrInfo"] = worker.DcrInfo;
                                row["GpuTemperature"] = worker.GpuTemperature;
                                row["Pool"] = worker.Pool;
                                row["Version"] = worker.Version;
                                row["Comments"] = worker.Comments;
                                row["CreatedDate"] = worker.CreatedDate;
                                dt.Rows.Add(row);
                            }
                            gvData_ETH.AutoGenerateColumns = false;
                            gvData_ETH.DataSource = dt;
                            gvData_ETH.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);

                            // Gửi dữ liệu lên website
                            miner.Workers = arrWorkers.ToArray();
                            var minerUniqueName = monitorService.SendtMiner(miner);
                            linkLabel1.Text = @"http://ethmonitor.net/miners/" + minerUniqueName.ToLower();

                            SetTextBoxData($"{i}. Sent infomation miner '{miner.Name}' and {arrWorkers.Count} workers to EthMonitor.NET successful at {DateTime.Now}");
                            i++;

                            // Sleep tiến trình
                            Thread.Sleep(SLEEP_TIME_MONITOR);
                        }
                    }
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

        private void MinersList_Load(object sender, EventArgs e)
        {
            Text = lblAppTitle.Text = @"V-Miner Monitor 1.2 | EthMonitor.NET - ZecMonitor.NET";
            txtName.Select();

            string currentVersion;
            if (IsCurrentVersion(out currentVersion))
            {
                if (File.Exists(DataFilePath) && !string.IsNullOrEmpty(File.ReadAllText(DataFilePath)))
                {
                    var obj = JsonConvert.DeserializeObject<MonitorObject>(ConvertHelper.ToString(File.ReadAllText(DataFilePath)));
                    txtName.Text = obj.Name;
                    txtEmail.Text = obj.Email;
                    txtWallet.Text = obj.Wallet;
                    RunningApp(true);
                    thMonitor.Start();
                    thCommand.Start();
                }
                else
                {
                    RunningApp(false);
                    thMonitor.Abort();
                }
            }
            else
            {
                AlertVersion(currentVersion);
            }
        }

        private void MinersList_FormClosed(object sender, FormClosedEventArgs e)
        {
            thMonitor?.Abort();
            thCommand?.Abort();
        }

        #endregion

        #region User Events

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            try
            {
                string currentVersion;
                if (IsCurrentVersion(out currentVersion))
                {
                    var obj = new MonitorObject
                    {
                        Wallet = StringHelper.RemoveSign4VietnameseString(txtWallet.Text.ToLower().Trim()),
                        Email = txtEmail.Text.ToLower().Trim(),
                        Name = txtName.Text.Trim(),
                        CreatedDate = DateTime.Now,
                        StatsDate = DateTime.Now
                    };

                    if (!string.IsNullOrEmpty(obj.Email) && !string.IsNullOrEmpty(obj.Wallet) && !string.IsNullOrEmpty(obj.Name))
                    {
                        if (StatsHelper.CheckEthWallet(obj.Wallet))
                        {
                            // Ghi nhớ Email
                            if (!File.Exists(DataFilePath))
                            {
                                File.Create(DataFilePath).Close();
                            }
                            File.WriteAllText(DataFilePath, JsonConvert.SerializeObject(obj));

                            // Monitoring
                            thMonitor = new Thread(SendData) { IsBackground = true };
                            thMonitor.Start();
                            if (!thCommand.IsAlive)
                            {
                                thCommand = new Thread(GetCommands) { IsBackground = true };
                                thCommand.Start();
                            }
                            RunningApp(true);
                        }
                        else
                        {
                            txtWallet.Select();
                            MessageBox.Show(this, @"Your ETH Wallet not found on Ethermine.org !!!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        RunningApp(false);
                        txtName.Select();
                        MessageBox.Show(this, @"You need to enter your Name, Email and ETH Wallet !!!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    AlertVersion(currentVersion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            RunningApp(false);
            thMonitor.Abort();
            thCommand.Abort();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            var aboutForm = new About { MaximizeBox = false };
            aboutForm.ShowDialog();
        }

        #endregion
    }
}
