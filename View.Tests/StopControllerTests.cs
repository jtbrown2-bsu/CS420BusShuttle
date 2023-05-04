using Core;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using View.Controllers;

namespace View.Tests
{
    public class StopControllerTests
    {
        private readonly Mock<IStopRepository> _mock;

        public StopControllerTests()
        {
            _mock = new Mock<IStopRepository>();
            _mock.Setup(data => data.Get())
                .ReturnsAsync(new List<Stop>() {
                new Stop
                {
                    Id = 1,
                    Name = "",
                },
                new Stop
                {
                    Id = 2,
                    Name = "",
                },
                new Stop
                {
                    Id = 3,
                    Name = "",
                }
            });
            _mock.Setup(data => data.Get(It.IsAny<int>()))
                .ReturnsAsync(
                new Stop
                {
                    Id = 1,
                    Name = "",
                }
            );
            _mock.Setup(data => data.Add(It.IsAny<Stop>()))
                .Verifiable();
            _mock.Setup(data => data.Update(It.IsAny<Stop>()))
                .Verifiable();
            _mock.Setup(data => data.Delete(It.IsAny<int>()))
                .Verifiable();

        }

        private ILogger<StopController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<StopController>();
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

        private StopController GetController()
        {
            return new StopController(_mock.Object, GetLogger());
        }

        [Fact]
        public async Task IndexHasList()
        {
            var stopController = GetController();

            var result = await stopController.Index() as ViewResult;

            var model = result.ViewData.Model as List<Stop>;

            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task CreateRedirects()
        {
            var stopController = GetController();

            var result = await stopController.Create(new Stop()) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task EditRedirects()
        {
            var stopController = GetController();

            var result = await stopController.Edit(new Stop()) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task DeleteRedirects()
        {
            var stopController = GetController();

            var result = await stopController.DeletePost(1) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }
    }
}