using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShoppingCart.API;
using ShoppingCart.Core.DTO.Auth;
using Xunit;

namespace ShoppingCart.Test.Controller
{
    public class ControllerTestsBase : IClassFixture<ShoppingCartApiFactory<Startup>>
    {
        private ShoppingCartApiFactory<Startup> _factory;
        protected HttpClient Client;
        protected string Version { get; set; }
        public ControllerTestsBase(ShoppingCartApiFactory<Startup> factory, string version)
        {
            _factory = factory;
            Client = factory.CreateClient();
            Version = version;
        }

        protected async Task AuthorizeAsync()
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new AuthenticateRequest { Username = "test", Password = "pass" }), Encoding.UTF8, "application/json");
            var authResponse = await Client.PostAsync($"/api/{Version}/user/authorize", httpContent);
            var response = JsonConvert.DeserializeObject<AuthenticateResponse>(authResponse.Content.ReadAsStringAsync().Result);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.JwtToken);
            authResponse.Headers.TryGetValues("Set-Cookie", out var setCookies);
            var enumerable = setCookies as string[] ?? setCookies.ToArray();
            Client.DefaultRequestHeaders.Add("Cookie", enumerable);
        }
    }
}
