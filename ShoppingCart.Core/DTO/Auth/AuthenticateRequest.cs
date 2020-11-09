using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Core.DTO.Auth
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
