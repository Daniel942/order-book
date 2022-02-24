using Microsoft.AspNetCore.Mvc;
using order_book_backend.Extensions;
using order_book_backend.Model;
using order_book_backend.Services;
using System.Net;
using System.Text.Json;

namespace order_book_backend.Controllers
{
    [ApiController]
    public class OrderBookController : ControllerBase
    {
        private static readonly string[] SupportedCurrencyPairs = new[]
        {
            "btcusd", "btceur", "btcgbp", "btcpax", "gbpusd", "gbpeur", "eurusd", "xrpusd", "xrpeur", "xrpbtc", "xrpgbp", "xrppax", "ltcbtc",
            "ltcusd", "ltceur", "ltcgbp", "ethbtc", "ethusd", "etheur", "ethgbp", "ethpax", "bchusd", "bcheur", "bchbtc", "bchgbp", "paxusd",
            "paxeur", "paxgbp", "xlmbtc", "xlmusd", "xlmeur", "xlmgbp", "linkusd", "linkeur", "linkgbp", "linkbtc", "linketh", "omgusd", "omgeur",
            "omggbp", "omgbtc", "usdcusd", "usdceur", "btcusdc", "ethusdc", "eth2eth", "aaveusd", "aaveeur", "aavebtc", "batusd", "bateur", "batbtc",
            "umausd", "umaeur", "umabtc", "daiusd", "kncusd", "knceur", "kncbtc", "mkrusd", "mkreur", "mkrbtc", "zrxusd", "zrxeur", "zrxbtc",
            "gusdusd", "algousd", "algoeur", "algobtc", "audiousd", "audioeur", "audiobtc", "crvusd", "crveur", "crvbtc", "snxusd", "snxeur",
            "snxbtc", "uniusd", "unieur", "unibtc", "yfiusd", "yfieur", "yfibtc", "compusd", "compeur", "compbtc", "grtusd", "grteur", "usdtusd",
            "usdteur", "usdcusdt", "btcusdt", "ethusdt", "xrpusdt", "eurteur", "eurtusd", "maticusd", "maticeur", "sushiusd", "sushieur", "chzusd",
            "chzeur", "enjusd", "enjeur", "hbarusd", "hbareur", "alphausd", "alphaeur", "axsusd", "axseur", "fttusd", "ftteur", "sandusd", "sandeur",
            "storjusd", "storjeur", "adausd", "adaeur", "adabtc", "fetusd", "feteur", "rgtusd", "rgteur", "sklusd", "skleur", "celusd", "celeur",
            "slpusd", "slpeur", "sxpusd", "sxpeur", "sgbusd", "sgbeur", "dydxusd", "dydxeur", "ftmusd", "ftmeur", "ampusd", "ampeur", "galausd",
            "galaeur", "perpusd", "perpeur"
        };

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
        public async Task<IActionResult> Get([FromQuery]OrderBookRequest request)
        {
            // Invalid request or currency pair
            if (request == null || string.IsNullOrEmpty(request.CurrencyPair))
            {
                _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - Request or currency pair is invalid.");
                return BadRequest();
            }

            // Unsupported currency pair
            if (!SupportedCurrencyPairs.Contains(request.CurrencyPair))
            {
                string errorMessage = $"Currency pair '{request.CurrencyPair}' is not supported.";
                _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - {errorMessage}");
                return Problem(errorMessage, statusCode: (int)HttpStatusCode.BadRequest);
            }

            try
            {
                using HttpClient client = new();
                using HttpResponseMessage httpResponse = await client.GetAsync($"https://www.bitstamp.net/api/v2/order_book/{request.CurrencyPair}");
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    OrderBookResponse response = JsonSerializer.Deserialize<OrderBookResponse>(await httpResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Not enough data
                    if (response == null || response.Asks.IsEmpty() || response.Bids.IsEmpty())
                    {
                        _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - Not enough data to send.");
                        return NoContent();
                    }

                    // Show only up to first 100 asks and first 100 bids
                    response.Asks = response.Asks.Count >= 100 ? response.Asks.GetRange(0, 100) : response.Asks;
                    response.Bids = response.Bids.Count >= 100 ? response.Bids.GetRange(0, 100) : response.Bids;

                    // Set unique ID
                    response.ID = Guid.NewGuid().ToString("N");

                    // Set audit log
                    _orderBookService.Add(response);

                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - An error occurred: {e}");
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("api/getorderbookbyid")]
        public async Task<IActionResult> GetByID(string id)
        {
            // Invalid request or currency pair
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning($"{nameof(OrderBookController)}.{nameof(OrderBookController.GetByID)} - ID is missing.");
                return BadRequest();
            }

            try
            {
                OrderBookResponse orderBook = _orderBookService.Get(id);
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
        public async Task<IActionResult> GetAuditLog()
        {
            List<OrderBookResponse> auditLog = _orderBookService.GetAll();
            if (!auditLog.IsEmpty())
            {
                return Ok(auditLog.Select(orderBook => new { id = orderBook.ID, timestamp = orderBook.Timestamp }));
            }

            return Ok(Array.Empty<string>());
        }
    }
}