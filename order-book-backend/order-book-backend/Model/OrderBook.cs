using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace order_book_backend.Model
{
    public class OrderBookRequest
    {
        public string CurrencyPair { get; set; }
    }

    public class OrderBookResponse
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("timestamp")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Timestamp { get; set; }
        [JsonPropertyName("bids")]
        public List<List<string>> Bids { get; set; }
        [JsonPropertyName("asks")]
        public List<List<string>> Asks { get; set; }
    }

    public class OrderBook
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        [JsonPropertyName("bids")]
        public ICollection<OrderBookBid> Bids { get; set; }
        [JsonPropertyName("asks")]
        public ICollection<OrderBookAsk> Asks { get; set; }
    }

    public class OrderBookBid
    {
        [JsonPropertyName("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        [JsonPropertyName("price")]
        public string Price { get; set; }
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }

    public class OrderBookAsk
    {
        [JsonPropertyName("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        [JsonPropertyName("price")]
        public string Price { get; set; }
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }
}
