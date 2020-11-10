using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using ShoppingCart.Core.Model;
using ShoppingCart.Data.Repository;
using ShoppingCart.Service.Interface;

namespace ShoppingCart.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IMongoRepository<Core.Model.ShoppingCart> _repository;
        private readonly IMongoRepository<Product> _productRepository;

        public ShoppingCartService(IMongoRepository<Core.Model.ShoppingCart> repository, IMongoRepository<Product> productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public Task<Core.Model.ShoppingCart> GetByIdAsync(string id)
        {
            return _repository.FindByIdAsync(id);
        }

        public async Task<Core.Model.ShoppingCart> AddShoppingCart()
        {
            var model = new Core.Model.ShoppingCart
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ShoppingCartItems = new List<ShoppingCartItem>()
            };
            await _repository.InsertOneAsync(model);
            return model;
        }

        public async Task<Core.Model.ShoppingCart> AddShoppingCartItem(string shoppingCartId, string productId, int quantity)
        {
            var shoppingCart = await _repository.FindByIdAsync(shoppingCartId);

            if (shoppingCart == null)
                return null;

            var shoppingCartItem = shoppingCart.ShoppingCartItems.SingleOrDefault(c => c.ProductId == productId);
            if (shoppingCartItem == null)
            {
                var product = _productRepository.FindById(productId);

                if (product == null) // product not found
                    return null;

                var productInCarts =
                    await _repository.FilterByAsync(c => c.ShoppingCartItems.Any(f => f.ProductId == productId));
                var totalCartAmount = productInCarts
                    .SelectMany(c => c.ShoppingCartItems.Where(a => a.ProductId == productId))
                    .Sum(f => f.Quantity);

                if (product.StockAmount < totalCartAmount + quantity) // stock amount is not enough
                    return null;

                shoppingCartItem = new ShoppingCartItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Quantity = quantity,
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    ItemTotalPrice = quantity * product.Price,
                    Price = product.Price
                };

                shoppingCart.ShoppingCartItems.Add(shoppingCartItem);
                shoppingCart.TotalPrice = shoppingCart.ShoppingCartItems.Sum(c=> c.ItemTotalPrice);
                await _repository.ReplaceOneAsync(shoppingCart);
                return shoppingCart;
            }

            shoppingCart = await ChangeShoppingCartItemQuantityAsync(shoppingCart, shoppingCartItem, quantity);
            return shoppingCart;
        }

        public async Task<Core.Model.ShoppingCart> ChangeShoppingCartItemQuantityAsync(string shoppingCartId, string shoppingCartItemId, int quantity)
        {
            var shoppingCart = await _repository.FindByIdAsync(shoppingCartId);

            var shoppingCartItem = shoppingCart?.ShoppingCartItems?.SingleOrDefault(c => c.Id.ToString() == shoppingCartItemId);

            if (shoppingCartItem == null)
                return null;

            shoppingCart.ShoppingCartItems.Remove(shoppingCartItem);
            shoppingCart = await ChangeShoppingCartItemQuantityAsync(shoppingCart, shoppingCartItem, quantity);

            return shoppingCart;
        }

        private async Task<Core.Model.ShoppingCart> ChangeShoppingCartItemQuantityAsync(Core.Model.ShoppingCart shoppingCart, ShoppingCartItem shoppingCartItem, int quantity)
        {
            var product = await _productRepository.FindByIdAsync(shoppingCartItem.ProductId);

            if (product == null) // product not found
                return null;

            var productInCarts =
                await _repository.FilterByAsync(c => c.ShoppingCartItems.Any(f => f.ProductId == shoppingCartItem.ProductId));
            var totalCartAmount = productInCarts
                .SelectMany(c => c.ShoppingCartItems.Where(a => a.ProductId == shoppingCartItem.ProductId))
                .Sum(f => f.Quantity);

            if (product.StockAmount < totalCartAmount - shoppingCartItem.Quantity + quantity) // stock amount is not enough
                return null;

            shoppingCartItem.Quantity = quantity;
            shoppingCartItem.ItemTotalPrice = product.Price * shoppingCartItem.Quantity;
            shoppingCartItem.Price = product.Price;

            shoppingCart.ShoppingCartItems.Add(shoppingCartItem);
            shoppingCart.TotalPrice = shoppingCart.ShoppingCartItems.Sum(c => c.ItemTotalPrice);

            await _repository.ReplaceOneAsync(shoppingCart);

            return shoppingCart;
        }
        
        public async Task<Core.Model.ShoppingCart> RemoveShoppingCartItemAsync(string shoppingCartId, string shoppingCartItemId)
        {
            var shoppingCart = await _repository.FindByIdAsync(shoppingCartId);

            var shoppingCartItem = shoppingCart?.ShoppingCartItems?.SingleOrDefault(c => c.Id.ToString() == shoppingCartItemId);

            if (shoppingCartItem == null)
                return null;

            shoppingCart.ShoppingCartItems.Remove(shoppingCartItem);
            shoppingCart.TotalPrice = shoppingCart.ShoppingCartItems.Sum(c => c.ItemTotalPrice);
            await _repository.ReplaceOneAsync(shoppingCart);

            return shoppingCart;
        }

        public async Task<Core.Model.ShoppingCart> RemoveAllShoppingCartItemsAsync(string shoppingCartId)
        {
            var shoppingCart = await _repository.FindByIdAsync(shoppingCartId);
            shoppingCart.ShoppingCartItems.Clear();
            shoppingCart.TotalPrice = 0;
            
            await _repository.ReplaceOneAsync(shoppingCart);

            return shoppingCart;
        }
    }
}
