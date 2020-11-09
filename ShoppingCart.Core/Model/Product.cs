using ShoppingCart.Core.Attributes;
using ShoppingCart.Core.Model.Base;

namespace ShoppingCart.Core.Model
{
    [BsonCollection("Products")]
    public class Product: Document
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int StockAmount { get; set; }
        public decimal Price { get; set; }
    }
}
