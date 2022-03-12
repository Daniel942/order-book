using Microsoft.EntityFrameworkCore;
using order_book_backend.Data;
using order_book_backend.Extensions;
using order_book_backend.Model;
using System.Net;
using System.Text.Json;

namespace order_book_backend.Services
{
    public class OrderBookService : IOrderBookService
    {
        private static readonly HttpClient _httpClient = new();

        private readonly OrderBookContext _context;

        public OrderBookService(OrderBookContext context)
        {
            _context = context;
        }

        public List<OrderBook> GetAll()
        {
            return _context.OrderBook
                .OrderByDescending(o => o.Timestamp)
                .ToList();
        }

        public async Task<OrderBook?> GetAsync(string currencyPair, bool storeOrderBook = true)
        {
            using HttpResponseMessage httpResponse = await _httpClient.GetAsync($"https://www.bitstamp.net/api/v2/order_book/{currencyPair}");
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

                OrderBook orderBook = new();

                // Set unique ID
                orderBook.ID = Guid.NewGuid().ToString("N");

                orderBook.Timestamp = response.Timestamp;
                orderBook.Asks = response.Asks.Select(ask => new OrderBookAsk { Price = ask[0], Amount = ask[1] }).ToList();
                orderBook.Bids = response.Bids.Select(bid => new OrderBookBid { Price = bid[0], Amount = bid[1] }).ToList();

                // Store order book
                if (storeOrderBook)
                {
                    _context.Add(orderBook);
                    await _context.SaveChangesAsync();
                }

                return orderBook;
            }

            return null;
        }

        public OrderBook? GetCached(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return _context.OrderBook
                .Include(o => o.Asks.OrderBy(ask => ask.Price))
                .Include(o => o.Bids.OrderByDescending(bid => bid.Price))
                .AsNoTracking()
                .FirstOrDefault(o => o.ID == id);
        }
    }
}
