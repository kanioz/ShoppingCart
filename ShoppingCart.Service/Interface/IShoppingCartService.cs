using System.Threading.Tasks;

namespace ShoppingCart.Service.Interface
{
    public interface IShoppingCartService
    {
        Task<Core.Model.ShoppingCart> GetByIdAsync(string id);

        Task<Core.Model.ShoppingCart> AddShoppingCart();
        Task<Core.Model.ShoppingCart> AddShoppingCartItem(string shoppingCartId, string productId, int quantity);

        Task<Core.Model.ShoppingCart> ChangeShoppingCartItemQuantityAsync(string shoppingCartId, string shoppingCartItemId, int quantity);
        Task<Core.Model.ShoppingCart> RemoveShoppingCartItemAsync(string shoppingCartId, string shoppingCartItemId);
        Task<Core.Model.ShoppingCart> RemoveAllShoppingCartItemsAsync(string shoppingCartId);
    }
}
