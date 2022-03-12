using Microsoft.AspNetCore.Mvc;
using order_book_backend.Extensions;
using order_book_backend.Helpers;
using order_book_backend.Model;
using order_book_backend.Services;
using System.Net;

namespace order_book_backend.Controllers
{
    [ApiController]
    public class OrderBookController : ControllerBase
    {
        private readonly ILogger<OrderBookController> _logger;
        private readonly IOrderBookService _orderBookService;

        public OrderBookController(
            ILogger<OrderBookController> logger,
            IOrderBookService orderBookService)
        {
            _logger = logger;
            _orderBookService = orderBookService;
        }

        [HttpGet]
        [Route("api/getorderbook")]
        public async Task<IActionResult> Get([FromQuery] OrderBookRequest request)
        {
            // Invalid request or currency pair
            if (request == null || string.IsNullOrEmpty(request.CurrencyPair))
            {
                _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - Request or currency pair is invalid.");
                return BadRequest();
            }

            // Unsupported currency pair
            if (!BitstampConfig.SupportedCurrencyPairs.Contains(request.CurrencyPair))
            {
                string errorMessage = $"Currency pair '{request.CurrencyPair}' is not supported.";
                _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - {errorMessage}");
                return Problem(errorMessage, statusCode: (int)HttpStatusCode.BadRequest);
            }

            try
            {
                OrderBook? orderBook = await _orderBookService.GetAsync(request.CurrencyPair);
                if (orderBook == null)
                {
                    _logger.LogWarning($"{nameof(OrderBookService)}.{nameof(OrderBookService.GetAsync)} - Not enough data to send.");
                    return NoContent();
                }

                return Ok(orderBook);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - An error occurred: {e}");
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("api/getorderbookbyid")]
        public IActionResult GetByID(string id)
        {
            // Invalid request or currency pair
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.GetByID)} - ID is missing.");
                return BadRequest();
            }

            try
            {
                OrderBook? orderBook = _orderBookService.GetCached(id);
                if (orderBook == null)
                {
                    _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.GetByID)} - ID is invalid.");
                    return BadRequest();
                }

                return Ok(orderBook);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(OrderBookController)}.{nameof(OrderBookController.GetByID)} - An error occurred: {e}");
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("api/getauditlog")]
        public IActionResult GetAuditLog()
        {
            List<OrderBook> auditLog = _orderBookService.GetAll();
            if (!auditLog.IsEmpty())
            {
                return Ok(auditLog.Select(orderBook => new { id = orderBook.ID, timestamp = orderBook.Timestamp }));
            }

            return Ok(Array.Empty<string>());
        }
    }
}