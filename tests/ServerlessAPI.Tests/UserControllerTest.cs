using ServerlessAPI.Entities;
using ServerlessAPI.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ServerlessAPI.Tests
{
    public class UserControllerTest
    {
        private readonly WebApplicationFactory<Program> webApplication;
        public UserControllerTest()
        {
            webApplication = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        //Mock the repository implementation
                        //to remove infra dependencies for Test project
                        services.AddScoped<IUserRepository, MockUserRepository>();
                    });
                });
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        public async Task Call_GetApiUsers_ShouldReturn_LimitedListOfUsers(int limit)
        {
            var client = webApplication.CreateClient();
            var users = await client.GetFromJsonAsync<IList<UserEntity>>($"/api/Users?limit={limit}");

            Assert.NotEmpty(users);
            Assert.Equal(limit, users?.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public async Task Call_GetApiUser_ShouldReturn_BadRequest(int limit)
        {
            var client = webApplication.CreateClient();
            var result = await client.GetAsync($"/api/Users?limit={limit}");

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result?.StatusCode);

        }
    }
}
