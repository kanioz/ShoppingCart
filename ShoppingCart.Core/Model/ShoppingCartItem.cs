﻿using ShoppingCart.Core.Model.Base;

namespace ShoppingCart.Core.Model
{
    public class ShoppingCartItem : Document
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotalPrice { get; set; }
        public Product Product { get; set; }
    }
}