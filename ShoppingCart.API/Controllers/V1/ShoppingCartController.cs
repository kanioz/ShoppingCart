using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.API.Controllers.Base;
using ShoppingCart.Core.DTO.ShoppingCart;
using ShoppingCart.Service.Interface;

namespace ShoppingCart.API.Controllers.V1
{
    public class ShoppingCartController : BaseV1Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var shoppingCart = await _shoppingCartService.GetByIdAsync(id);
            if (shoppingCart == null) return NotFound();

            return Ok(shoppingCart);
        }

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetItems(string id)
        {
            var shoppingCart = await _shoppingCartService.GetByIdAsync(id);
            if (shoppingCart == null) return NotFound();

            return Ok(shoppingCart.ShoppingCartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var shoppingCart = await _shoppingCartService.AddShoppingCart();
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                base.LogError<ShoppingCartController>(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("{id}/add-item")]
        public async Task<IActionResult> AddItem(string id, [FromBody] ShoppingCartAddRequest model)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.AddShoppingCartItem(id, model.ProductId, model.Quantity);
                if (shoppingCart == null)
                    return NotFound("Shopping Cart Item not found in the basket");
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                base.LogError<ShoppingCartController>(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}/change-item-quantity")]
        public async Task<IActionResult> ChangeItemQuantity(string id, [FromBody] ShoppingCartChangeItemQuantityRequest model)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.ChangeShoppingCartItemQuantityAsync(id, model.ShoppingCartItemId, model.Quantity);
                if (shoppingCart == null)
                    return NotFound("Shopping Cart Item not found");
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                base.LogError<ShoppingCartController>(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("{id}/delete-item")]
        public async Task<IActionResult> RemoveItemFromShoppingCart(string id, [FromBody] RemoveShoppingCartItemRequest model)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.RemoveShoppingCartItemAsync(id, model.ShoppingCartItemId);
                if (shoppingCart == null)
                    return NotFound("Shopping Cart Item not found");
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                base.LogError<ShoppingCartController>(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
