using Core;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using View.Controllers;
using View.Models;

namespace View.Tests
{
    public class DriverControllerTests
    {
        private readonly Mock<IDriverRepository> _mock;

        public DriverControllerTests()
        {
            _mock = new Mock<IDriverRepository>();
            _mock.Setup(data => data.Get())
                .ReturnsAsync(new List<Driver>() {
                new Driver
                {
                    Id = "",
                    FirstName = "",
                    LastName = ""
                },
                new Driver
                {
                    Id = "",
                    FirstName = "",
                    LastName = ""
                },
                new Driver
                {
                    Id = "",
                    FirstName = "",
                    LastName = ""
                }
            });
            _mock.Setup(data => data.Get(It.IsAny<string>()))
                .ReturnsAsync(
                new Driver
                {
                    Id = "",
                    FirstName = "",
                    LastName = ""
                }
            );
            _mock.Setup(data => data.Add(It.IsAny<Driver>()))
                .Verifiable();
            _mock.Setup(data => data.Update(It.IsAny<Driver>()))
                .Verifiable();
            _mock.Setup(data => data.Delete(It.IsAny<string>()))
                .Verifiable();

        }

        private ILogger<DriverController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<DriverController>();
        }

        private async Task<ShuttleDbContext> GetDbContext()
        {
            var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ShuttleDbContext>()
            .UseInMemoryDatabase($"Shuttle{Guid.NewGuid()}")
            .UseInternalServiceProvider(serviceProvider)
            .Options;

            var ctx = new ShuttleDbContext(options);
            await ctx.Database.EnsureDeletedAsync();
            return ctx;
        }

        private DriverController GetController()
        {
            return new DriverController(_mock.Object, GetLogger());
        }

        [Fact]
        public async Task IndexHasList()
        {
            var driverController = GetController();

            var result = await driverController.Index() as ViewResult;

            var model = result.ViewData.Model as List<Driver>;

            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task EditRedirects()
        {
            var driverController = GetController();

            var result = await driverController.Edit(new DriverViewModel()) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task DeleteRedirects()
        {
            var driverController = GetController();

            var result = await driverController.DeletePost("") as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }
    }
}