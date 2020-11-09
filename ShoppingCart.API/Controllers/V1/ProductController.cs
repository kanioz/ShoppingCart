using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.API.Controllers.Base;
using ShoppingCart.Service.Interface;

namespace ShoppingCart.API.Controllers.V1
{
    public class ProductController : BaseV1Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }
    }
}
