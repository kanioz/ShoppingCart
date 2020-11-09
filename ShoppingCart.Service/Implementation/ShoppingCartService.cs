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
                ShoppingCartItems = new List<ShoppingCartItem>(),
                TotalPrice = 0
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

                if (product.StockAmount < quantity) // stock amount is not enough
                    return null;

                shoppingCartItem = new ShoppingCartItem
                {
                    Product = product,
                    Id = ObjectId.GenerateNewId().ToString(),
                    Quantity = quantity,
                    ProductId = productId,
                    ItemTotalPrice = quantity * product.Price
                };

                shoppingCart.ShoppingCartItems.Add(shoppingCartItem);
                CalculateTotalPrice(shoppingCart);
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

            shoppingCart = await ChangeShoppingCartItemQuantityAsync(shoppingCart, shoppingCartItem, quantity);

            return shoppingCart;
        }

        private async Task<Core.Model.ShoppingCart> ChangeShoppingCartItemQuantityAsync(Core.Model.ShoppingCart shoppingCart, ShoppingCartItem shoppingCartItem, int quantity)
        {
            var product = await _productRepository.FindByIdAsync(shoppingCartItem.ProductId);

            if (product == null) // product not found
                return null;

            if (product.StockAmount < quantity) // stock amount is not enough
                return null;

            shoppingCartItem.Quantity += quantity;
            shoppingCartItem.ItemTotalPrice = product.Price * shoppingCartItem.Quantity;
            CalculateTotalPrice(shoppingCart);
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
            CalculateTotalPrice(shoppingCart);
            await _repository.ReplaceOneAsync(shoppingCart);

            return shoppingCart;
        }

        private void CalculateTotalPrice(Core.Model.ShoppingCart shoppingCart)
        {
            decimal total = 0;
            foreach (var item in shoppingCart.ShoppingCartItems)
            {
                total += item.ItemTotalPrice;
            }

            shoppingCart.TotalPrice = total;
        }
    }
}
