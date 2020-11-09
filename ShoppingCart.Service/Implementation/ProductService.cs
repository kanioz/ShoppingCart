using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Core.Model;
using ShoppingCart.Data.Repository;
using ShoppingCart.Service.Interface;

namespace ShoppingCart.Service.Implementation
{
    public class ProductService: IProductService
    {
        private readonly IMongoRepository<Product> _productRepository;

        public ProductService(IMongoRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return _productRepository.FilterByAsync(c => true);
        }
    }
}
