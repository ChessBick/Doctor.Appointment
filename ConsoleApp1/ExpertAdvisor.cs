using System;
using System.Threading;
using System.Threading.Tasks;

namespace DerivVolatility75EA
{
    public class ExpertAdvisor
    {
        private readonly TradingConfig _config;
        private readonly DerivApiClient _apiClient;
        private readonly TechnicalAnalysis _technicalAnalysis;
        private readonly RiskManager _riskManager;
        private readonly TradingStrategy _strategy;
        private CancellationTokenSource _cancellationTokenSource;

        public ExpertAdvisor(TradingConfig config)
        {
            _config = config;
            _apiClient = new DerivApiClient(config.ApiToken, config.AppId);
            _technicalAnalysis = new TechnicalAnalysis();
            _riskManager = new RiskManager(config.RiskPercentage, config.InitialBalance);
            _strategy = new TradingStrategy(_technicalAnalysis, _riskManager);
        }

        public async Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Connect to Deriv API
                Console.WriteLine("Connecting to Deriv API...");
                await _apiClient.ConnectAsync();

                // Authorize user
                Console.WriteLine("Authenticating...");
                var authResult = await _apiClient.AuthorizeAsync();
                Console.WriteLine($"Logged in as: {authResult.Email}");
                Console.WriteLine($"Balance: {authResult.Balance} {authResult.Currency}\n");

                // Subscribe to tick stream
                Console.WriteLine($"Subscribing to {_config.Symbol} tick stream...");
                await _apiClient.SubscribeToTicksAsync(_config.Symbol, OnTickReceived);

                Console.WriteLine("Expert Advisor is running. Press Ctrl+C to stop.\n");
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    _cancellationTokenSource.Cancel();
                };

                await Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nShutting down Expert Advisor...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await _apiClient.DisconnectAsync();
                Console.WriteLine("Disconnected from Deriv API.");
            }
        }

        private async void OnTickReceived(TickData tick)
        {
            try
            {
                // Add tick to analysis buffer
                _technicalAnalysis.AddTick(tick);

                // Check if we have enough data
                if (!_technicalAnalysis.HasEnoughData())
                    return;

                // Analyze market conditions
                var signal = await _strategy.AnalyzeMarketAsync(_config.Symbol);

                if (signal.ShouldTrade && _riskManager.CanOpenNewTrade(_config.MaxOpenTrades))
                {
                    await ExecuteTradeAsync(signal);
                }

                // Display current analysis
                DisplayAnalysis(tick, signal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing tick: {ex.Message}");
            }
        }

        private async Task ExecuteTradeAsync(TradeSignal signal)
        {
            var currentBalance = await _apiClient.GetBalanceAsync();
            var positionSize = _riskManager.CalculatePositionSize(currentBalance, signal.StopLoss);

            var trade = new TradeRequest
            {
                Symbol = _config.Symbol,
                TradeType = signal.Direction,
                Amount = positionSize,
                Duration = 5, // 5 ticks
                DurationType = "t",
                Basis = "stake"
            };

            Console.WriteLine($"\n>>> TRADE SIGNAL: {signal.Direction} <<<");
            Console.WriteLine($"Entry: {signal.EntryPrice:F5}");
            Console.WriteLine($"Stop Loss: {signal.StopLoss:F5}");
            Console.WriteLine($"Take Profit: {signal.TakeProfit:F5}");
            Console.WriteLine($"Position Size: {positionSize:F2}");
            Console.WriteLine($"Reason: {signal.Reason}\n");

            try
            {
                var result = await _apiClient.PlaceTradeAsync(trade);
                Console.WriteLine($"Trade placed successfully. Contract ID: {result.ContractId}");
                
                _riskManager.RegisterTrade(result.ContractId, positionSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to place trade: {ex.Message}");
            }
        }

        private void DisplayAnalysis(TickData tick, TradeSignal signal)
        {
            var indicators = _technicalAnalysis.GetCurrentIndicators();
            
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"Price: {tick.Quote:F5} | RSI: {indicators.RSI:F2} | " +
                         $"Trend: {indicators.Trend} | Signal: {signal.Direction ?? "NONE"}".PadRight(80));
        }
    }
}