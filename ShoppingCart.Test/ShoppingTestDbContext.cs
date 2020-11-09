using Moq;
using ShoppingCart.Core.Configuration;
using ShoppingCart.Data;


namespace ShoppingCart.Test
{
    public class ShoppingTestDbContext : ShoppingDbContext
    {
        private IMock<IDatabaseSettings> _mockDatabaseSettings;

        public ShoppingTestDbContext(IMock<IDatabaseSettings> mockDatabaseSettings) :base(mockDatabaseSettings.Object)
        {
            _mockDatabaseSettings = new Mock<IDatabaseSettings>();
        }
    }
}
