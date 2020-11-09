using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ShoppingCart.API;
using ShoppingCart.Core.DTO.Auth;
using Xunit;
using Newtonsoft.Json;
using ShoppingCart.Core.Model;

namespace ShoppingCart.Test.Controller.V1
{
    public class UserControllerTests : ControllerTestsBase
    {
        public UserControllerTests(ShoppingCartApiFactory<Startup> factory, string version = "v1") : base(factory, version)
        {

        }

        [Fact]
        public virtual async Task Get_Token_Authorized()
        {
            // Arrange

            var user = new AuthenticateRequest
            {
                Username = "test",
                Password = "pass"
            };
            var userContent = new StringContent(JsonConvert.SerializeObject(user), System.Text.Encoding.UTF8, "application/json");
            var httpResponse = await Client.PostAsync($"/api/{Version}/user/authorize", userContent);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var tokenModel = JsonConvert.DeserializeObject<AuthenticateResponse>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);

            Assert.NotNull(tokenModel.JwtToken);
            Assert.True(tokenModel.ExpiresIn > 0);
            Assert.True(tokenModel.Type == "bearer");
        }

        [Fact]
        public virtual async Task Refresh_Token()
        {
            // Arrange

            await AuthorizeAsync();
            var httpResponse = await Client.PostAsync($"/api/{Version}/user/refresh-token", null);
            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var tokenModel = JsonConvert.DeserializeObject<AuthenticateResponse>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);

            Assert.NotNull(tokenModel.JwtToken);
            Assert.True(tokenModel.ExpiresIn > 0);
            Assert.True(tokenModel.Type == "bearer");
        }

        [Fact]
        public virtual async Task Revoke_Token()
        {
            // Arrange

            await AuthorizeAsync();

            var revokeTokenRequest = new RevokeTokenRequest
            {
                Token = ""
            };

            var revokeTokenRequestContent = new StringContent(JsonConvert.SerializeObject(revokeTokenRequest), System.Text.Encoding.UTF8, "application/json");

            var httpResponse = await Client.PostAsync($"/api/{Version}/user/revoke-token", revokeTokenRequestContent);
            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var tokenRevokeModel = JsonConvert.DeserializeObject<RevokeTokenResponse>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);

            Assert.NotNull(tokenRevokeModel.Message);
            Assert.True(tokenRevokeModel.Message == "Token revoked");
        }

        [Fact]
        public virtual async Task Get_Users()
        {
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.GetAsync($"/api/{Version}/user");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userList = JsonConvert.DeserializeObject<List<User>>(stringResponse);

            //Assert

            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(userList);
        }

        [Fact]
        public virtual async Task Get_UserById()
        {
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.GetAsync($"/api/{Version}/user/");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userList = JsonConvert.DeserializeObject<List<User>>(stringResponse);
            // Assert
            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(userList);

            // Arrange
            httpResponse = await Client.GetAsync($"/api/{Version}/user/{userList.FirstOrDefault().Id}");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(stringResponse);
            // Assert
            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(user);
        }

        [Fact]
        public virtual async Task Get_User_RefreshTokens()
        {
            // Arrange
            await AuthorizeAsync();
            var httpResponse = await Client.GetAsync($"/api/{Version}/user/");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userList = JsonConvert.DeserializeObject<List<User>>(stringResponse);
            // Assert
            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(userList);

            // Arrange
            httpResponse = await Client.GetAsync($"/api/{Version}/user/{userList.FirstOrDefault().Id}/refresh-tokens");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var userRefreshTokens = JsonConvert.DeserializeObject<List<RefreshToken>>(stringResponse);
            // Assert
            Assert.True(httpResponse.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(userRefreshTokens);
        }
    }
}
