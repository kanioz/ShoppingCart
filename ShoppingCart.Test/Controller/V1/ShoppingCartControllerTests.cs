using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShoppingCart.API;
using ShoppingCart.Core.DTO.ShoppingCart;
using ShoppingCart.Core.Model;
using Xunit;

namespace ShoppingCart.Test.Controller.V1
{
    public class ShoppingCartControllerTests : ControllerTestsBase
    {
        public ShoppingCartControllerTests(ShoppingCartApiFactory<Startup> factory, string version = "v1") : base(factory, version)
        {

        }

        [Fact]
        public virtual async Task Initialize_ShoppingCart()
        {
            #region Initialize Shopping Cart
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart", null);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart); 
            #endregion
        }

        [Fact]
        public virtual async Task Get_ShoppingCartById()
        {
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart", null);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);

            // Arrange
            httpResponse = await Client.GetAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);

        }

        [Fact]
        public virtual async Task Get_ShoppingCartItemsById()
        {
            #region Initialize Shopping Cart
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart", null);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            #endregion

            #region Get Products
            // Arrange

            var httpProductResponse = await Client.GetAsync($"/api/{Version}/product");

            // Must be successful.
            httpProductResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringProductResponse = await httpProductResponse.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<List<Product>>(stringProductResponse);

            //Assert

            Assert.True(httpProductResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(productList);
            Assert.True(productList.Any());
            #endregion

            #region New Item to Shopping Cart 
            // Arrange
            var product = productList.FirstOrDefault(c => c.StockAmount > 0);

            var addRequest = new ShoppingCartAddRequest
            {
                ProductId = product?.Id,
                Quantity = 1
            };
            var shoppingCartAddItemContent = new StringContent(JsonConvert.SerializeObject(addRequest), System.Text.Encoding.UTF8, "application/json");
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/add-item", shoppingCartAddItemContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            Assert.NotEmpty(shoppingCart.ShoppingCartItems);
            Assert.True(shoppingCart.ShoppingCartItems.Count == 1);
            Assert.True(shoppingCart.TotalPrice == addRequest.Quantity * product?.Price);
            #endregion
            
            #region Get Shopping Cart Items
            // Arrange
            httpResponse = await Client.GetAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/items");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCartItems);
            #endregion

            #region Clear Shopping Cart
            // Arrange
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/delete-all-items", null);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.Empty(shoppingCart.ShoppingCartItems);
            #endregion

        }

        [Fact]
        public virtual async Task Add_ShoppingCartItemById()
        {
            await AuthorizeAsync();

            #region Initialize Shopping Cart
            // Arrange

            var httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart", null);


            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();


            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);


            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            #endregion

            #region Get Products
            // Arrange

            var httpProductResponse = await Client.GetAsync($"/api/{Version}/product");

            // Must be successful.
            httpProductResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringProductResponse = await httpProductResponse.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<List<Product>>(stringProductResponse);

            //Assert

            Assert.True(httpProductResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(productList);
            Assert.True(productList.Any());
            #endregion

            #region New Item to Shopping Cart 
            // Arrange
            var product = productList.FirstOrDefault(c => c.StockAmount > 0);

            var addRequest = new ShoppingCartAddRequest
            {
                ProductId = product?.Id,
                Quantity = 1
            };
            var shoppingCartAddItemContent = new StringContent(JsonConvert.SerializeObject(addRequest), System.Text.Encoding.UTF8, "application/json");
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/add-item", shoppingCartAddItemContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            Assert.NotEmpty(shoppingCart.ShoppingCartItems);
            Assert.True(shoppingCart.ShoppingCartItems.Count == 1);
            Assert.True(shoppingCart.TotalPrice == addRequest.Quantity * product?.Price);
            #endregion

            #region Clear Shopping Cart
            // Arrange
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/delete-all-items", null);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.Empty(shoppingCart.ShoppingCartItems);
            #endregion
        }

        [Fact]
        public virtual async Task ChangeQuantity_ShoppingCartItemById()
        {
            await AuthorizeAsync();

            #region Initialize Shopping Cart
            // Arrange

            var httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart", null);


            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();


            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);


            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            #endregion

            #region Get Products
            // Arrange

            var httpProductResponse = await Client.GetAsync($"/api/{Version}/product");

            // Must be successful.
            httpProductResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringProductResponse = await httpProductResponse.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<List<Product>>(stringProductResponse);

            //Assert

            Assert.True(httpProductResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(productList);
            Assert.True(productList.Any());
            #endregion

            #region Add New Item To Shopping Cart
            // Arrange
            var product = productList.FirstOrDefault(c => c.StockAmount > 0);

            var addRequest = new ShoppingCartAddRequest
            {
                ProductId = product?.Id,
                Quantity = 1
            };
            var shoppingCartAddItemContent = new StringContent(JsonConvert.SerializeObject(addRequest), System.Text.Encoding.UTF8, "application/json");
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/add-item", shoppingCartAddItemContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            Assert.NotEmpty(shoppingCart.ShoppingCartItems);
            Assert.True(shoppingCart.ShoppingCartItems.Count == 1);
            Assert.True(shoppingCart.TotalPrice == addRequest.Quantity * product?.Price); 
            #endregion

            #region Change Quantity of Shopping Cart Item
            // Arrange


            var changeItemQuantityRequestRequest = new ShoppingCartChangeItemQuantityRequest
            {
                ShoppingCartItemId = shoppingCart.ShoppingCartItems.FirstOrDefault()?.Id,
                Quantity = 2
            };
            var shoppingCartChangeItemQuantityContent = new StringContent(JsonConvert.SerializeObject(changeItemQuantityRequestRequest), System.Text.Encoding.UTF8, "application/json");
            httpResponse = await Client.PutAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/change-item-quantity", shoppingCartChangeItemQuantityContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            Assert.NotEmpty(shoppingCart.ShoppingCartItems);
            Assert.True(shoppingCart.ShoppingCartItems.Count == 1);
            Assert.True(shoppingCart.ShoppingCartItems.FirstOrDefault()?.Quantity == 2);
            Assert.True(shoppingCart.TotalPrice == changeItemQuantityRequestRequest.Quantity * product?.Price);


            #endregion

            #region Clear Shopping Cart
            // Arrange
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/delete-all-items", null);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.Empty(shoppingCart.ShoppingCartItems); 
            #endregion

        }

        [Fact]
        public virtual async Task Delete_ShoppingCartItemById()
        {
            #region Initialize Shopping Cart
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart", null);
            var httpProductResponse = await Client.GetAsync($"/api/{Version}/product");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();
            httpProductResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            var stringProductResponse = await httpProductResponse.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<List<Product>>(stringProductResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);

            Assert.True(httpProductResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(productList);
            Assert.True(productList.Any());
            #endregion

            #region Add New Item To Shopping Cart
            // Arrange
            var product = productList.FirstOrDefault(c => c.StockAmount > 0);

            var addRequest = new ShoppingCartAddRequest
            {
                ProductId = product?.Id,
                Quantity = 1
            };
            var shoppingCartAddItemContent = new StringContent(JsonConvert.SerializeObject(addRequest), System.Text.Encoding.UTF8, "application/json");
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/add-item", shoppingCartAddItemContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            Assert.NotEmpty(shoppingCart.ShoppingCartItems);
            Assert.True(shoppingCart.ShoppingCartItems.Count == 1);
            #endregion

            #region Delete Shopping Cart
            // Arrange
            var removeItem = new RemoveShoppingCartItemRequest
            {
                ShoppingCartItemId = shoppingCart.ShoppingCartItems.FirstOrDefault()?.Id
            };
            var removeItemContent = new StringContent(JsonConvert.SerializeObject(removeItem), System.Text.Encoding.UTF8, "application/json");
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/delete-item", removeItemContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.Empty(shoppingCart.ShoppingCartItems);
            #endregion

        }

        [Fact]
        public virtual async Task Clear_ShoppingCartItemsById()
        {
            #region Initialize Shopping Cart
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart", null);
            var httpProductResponse = await Client.GetAsync($"/api/{Version}/product");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();
            httpProductResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            var stringProductResponse = await httpProductResponse.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<List<Product>>(stringProductResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);

            Assert.True(httpProductResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(productList);
            Assert.True(productList.Any());
            #endregion

            #region Add New Item To Shopping Cart
            // Arrange
            var product = productList.FirstOrDefault(c => c.StockAmount > 0);

            var addRequest = new ShoppingCartAddRequest
            {
                ProductId = product?.Id,
                Quantity = 1
            };
            var shoppingCartAddItemContent = new StringContent(JsonConvert.SerializeObject(addRequest), System.Text.Encoding.UTF8, "application/json");
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/add-item", shoppingCartAddItemContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(shoppingCart);
            Assert.NotEmpty(shoppingCart.ShoppingCartItems);
            Assert.True(shoppingCart.ShoppingCartItems.Count == 1);
            #endregion

            #region Clear Shopping Cart
            // Arrange
            httpResponse = await Client.PostAsync($"/api/{Version}/shoppingcart/{shoppingCart.Id}/delete-all-items", null);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            shoppingCart = JsonConvert.DeserializeObject<Core.Model.ShoppingCart>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.Empty(shoppingCart.ShoppingCartItems); 
            #endregion

        }

    }
}
