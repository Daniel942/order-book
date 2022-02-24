using order_book_backend.Model;

namespace order_book_backend.Services
{
    public interface IOrderBookService
    {
        public void Add(OrderBookResponse orderBook);
        public List<OrderBookResponse> GetAll();
        public OrderBookResponse Get(string id);
    }
}
