using Core;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using View.Controllers;
using View.Models;

namespace View.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<UserManager<Driver>> _mock;
        private readonly Mock<SignInManager<Driver>> _smock;

        public UserControllerTests()
        {
            _mock = new Mock<UserManager<Driver>>(Mock.Of<IUserStore<Driver>>(), null, null, null, null, null, null, null, null);
            _mock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Driver());
            _mock.Setup(userManager => userManager.AddClaimAsync(It.IsAny<Driver>(), It.IsAny<Claim>()))
            .ReturnsAsync(new IdentityResult());
            _mock.Setup(userManager => userManager.UpdateAsync(It.IsAny<Driver>()))
            .ReturnsAsync(new IdentityResult());
            _mock.Setup(userManager => userManager.CreateAsync(It.IsAny<Driver>(), It.IsAny<string>()))
            .Returns(Task.FromResult(IdentityResult.Success));

            _smock = new Mock<SignInManager<Driver>>(_mock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<Driver>>(),
                null,
                null,
                null,
                null);

            _smock.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

            _smock.Setup(s => s.SignInAsync(It.IsAny<Driver>(), It.IsAny<bool>(), It.IsAny<string>()))
            .Verifiable();
        }

        private ILogger<UserController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<UserController>();
        }

        private UserController GetController()
        {
            return new UserController(_mock.Object, _smock.Object, GetLogger());
        }

        [Fact]
        public async Task RegisterRedirects()
        {
            var userController = GetController();

            var result = await userController.Register(new RegisterViewModel
            {
                Email = "test@gmail.com",
                FirstName = "",
                LastName = "",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!",
                RememberMe = true
            }) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public async Task LoginRedirects()
        {
            var userController = GetController();

            var result = await userController.Login(new LoginViewModel
            {
                Email = "test@gmail.com",
                Password = "Pass123!",
                RememberMe = true
            }) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }
    }
}