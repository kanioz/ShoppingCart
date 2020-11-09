using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ShoppingCart.API;
using Xunit;
using Newtonsoft.Json;
using ShoppingCart.Core.Model;

namespace ShoppingCart.Test.Controller.V1
{
    public class ProductControllerTests : ControllerTestsBase
    {
        public ProductControllerTests(ShoppingCartApiFactory<Startup> factory, string version = "v1") : base(factory, version)
        {

        }

        [Fact]
        public virtual async Task Get_Products()
        {
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.GetAsync($"/api/{Version}/product");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<List<User>>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(productList);
        }
    }
}
