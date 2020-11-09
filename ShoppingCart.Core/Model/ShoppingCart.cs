using System.Collections.Generic;
using ShoppingCart.Core.Attributes;
using ShoppingCart.Core.Model.Base;

namespace ShoppingCart.Core.Model
{
    [BsonCollection("ShoppingCarts")]
    public class ShoppingCart : Document
    {
        public decimal TotalPrice { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
        
    }
}
