﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using DevGuard.Controllers;

namespace DevGuard.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public void GetClaims_WithProvidedEmailRoleName_ReturnsIEnumarableWithEmailRoleName()
        {
            //Arrange
            var stubServiceProvider = new Mock<IServiceProvider>();
            var stubHttpClientFactory = new Mock<IHttpClientFactory>();

            AccountController controller = new(stubServiceProvider.Object,
                stubHttpClientFactory.Object);

            var identity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.Email, "test@example.com"),
                    new(ClaimTypes.Role, "Admin"),
                ]
            );

            var principal = new ClaimsPrincipal(identity);
            var authenticateResult = AuthenticateResult.Success(
                new AuthenticationTicket(principal, GoogleDefaults.AuthenticationScheme)
            );

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
        public void GetClaims_NullArgument_ReturnsNullReferenceException()
        {
            //Arrange
            var stubServiceProvider = new Mock<IServiceProvider>();
            var stubHttpClientFactory = new Mock<IHttpClientFactory>();

            AccountController controller = new(stubServiceProvider.Object,
                stubHttpClientFactory.Object);

            //Act and Assert
            Assert.Throws<NullReferenceException>(() => controller.GetClaims(null!));
        }

        [Fact]
        public void GetClaims_WithNoClaimsProvided_ReturnsArgumentException()
        {
            //Arrange
            var stubServiceProvider = new Mock<IServiceProvider>();
            var stubHttpClientFactory = new Mock<IHttpClientFactory>();

            AccountController controller = new(stubServiceProvider.Object,
                stubHttpClientFactory.Object);

            var identity = new ClaimsIdentity();

            var principal = new ClaimsPrincipal(identity);
            var authenticateResult = AuthenticateResult.Success(
                new AuthenticationTicket(principal, GoogleDefaults.AuthenticationScheme)
            );

            //Act and Assert
            Assert.Throws<ArgumentException>(() => controller.GetClaims(authenticateResult));
        }

        [Fact]
        public async Task Login_RedirectsToGoogleAuthentication()
        {
            // Arrange
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var urlHelperFactoryStub = new Mock<IUrlHelperFactory>();
            var urlHelperMock = new Mock<IUrlHelper>();

            urlHelperMock
                .Setup(url => url.Action(It.IsAny<UrlActionContext>()))
                .Returns("/Account/GoogleResponse");

            urlHelperFactoryStub
                .Setup(factory => factory.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelperMock.Object);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IAuthenticationService>(
                authenticationServiceMock.Object
            );
            serviceCollection.AddSingleton<IUrlHelperFactory>(urlHelperFactoryStub.Object);
            serviceCollection.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };

            var stubServiceProvider = new Mock<IServiceProvider>();
            var stubHttpClientFactory = new Mock<IHttpClientFactory>();

            AccountController controller = new(stubServiceProvider.Object,
                stubHttpClientFactory.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act
            await controller.Login();

            // Assert
            authenticationServiceMock.Verify(
                auth =>
                    auth.ChallengeAsync(
                        It.IsAny<HttpContext>(),
                        GoogleDefaults.AuthenticationScheme,
                        It.Is<AuthenticationProperties>(props =>
                            props.RedirectUri == "/Account/GoogleResponse"
                        )
                    ),
                Times.Once
            );
        }
    }
}
