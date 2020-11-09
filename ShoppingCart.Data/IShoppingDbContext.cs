using MongoDB.Driver;

namespace ShoppingCart.Data
{
    public interface IShoppingDbContext
    {
        IMongoCollection<TDocument> GetCollection<TDocument>();
    }
}
