using Moq;
using SmartLunch.Api.Dtos;
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

            var claimsDto = new ClaimsDto("test@example.com", "Test");

            var userService = new UserCreation(serviceProviderStub.Object);

            //Act
            await userService.CreateUserIfNotExistingAsync(claimsDto);

            context.ChangeTracker.Clear();

            //Assert
            bool doesContainUser =
                context.Users.Any(user => user.Email == "test@example.com");

            Assert.True(doesContainUser);

        }
    }
}
