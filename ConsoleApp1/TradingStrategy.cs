using System;
using System.Threading.Tasks;

namespace DerivVolatility75EA
{
    public class TradingStrategy
    {
        private readonly TechnicalAnalysis _technicalAnalysis;
        private readonly RiskManager _riskManager;
        private DateTime _lastTradeTime = DateTime.MinValue;
        private readonly TimeSpan _minTimeBetweenTrades = TimeSpan.FromMinutes(5);

        public TradingStrategy(TechnicalAnalysis technicalAnalysis, RiskManager riskManager)
        {
            _technicalAnalysis = technicalAnalysis;
            _riskManager = riskManager;
        }

        public Task<TradeSignal> AnalyzeMarketAsync(string symbol)
        {
            var indicators = _technicalAnalysis.GetCurrentIndicators();
            var signal = new TradeSignal { ShouldTrade = false };

            // Time filter - avoid overtrading
            if (DateTime.UtcNow - _lastTradeTime < _minTimeBetweenTrades)
                return Task.FromResult(signal);

            // Strategy: RSI + Bollinger Bands + Trend + Price Action
            
            // BULLISH SIGNAL CONDITIONS
            if (IsBullishSetup(indicators))
            {
                signal.ShouldTrade = true;
                signal.Direction = "CALL";
                signal.EntryPrice = indicators.CurrentPrice;
                signal.StopLoss = indicators.BollingerLower;
                signal.TakeProfit = indicators.CurrentPrice + 
                    (indicators.CurrentPrice - indicators.BollingerLower) * 2;
                signal.Reason = "RSI Oversold + Bullish Trend + Price at Lower BB";
                
                _lastTradeTime = DateTime.UtcNow;
            }
            // BEARISH SIGNAL CONDITIONS
            else if (IsBearishSetup(indicators))
            {
                signal.ShouldTrade = true;
                signal.Direction = "PUT";
                signal.EntryPrice = indicators.CurrentPrice;
                signal.StopLoss = indicators.BollingerUpper;
                signal.TakeProfit = indicators.CurrentPrice - 
                    (indicators.BollingerUpper - indicators.CurrentPrice) * 2;
                signal.Reason = "RSI Overbought + Bearish Trend + Price at Upper BB";
                
                _lastTradeTime = DateTime.UtcNow;
            }

            return Task.FromResult(signal);
        }

        private bool IsBullishSetup(MarketIndicators indicators)
        {
            return indicators.RSI < 30 && // Oversold
                   indicators.Trend == "BULLISH" &&
                   indicators.CurrentPrice <= indicators.BollingerLower * 1.001 && // Near lower band
                   indicators.EMA20 > indicators.EMA50 && // Bullish crossover
                   _technicalAnalysis.IsBullishEngulfing(); // Price action confirmation
        }

        private bool IsBearishSetup(MarketIndicators indicators)
        {
            return indicators.RSI > 70 && // Overbought
                   indicators.Trend == "BEARISH" &&
                   indicators.CurrentPrice >= indicators.BollingerUpper * 0.999 && // Near upper band
                   indicators.EMA20 < indicators.EMA50 && // Bearish crossover
                   _technicalAnalysis.IsBearishEngulfing(); // Price action confirmation
        }
    }

    public class TradeSignal
    {
        public bool ShouldTrade { get; set; }
        public string Direction { get; set; } // "CALL" or "PUT"
        public double EntryPrice { get; set; }
        public double StopLoss { get; set; }
        public double TakeProfit { get; set; }
        public string Reason { get; set; }
    }
}