using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Core.DTO.Auth;
using ShoppingCart.Core.Model;

namespace ShoppingCart.Service.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(string userId);
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);
        Task<bool> RevokeTokenAsync(string token, string ipAddress);
        User CreatePassword(User user, string password);
    }
}
