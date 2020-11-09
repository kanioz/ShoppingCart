using ShoppingCart.API;

namespace ShoppingCart.Test.Controller.V1
{
    public class ShoppingCartControllerTests : ControllerTestsBase
    {
        public ShoppingCartControllerTests(ShoppingCartApiFactory<Startup> factory, string version = "v1") : base(factory, version)
        {

        }
    }
}
