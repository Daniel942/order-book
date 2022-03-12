using Microsoft.AspNetCore.Mvc;
using order_book_backend.Helpers;
using order_book_backend.Model;
using order_book_backend.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace order_book_backend.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private readonly IOrderBookService _orderBookService;

        public WebSocketController(
            ILogger<WebSocketController> logger,
            IOrderBookService orderBookService)
        {
            _logger = logger;
            _orderBookService = orderBookService;
        }

        [HttpGet]
        [Route("ws/getorderbookasks")]
        public async Task GetAsks()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket, true);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [HttpGet]
        [Route("ws/getorderbook")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task Echo(WebSocket webSocket, bool onlyAsks = false)
        {
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            // Retrieve order book every second until close is called
            while (!receiveResult.CloseStatus.HasValue)
            {
                string currencyPair = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

                // Invalid request or currency pair
                if (string.IsNullOrEmpty(currencyPair))
                {
                    _logger.LogWarning($"{nameof(WebSocketController)}.{nameof(WebSocketController.Echo)} - Currency pair is invalid.");
                    break;
                }
                // Unsupported currency pair
                else if (!BitstampConfig.SupportedCurrencyPairs.Contains(currencyPair))
                {
                    _logger.LogWarning($"{nameof(WebSocketController)}.{nameof(WebSocketController.Echo)} - Currency pair '{currencyPair}' is not supported.");
                    break;
                }
                else
                {
                    try
                    {
                        OrderBook? response = await _orderBookService.GetAsync(currencyPair, !onlyAsks);
                        if (response == null)
                        {
                            response = new OrderBook
                            {
                                Asks = new List<OrderBookAsk>(),
                                Bids = new List<OrderBookBid>()
                            };
                        }

                        string message = onlyAsks ? JsonSerializer.Serialize(response.Asks) : JsonSerializer.Serialize(response);

                        byte[] bytes = Encoding.Default.GetBytes(message);
                        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"{nameof(WebSocketController)}.{nameof(WebSocketController.Echo)} - An error occurred: {e}");
                        break;
                    }

                }

                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (onlyAsks)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(3000);
                }
            }

            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
        }
    }
}