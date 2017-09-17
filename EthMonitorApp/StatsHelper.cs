using System.IO;
using System.Net;
using EthMonitorApp.MonitorService;
using Newtonsoft.Json;

namespace EthMonitorApp
{
    public class StatsHelper
    {
        private static WebClient client = new WebClient();

        public static bool CheckEthWallet(string wallet)
        {
            // Check Wallet
            //https://api.ethermine.org/miner/deoco/currentStats
            var url = $"/miner/{wallet}/currentStats";
            var minerStats = JsonConvert.DeserializeObject<MinerStats>(client.DownloadString("https://api.ethermine.org" + url));
            return minerStats.status != "ERROR";
        }

        public static IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
        {
            return new IPEndPoint(IPAddress.Any, 5000);
        }

        public static string GetApiData(string requestUriUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(requestUriUrl);
            request.ServicePoint.BindIPEndPointDelegate = BindIPEndPointCallback;
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        public static MinerInfo GetStatsFromEthermine(MinerInfo minerInfo)
        {
            // History
            string url = $"/miner/{minerInfo.Wallet}/history";
            var data = GetApiData("https://api.ethermine.org" + url);
            var historyStats = JsonConvert.DeserializeObject<HistoryReturnData>(data);
            minerInfo.History = historyStats.data.ToArray();

            // currentStats
            url = $"/miner/{minerInfo.Wallet}/currentStats";
            data = GetApiData("https://api.ethermine.org" + url);
            var minerStats = JsonConvert.DeserializeObject<StatsReturnData>(data);
            minerInfo.Stats = minerStats.data;

            // Rounds
            url = $"/miner/{minerInfo.Wallet}/rounds";
            data = GetApiData("https://api.ethermine.org" + url);
            var rounds = JsonConvert.DeserializeObject<RoundReturnData>(data);
            minerInfo.Rounds = rounds.data.ToArray();

            // Payouts
            url = $"/miner/{minerInfo.Wallet}/payouts";
            data = GetApiData("https://api.ethermine.org" + url);
            var payouts = JsonConvert.DeserializeObject<PayoutReturnData>(data);
            minerInfo.Payouts = payouts.data.ToArray();

            // Settings
            url = $"/miner/{minerInfo.Wallet}/settings";
            data = GetApiData("https://api.ethermine.org" + url);
            MinerSettings settings;
            if (!data.Contains("NO DATA"))
            {
                settings = JsonConvert.DeserializeObject<SettingsReturnData>(data).data;
            }
            else
            {
                settings = new MinerSettings
                {
                    minPayout = 1,
                };
            }
            minerInfo.Settings = settings;
            return minerInfo;
        }
    }
}
