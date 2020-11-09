namespace ShoppingCart.Core.DTO.ShoppingCart
{
    public class ShoppingCartChangeItemQuantityRequest
    {
        public string ShoppingCartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
