using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppingCart.API.Infrastructure;
using ShoppingCart.Core.Configuration;
using ShoppingCart.Data;
using ShoppingCart.Service.Interface;

namespace ShoppingCart.Test
{
    public class ShoppingCartApiFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IShoppingDbContext));
                services.RemoveAll(typeof(IDatabaseSettings));

                var settings = new Mock<DatabaseSettings>();
                settings.Object.ConnectionString = "mongodb://root:Password12345@localhost:27017";
                settings.Object.DatabaseName = "TestShoppingDB";

                services.AddSingleton<IMock<IDatabaseSettings>>(settings);
                services.AddScoped(typeof(IShoppingDbContext), typeof(ShoppingTestDbContext));

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<IShoppingDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<ShoppingCartApiFactory<TStartup>>>();

                try
                {
                    var userService = scopedServices.GetRequiredService<IUserService>();
                    db.AddInitializeUserDataAsync(userService).Wait();
                    db.AddInitializeProductDataAsync().Wait();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                                        "database with test messages. Error: {Message}", ex.Message);
                }
            });
        }
    }
}
