using System.Collections.Generic;
using EthMonitorApp.MonitorService;

namespace EthMonitorApp
{
    public class MonitorObject
    {
        public string Email;
        public string Wallet;
        public string Name;
    }

    public class MinerStats
    {
        public string status { get; set; }
    }

    public class SettingsReturnData
    {
        public string status { get; set; }
        public MinerSettings data { get; set; }
    }

    public class StatsReturnData
    {
        public string status { get; set; }
        public MinerStatistics data { get; set; }
    }

    public class HistoryReturnData
    {
        public string status { get; set; }
        public List<MinerHistory> data { get; set; }
    }

    public class RoundReturnData
    {
        public string status { get; set; }
        public List<MinerRounds> data { get; set; }
    }

    public class PayoutReturnData
    {
        public string status { get; set; }
        public List<MinerPayouts> data { get; set; }
    }
}