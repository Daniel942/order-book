using order_book_backend.Model;

namespace order_book_backend.Services
{
    public interface IOrderBookService
    {
        public List<OrderBookResponse> GetAll();
        public Task<OrderBookResponse?> GetAsync(string currencyPair, bool storeOrderBook = true);
        public OrderBookResponse? GetCached(string id);
    }
}
