using Core;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using View.Controllers;
using View.Models;

namespace View.Tests
{
    public class LoopControllerTests
    {
        private readonly Mock<ILoopRepository> _mock;
        private readonly Mock<IEntryRepository> _emock;
        private readonly Mock<IStopRepository> _smock;

        public LoopControllerTests()
        {
            _mock = new Mock<ILoopRepository>();
            _mock.Setup(data => data.Get())
                .ReturnsAsync(new List<Loop>() {
                new Loop
                {
                    Id = 1,
                    Name = "",
                },
                new Loop
                {
                    Id = 2,
                    Name = "",
                },
                new Loop
                {
                    Id = 3,
                    Name = "",
                }
            });
            _mock.Setup(data => data.Get(It.IsAny<int>()))
                .ReturnsAsync(
                new Loop
                {
                    Id = 1,
                    Name = "",
                }
            );
            _mock.Setup(data => data.Add(It.IsAny<Loop>()))
                .Verifiable();
            _mock.Setup(data => data.Update(It.IsAny<Loop>()))
                .Verifiable();
            _mock.Setup(data => data.Delete(It.IsAny<int>()))
                .Verifiable();

            _emock = new Mock<IEntryRepository>();
            _emock.Setup(data => data.Get())
                .ReturnsAsync(new List<Entry>() {
                new Entry
                {
                    Id = 1,
                    DriverId = "1",
                    LoopId = 2,
                    StopId = 2,
                    BusId = 2,
                    RouteId = 2,
                    Boarded = 1,
                    LeftBehind = 2,
                    Timestamp = new DateTime(2000, 2, 1)
                }
            });

            _smock = new Mock<IStopRepository>();
            _smock.Setup(data => data.Get())
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
            _smock.Setup(data => data.Get(It.IsAny<int>()))
                .ReturnsAsync(
                new Stop
                {
                    Id = 1,
                    Name = "",
                }
            );
            _smock.Setup(data => data.Add(It.IsAny<Stop>()))
                .Verifiable();
            _smock.Setup(data => data.Update(It.IsAny<Stop>()))
                .Verifiable();
            _smock.Setup(data => data.Delete(It.IsAny<int>()))
                .Verifiable();
        }

        private ILogger<LoopController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<LoopController>();
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

        private LoopController GetController()
        {
            return new LoopController(_mock.Object, _smock.Object, _emock.Object, GetLogger());
        }

        [Fact]
        public async Task IndexHasList()
        {
            var loopController = GetController();

            var result = await loopController.Index(null, null) as ViewResult;

            var model = result.ViewData.Model as List<Loop>;

            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task CreateRedirects()
        {
            var loopController = GetController();

            var result = await loopController.Create(new LoopViewModel()) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task EditRedirects()
        {
            var loopController = GetController();

            var result = await loopController.Edit(new LoopViewModel()) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task DeleteRedirects()
        {
            var loopController = GetController();

            var result = await loopController.DeletePost(1) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }
    }
}