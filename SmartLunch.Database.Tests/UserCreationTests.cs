using Moq;
using SmartLunch.Services;
using System.Security.Claims;

namespace SmartLunch.Database.Tests
{
    public class UserCreationTests : IClassFixture<TestDatabaseFixture>
    {
        public TestDatabaseFixture Fixture { get; }

        public UserCreationTests(TestDatabaseFixture fixture) 
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task CreateUserIfNotExistingAsync_CreateUser_AddUserToDatabase()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var serviceProviderStub = new Mock<IServiceProvider>();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.GivenName, "Test"),
                new Claim(ClaimTypes.MobilePhone, "123456789")
            };

            var userService = new UserCreation(serviceProviderStub.Object);

            //Act
            await userService.CreateUserIfNotExistingAsync(claims);

            context.ChangeTracker.Clear();

            //Assert
            bool doesContainUser = 
                context.Users.Any(user => user.Email == "test@example.com");

            Assert.True(doesContainUser);

        }
    }
}
