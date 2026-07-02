using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EMS.IntegrationTests.Controllers
{
    public class DepartmentControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public DepartmentControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
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
            var response = await client.GetAsync("/Department");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Departments", content);
            Assert.Contains("Technology", content);
        }

        [Fact]
        public async Task POST_Create_AsAdmin_RedirectsToIndex()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Login as Admin
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "admin@workforcehub.com"),
                new KeyValuePair<string, string>("Password", "Admin@123")
            });
            await client.PostAsync("/Account/Login", formContent);

            // Create new Department DTO
            var newDeptContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "Customer Success"),
                new KeyValuePair<string, string>("Code", "CS")
            });

            // Act
            var response = await client.PostAsync("/Department/Create", newDeptContent);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Department", response.Headers.Location?.ToString());
        }
    }
}
