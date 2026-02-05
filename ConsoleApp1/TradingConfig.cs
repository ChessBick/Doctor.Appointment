namespace DerivVolatility75EA
{
    public class TradingConfig
    {
        public string ApiToken { get; set; }
        public string AppId { get; set; }
        public string Symbol { get; set; }
        public double RiskPercentage { get; set; }
        public double InitialBalance { get; set; }
        public double TakeProfitMultiplier { get; set; }
        public int MaxOpenTrades { get; set; }
    }
}