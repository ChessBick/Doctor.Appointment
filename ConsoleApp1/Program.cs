using System;
using System.Threading.Tasks;

namespace DerivVolatility75EA
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Deriv Volatility 75 Expert Advisor ===");
            Console.WriteLine("WARNING: Trading involves significant risk. Use at your own discretion.\n");

            // Configuration
            var config = new TradingConfig
            {
                ApiToken = "GhrE694YAtia7s3", // Replace with your API token
                AppId = "64346", // Replace with your App ID from Deriv
                Symbol = "R_75", // Volatility 75 Index
                RiskPercentage = 2.0, // Risk 2% per trade
                InitialBalance = 1000.0,
                TakeProfitMultiplier = 2.0, // Risk:Reward = 1:2
                MaxOpenTrades = 3
            };

            var ea = new ExpertAdvisor(config);
            await ea.StartAsync();
        }
    }
}