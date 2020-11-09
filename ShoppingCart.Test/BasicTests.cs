﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using ShoppingCart.API;
using Xunit;

namespace ShoppingCart.Test
{
    public class BasicTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Swagger doc. page
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("/swagger")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}
