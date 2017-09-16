namespace EthMonitorApp
{
    public class MonitorObject
    {
        public string Email;
        public string Wallet;
        public string Name;
    }

    public class Data
    {
        public int time { get; set; }
        public int lastSeen { get; set; }
        public int reportedHashrate { get; set; }
        public double currentHashrate { get; set; }
        public int validShares { get; set; }
        public int invalidShares { get; set; }
        public int staleShares { get; set; }
        public double averageHashrate { get; set; }
        public int activeWorkers { get; set; }
        public long unpaid { get; set; }
        public object unconfirmed { get; set; }
        public double coinsPerMin { get; set; }
        public double usdPerMin { get; set; }
        public double btcPerMin { get; set; }
    }

    public class MinerStats
    {
        public string status { get; set; }
        public Data data { get; set; }
    }
}