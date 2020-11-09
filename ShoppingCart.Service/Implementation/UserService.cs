using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using ShoppingCart.Core.Authorization;
using ShoppingCart.Core.DTO.Auth;
using ShoppingCart.Core.Model;
using ShoppingCart.Data.Repository;
using ShoppingCart.Service.Interface;

namespace ShoppingCart.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IMongoRepository<User> _repository;
        private readonly IAppSettings _appSettings;

        public UserService(IMongoRepository<User> repository, IAppSettings appSettings)
        {
            _repository = repository;
            _appSettings = appSettings;
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return _repository.FilterByAsync(c => true);
        }

        public Task<User> GetByIdAsync(string userId)
        {
            return _repository.FindByIdAsync(userId);
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress)
        {
            var user = await _repository.FindOneAsync(x => x.Username == model.Username);

            if (user == null) return null;

            if (user.HashedPassword != HashedPassword(model.Password, user.Salt))
                return null;

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);
            if (user.RefreshTokens == null)
                user.RefreshTokens = new List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);
            await _repository.ReplaceOneAsync(user);

            return new AuthenticateResponse(jwtToken, refreshToken.Token,"bearer", _appSettings.Expires);
        }

        public async Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            var user = await _repository.FindOneAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) return null;

            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            await _repository.ReplaceOneAsync(user);
            var jwtToken = GenerateJwtToken(user);

            return new AuthenticateResponse(jwtToken, newRefreshToken.Token, "bearer", _appSettings.Expires);
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
        {
            var user = await _repository.FindOneAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _repository.ReplaceOneAsync(user);

            return true;
        }

        public User CreatePassword(User user, string password)
        {
            user.Salt = CreateSalt();
            user.HashedPassword = HashedPassword(password, user.Salt);
            return user;
        }

        #region JWT Helper Methods

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion

        #region Password Helper Methods

        private string HashedPassword(string password, string salt)
        {
            return Convert.ToBase64String(
                KeyDerivation
                    .Pbkdf2(
                        password,
                        Encoding.UTF8.GetBytes(salt),
                        KeyDerivationPrf.HMACSHA1,
                        10000,
                        256 / 8
                    )
            );
        }

        private string CreateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        #endregion

    }
}
