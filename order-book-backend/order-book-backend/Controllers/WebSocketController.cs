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

        [HttpGet("/ws")]
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

        private async Task Echo(WebSocket webSocket)
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
                        OrderBookResponse? response = await _orderBookService.GetAsync(currencyPair, false);
                        if (response == null)
                        {
                            response = new OrderBookResponse
                            {
                                Asks = new List<List<string>>()
                            };
                        }

                        string message = JsonSerializer.Serialize(response.Asks);
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

                Thread.Sleep(1000);
            }

            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
        }
    }
}