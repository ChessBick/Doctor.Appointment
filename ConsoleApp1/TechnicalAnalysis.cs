using System;
using System.Collections.Generic;
using System.Linq;

namespace DerivVolatility75EA
{
    public class TechnicalAnalysis
    {
        private readonly List<TickData> _priceHistory;
        private readonly int _requiredDataPoints = 100;

        public TechnicalAnalysis()
        {
            _priceHistory = new List<TickData>();
        }

        public void AddTick(TickData tick)
        {
            _priceHistory.Add(tick);
            
            // Keep only last 200 ticks
            if (_priceHistory.Count > 200)
                _priceHistory.RemoveAt(0);
        }

        public bool HasEnoughData() => _priceHistory.Count >= _requiredDataPoints;

        public MarketIndicators GetCurrentIndicators()
        {
            var prices = _priceHistory.Select(t => t.Quote).ToList();
            
            return new MarketIndicators
            {
                RSI = CalculateRSI(prices, 14),
                EMA20 = CalculateEMA(prices, 20),
                EMA50 = CalculateEMA(prices, 50),
                BollingerUpper = CalculateBollingerBands(prices, 20, 2).Upper,
                BollingerLower = CalculateBollingerBands(prices, 20, 2).Lower,
                BollingerMiddle = CalculateBollingerBands(prices, 20, 2).Middle,
                CurrentPrice = prices.Last(),
                Trend = DetermineTrend(prices)
            };
        }

        public double CalculateRSI(List<double> prices, int period)
        {
            if (prices.Count < period + 1)
                return 50;

            var gains = new List<double>();
            var losses = new List<double>();

            for (int i = prices.Count - period; i < prices.Count; i++)
            {
                var change = prices[i] - prices[i - 1];
                gains.Add(change > 0 ? change : 0);
                losses.Add(change < 0 ? Math.Abs(change) : 0);
            }

            var avgGain = gains.Average();
            var avgLoss = losses.Average();

            if (avgLoss == 0)
                return 100;

            var rs = avgGain / avgLoss;
            return 100 - (100 / (1 + rs));
        }

        public double CalculateEMA(List<double> prices, int period)
        {
            if (prices.Count < period)
                return prices.Average();

            var multiplier = 2.0 / (period + 1);
            var ema = prices.Take(period).Average();

            for (int i = period; i < prices.Count; i++)
            {
                ema = (prices[i] - ema) * multiplier + ema;
            }

            return ema;
        }

        public (double Upper, double Middle, double Lower) CalculateBollingerBands(
            List<double> prices, int period, double standardDeviations)
        {
            if (prices.Count < period)
                return (0, 0, 0);

            var recentPrices = prices.TakeLast(period).ToList();
            var sma = recentPrices.Average();
            var variance = recentPrices.Sum(p => Math.Pow(p - sma, 2)) / period;
            var stdDev = Math.Sqrt(variance);

            return (
                Upper: sma + (standardDeviations * stdDev),
                Middle: sma,
                Lower: sma - (standardDeviations * stdDev)
            );
        }

        public string DetermineTrend(List<double> prices)
        {
            if (prices.Count < 50)
                return "UNKNOWN";

            var ema20 = CalculateEMA(prices, 20);
            var ema50 = CalculateEMA(prices, 50);

            if (ema20 > ema50)
                return "BULLISH";
            else if (ema20 < ema50)
                return "BEARISH";
            else
                return "SIDEWAYS";
        }

        public bool IsBullishEngulfing()
        {
            if (_priceHistory.Count < 2)
                return false;

            var previous = _priceHistory[_priceHistory.Count - 2].Quote;
            var current = _priceHistory[_priceHistory.Count - 1].Quote;

            return current > previous * 1.001; // 0.1% increase
        }

        public bool IsBearishEngulfing()
        {
            if (_priceHistory.Count < 2)
                return false;

            var previous = _priceHistory[_priceHistory.Count - 2].Quote;
            var current = _priceHistory[_priceHistory.Count - 1].Quote;

            return current < previous * 0.999; // 0.1% decrease
        }
    }

    public class MarketIndicators
    {
        public double RSI { get; set; }
        public double EMA20 { get; set; }
        public double EMA50 { get; set; }
        public double BollingerUpper { get; set; }
        public double BollingerMiddle { get; set; }
        public double BollingerLower { get; set; }
        public double CurrentPrice { get; set; }
        public string Trend { get; set; }
    }
}