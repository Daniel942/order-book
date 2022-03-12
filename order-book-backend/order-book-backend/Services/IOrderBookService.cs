using order_book_backend.Model;

namespace order_book_backend.Services
{
    public interface IOrderBookService
    {
        public List<OrderBook> GetAll();
        public Task<OrderBook?> GetAsync(string currencyPair, bool storeOrderBook = true);
        public OrderBook? GetCached(string id);
    }
}
