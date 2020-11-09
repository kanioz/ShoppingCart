using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using ShoppingCart.Core.Model;
using ShoppingCart.Data;
using ShoppingCart.Service.Interface;

namespace ShoppingCart.API.Infrastructure
{
    public static class InitializeDataExtensions
    {
        public static IHost SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<IShoppingDbContext>();
                var signInManager = services.GetRequiredService<IUserService>();


                dbContext.AddInitializeUserDataAsync(signInManager).Wait();
                dbContext.AddInitializeProductDataAsync().Wait();

            }
            return host;
        }

        public static async Task AddInitializeUserDataAsync(this IShoppingDbContext dbContext, IUserService userService)
        {
            var usersCollection = dbContext.GetCollection<User>();
            if (await usersCollection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                var user = new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "test"
                };
                userService.CreatePassword(user, "pass");
                usersCollection.InsertOne(user);
            }
        }

        public static async Task AddInitializeProductDataAsync(this IShoppingDbContext dbContext)
        {
            var productsCollection = dbContext.GetCollection<Product>();
            if (await productsCollection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                var initialProducts = new List<Product>
                {
                    new Product
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Category = "Computer",
                        ProductName = "Apple Macbook Pro 13",
                        StockAmount = 5,
                        Price = 1500
                    },
                    new Product
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Category = "Computer",
                        ProductName = "Dell XPS 15",
                        StockAmount = 7,
                        Price = 1800
                    }
                };
                productsCollection.InsertMany(initialProducts);
            }
        }

    }
}
