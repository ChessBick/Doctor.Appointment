using System.Collections.Generic;

namespace DerivVolatility75EA
{
    public class RiskManager
    {
        private readonly double _riskPercentage;
        private double _accountBalance;
        private readonly Dictionary<long, double> _openTrades;

        public RiskManager(double riskPercentage, double initialBalance)
        {
            _riskPercentage = riskPercentage;
            _accountBalance = initialBalance;
            _openTrades = new Dictionary<long, double>();
        }

        public double CalculatePositionSize(double balance, double stopLoss)
        {
            _accountBalance = balance;
            var riskAmount = balance * (_riskPercentage / 100);
            
            // For binary options, position size is the stake amount
            // Keep it between 1% and 5% of balance
            var minStake = balance * 0.01;
            var maxStake = balance * 0.05;
            
            return System.Math.Max(minStake, System.Math.Min(riskAmount, maxStake));
        }

        public bool CanOpenNewTrade(int maxOpenTrades)
        {
            return _openTrades.Count < maxOpenTrades;
        }

        public void RegisterTrade(long contractId, double amount)
        {
            _openTrades[contractId] = amount;
        }

        public void CloseTrade(long contractId)
        {
            _openTrades.Remove(contractId);
        }

        public double GetTotalExposure()
        {
            double total = 0;
            foreach (var trade in _openTrades.Values)
            {
                total += trade;
            }
            return total;
        }
    }
}