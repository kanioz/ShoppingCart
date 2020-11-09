using System.Collections.Generic;
using ShoppingCart.Core.Attributes;
using ShoppingCart.Core.Model.Base;

namespace ShoppingCart.Core.Model
{
    [BsonCollection("Users")]
    public class User: Document
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
