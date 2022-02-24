using order_book_backend.Model;

namespace order_book_backend.Services
{
    public class OrderBookService : IOrderBookService
    {
        private readonly List<OrderBookResponse> _orderBooks = new List<OrderBookResponse>();

        public void Add(OrderBookResponse orderBook)
        {
            _orderBooks.Add(orderBook);
        }

        public List<OrderBookResponse> GetAll()
        {
            return _orderBooks;
        }

        public OrderBookResponse Get(string id)
        {
            return _orderBooks.FirstOrDefault(orderBook => orderBook.ID == id);
        }
    }
}
