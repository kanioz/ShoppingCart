using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Core.Model;

namespace ShoppingCart.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
    }
}
