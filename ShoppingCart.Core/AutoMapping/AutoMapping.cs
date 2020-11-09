using AutoMapper;
using ShoppingCart.Core.DTO.Auth;
using ShoppingCart.Core.Model;

namespace ShoppingCart.Core.AutoMapping
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<User, UserDto>();
        }
    }
}
