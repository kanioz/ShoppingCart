namespace ShoppingCart.Core.DTO.ShoppingCart
{
    public class ShoppingCartAddResponse
    {
        public Model.ShoppingCart ShoppingCart { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
