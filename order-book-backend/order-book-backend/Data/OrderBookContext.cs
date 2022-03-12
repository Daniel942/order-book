using Microsoft.EntityFrameworkCore;
using order_book_backend.Model;

namespace order_book_backend.Data
{
    public class OrderBookContext : DbContext
    {
        public OrderBookContext(DbContextOptions<OrderBookContext> options) : base(options)
        {

        }

        public DbSet<OrderBook> OrderBook { get; set; }
        public DbSet<OrderBookBid> Bid { get; set; }
        public DbSet<OrderBookAsk> Ask { get; set; }
    }
}
