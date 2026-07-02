using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EMS.IntegrationTests.Controllers
{
    public class LeaveRequestControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public LeaveRequestControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GET_Index_AsAdmin_ReturnsSuccess()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as Admin
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "admin@workforcehub.com"),
                new KeyValuePair<string, string>("Password", "Admin@123")
            });
            await client.PostAsync("/Account/Login", formContent);

            // Act
            var response = await client.GetAsync("/LeaveRequest");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Leave Requests", content);
            Assert.Contains("Alice Smith", content);
        }
    }
}
