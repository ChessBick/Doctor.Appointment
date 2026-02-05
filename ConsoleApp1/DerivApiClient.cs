using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DerivVolatility75EA
{
    public class DerivApiClient
    {
        private readonly string _apiToken;
        private readonly string _appId;
        private ClientWebSocket _webSocket;
        private readonly string _wsUrl;
        private CancellationTokenSource _cancellationTokenSource;

        public DerivApiClient(string apiToken, string appId)
        {
            _apiToken = apiToken;
            _appId = appId;
            _wsUrl = $"wss://ws.derivws.com/websockets/v3?app_id={appId}";
        }

        public async Task ConnectAsync()
        {
            _webSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            await _webSocket.ConnectAsync(new Uri(_wsUrl), _cancellationTokenSource.Token);
        }

        public async Task<AuthResult> AuthorizeAsync()
        {
            var request = new
            {
                authorize = _apiToken
            };

            var response = await SendRequestAsync(request);
            var authData = JsonSerializer.Deserialize<JsonElement>(response);

            return new AuthResult
            {
                Email = authData.GetProperty("authorize").GetProperty("email").GetString(),
                Balance = authData.GetProperty("authorize").GetProperty("balance").GetDouble(),
                Currency = authData.GetProperty("authorize").GetProperty("currency").GetString()
            };
        }

        public async Task SubscribeToTicksAsync(string symbol, Action<TickData> onTickReceived)
        {
            var request = new
            {
                ticks = symbol,
                subscribe = 1
            };

            await SendAsync(JsonSerializer.Serialize(request));

            // Start listening for responses
            _ = Task.Run(async () =>
            {
                var buffer = new byte[8192];
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), 
                        _cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var data = JsonSerializer.Deserialize<JsonElement>(message);

                        if (data.TryGetProperty("tick", out var tickElement))
                        {
                            var tick = new TickData
                            {
                                Symbol = tickElement.GetProperty("symbol").GetString(),
                                Quote = tickElement.GetProperty("quote").GetDouble(),
                                Epoch = tickElement.GetProperty("epoch").GetInt64()
                            };

                            onTickReceived?.Invoke(tick);
                        }
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public async Task<double> GetBalanceAsync()
        {
            var request = new { balance = 1 };
            var response = await SendRequestAsync(request);
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            return data.GetProperty("balance").GetProperty("balance").GetDouble();
        }

        public async Task<TradeResult> PlaceTradeAsync(TradeRequest trade)
        {
            var request = new
            {
                buy = 1,
                price = trade.Amount,
                parameters = new
                {
                    contract_type = trade.TradeType == "CALL" ? "CALL" : "PUT",
                    symbol = trade.Symbol,
                    duration = trade.Duration,
                    duration_unit = trade.DurationType,
                    basis = trade.Basis,
                    amount = trade.Amount
                }
            };

            var response = await SendRequestAsync(request);
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            var buy = data.GetProperty("buy");

            return new TradeResult
            {
                ContractId = buy.GetProperty("contract_id").GetInt64(),
                Price = buy.GetProperty("buy_price").GetDouble()
            };
        }

        private async Task<string> SendRequestAsync(object request)
        {
            await SendAsync(JsonSerializer.Serialize(request));
            return await ReceiveAsync();
        }

        private async Task SendAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource.Token);
        }

        private async Task<string> ReceiveAsync()
        {
            var buffer = new byte[8192];
            var result = await _webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                _cancellationTokenSource.Token);

            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket?.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing",
                    CancellationToken.None);
            }
            _webSocket?.Dispose();
            _cancellationTokenSource?.Cancel();
        }
    }

    public class AuthResult
    {
        public string Email { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
    }

    public class TickData
    {
        public string Symbol { get; set; }
        public double Quote { get; set; }
        public long Epoch { get; set; }
    }

    public class TradeRequest
    {
        public string Symbol { get; set; }
        public string TradeType { get; set; } // "CALL" or "PUT"
        public double Amount { get; set; }
        public int Duration { get; set; }
        public string DurationType { get; set; }
        public string Basis { get; set; }
    }

    public class TradeResult
    {
        public long ContractId { get; set; }
        public double Price { get; set; }
    }
}