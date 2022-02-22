using Microsoft.AspNetCore.Mvc;
using order_book_backend.Model;
using System.Net;

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

        public OrderBookController(ILogger<OrderBookController> logger)
        {
            _logger = logger;
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
                    return Ok(await httpResponse.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(OrderBookController)}.{nameof(OrderBookController.Get)} - An error occurred: {ex}");
            }

            return BadRequest();
        }
    }
}