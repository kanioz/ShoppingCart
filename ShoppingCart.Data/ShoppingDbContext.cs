using System;
using System.Linq;
using MongoDB.Driver;
using ShoppingCart.Core.Attributes;
using ShoppingCart.Core.Configuration;

namespace ShoppingCart.Data
{
    public class ShoppingDbContext : IShoppingDbContext
    {
        private IMongoDatabase Db { get; set; }
        private MongoClient MongoClient { get; set; }
        public IClientSessionHandle Session { get; set; }

        public ShoppingDbContext(IDatabaseSettings configuration)
        {
            MongoClient = new MongoClient(configuration.ConnectionString);
            Db = MongoClient.GetDatabase(configuration.DatabaseName);
        }
        public IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            return Db.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }
        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }
    }
}
