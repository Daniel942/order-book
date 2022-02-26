using order_book_backend.Extensions;
using order_book_backend.Model;
using System.Net;
using System.Text.Json;

namespace order_book_backend.Services
{
    public class OrderBookService : IOrderBookService
    {
        private readonly List<OrderBookResponse> _orderBooks = new();

        public List<OrderBookResponse> GetAll()
        {
            return _orderBooks;
        }

        public async Task<OrderBookResponse?> GetAsync(string currencyPair, bool storeOrderBook = true)
        {
            using HttpClient client = new();
            using HttpResponseMessage httpResponse = await client.GetAsync($"https://www.bitstamp.net/api/v2/order_book/{currencyPair}");
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                OrderBookResponse? response = JsonSerializer.Deserialize<OrderBookResponse>(await httpResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Not enough data
                if (response == null || response.Asks.IsEmpty() || response.Bids.IsEmpty())
                {
                    return null;
                }

                // Show only up to first 100 asks and first 100 bids
                response.Asks = response.Asks.Count >= 100 ? response.Asks.GetRange(0, 100) : response.Asks;
                response.Bids = response.Bids.Count >= 100 ? response.Bids.GetRange(0, 100) : response.Bids;

                // Set unique ID
                response.ID = Guid.NewGuid().ToString("N");

                // Store order book
                if (storeOrderBook)
                {
                    _orderBooks.Add(response);
                }

                return response;
            }

            return null;
        }

        public OrderBookResponse? GetCached(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return _orderBooks.FirstOrDefault(orderBook => orderBook.ID == id);
        }
    }
}
