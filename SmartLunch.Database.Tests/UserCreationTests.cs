using Moq;
using SmartLunch.Api.Dtos;
using SmartLunch.Api.Mapping;
using SmartLunch.Services;

namespace SmartLunch.Database.Tests
{
    public class UserCreationTests(TestDatabaseFixture fixture)
        : IClassFixture<TestDatabaseFixture>
    {
        public TestDatabaseFixture Fixture { get; } = fixture;

        [Fact]
        public async Task CreateUserIfNotExistingAsync_CreateUser_AddUserToDatabase()
        {
            //Arrange
            using var context = TestDatabaseFixture.CreateContext();
            context.Database.BeginTransaction();

            var serviceProviderStub = new Mock<IServiceProvider>();
            var stubHttpClientFactory = new Mock<IHttpClientFactory>();

            var claimsDto = new ClaimsDto("test@example.com", "Test");

            var userService = new UserCreation(serviceProviderStub.Object,
                stubHttpClientFactory.Object);

            //Act
            await userService.CreateUserIfNotExistingAsync(claimsDto.ToUserCreationDto());

            context.ChangeTracker.Clear();

            //Assert
            bool doesContainUser =
                context.Users.Any(user => user.Email == "test@example.com");

            Assert.True(doesContainUser);

        }
    }
}
