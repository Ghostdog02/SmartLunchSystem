using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using SmartLunch;
using SmartLunch.Controllers;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmartLunch.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IServiceProvider> stubServiceProvider;
        private readonly AccountController controller;

        public AccountControllerTests()
        {
            stubServiceProvider = new Mock<IServiceProvider>();
            controller = new AccountController(stubServiceProvider.Object);
        }

        [Fact]
        public void GetClaims_WithProvidedEmailRoleName_ReturnIEnumarableWithEmailRoleName()
        {
            //Arrange
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "Admin")
            });

            var principal = new ClaimsPrincipal(identity);
            var authenticateResult = AuthenticateResult.Success(
                new AuthenticationTicket(principal, GoogleDefaults.AuthenticationScheme));

            //Act
            IEnumerable<Claim> claims = controller.GetClaims(authenticateResult);

            //Assert
            Assert.NotNull(claims);
            Assert.Equal(3, claims.Count());
            Assert.Equal("Test User", claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal("test@example.com", claims.First(c => c.Type == ClaimTypes.Email).Value);
            Assert.Equal("Admin", claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public void Login_WithValidCredentials_LoginUser()
        {
            //Arrange




            //Act

            //Assert
        }
    }
}
