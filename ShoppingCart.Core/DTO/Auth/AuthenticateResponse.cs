using Newtonsoft.Json;

namespace ShoppingCart.Core.DTO.Auth
{
    public class AuthenticateResponse
    {
        public string JwtToken { get; set; }
        public string Type { get; set; }
        public int ExpiresIn { get; set; }

        [JsonIgnore] 
        public string RefreshToken { get; set; }

        public AuthenticateResponse(string jwtToken, string refreshToken, string type, int expiresIn)
        {
            Type = type;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
    }
}
