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
    public class RouteControllerTests
    {
        private readonly Mock<IRouteRepository> _mock;
        private readonly Mock<ILoopRepository> _lmock;
        private readonly Mock<IStopRepository> _smock;

        public RouteControllerTests()
        {
            _mock = new Mock<IRouteRepository>();
            _mock.Setup(data => data.Get())
                .ReturnsAsync(new List<Route>() {
                new Route
                {
                    Id = 1,
                    Order = 1,
                    StopId = 1,
                    Stop = new Stop
                    {
                        Id = 1,
                        Name = "Test"
                    },
                    Loop = new Loop
                    {
                        Id = 0,
                        Name = "Test"
                    },
                    LoopId = 0
                },
                new Route
                {
                    Id = 2,
                    Order = 2,
                    StopId = 2,
                    Stop = new Stop
                    {
                        Id = 2,
                        Name = "Test"
                    }
                },
                new Route
                {
                    Id = 3,
                    Order = 3,
                    StopId = 3,
                    Stop = new Stop
                    {
                        Id = 3,
                        Name = "Test"
                    }
                }
            });
            _mock.Setup(data => data.Get(It.IsAny<int>()))
                .ReturnsAsync(
                new Route
                {
                    Id = 1,
                    Order = 1,
                    StopId = 1,
                    Stop = new Stop
                    {
                        Id = 1,
                        Name = "Test"
                    }
                }
            );
            _mock.Setup(data => data.Add(It.IsAny<Route>()))
                .Verifiable();
            _mock.Setup(data => data.Update(It.IsAny<Route>()))
                .Verifiable();
            _mock.Setup(data => data.Delete(It.IsAny<int>()))
                .Verifiable();

            _lmock = new Mock<ILoopRepository>();
            _lmock.Setup(data => data.Get())
                .ReturnsAsync(new List<Loop>() {
                new Loop
                {
                    Id = 0,
                    Name = "",
                    Routes = new List<Route>
                    {
                        new Route
                        {
                            Id = 2,
                            Order = 1,
                            StopId = 1,
                            Stop = new Stop
                            {
                                Id = 1,
                                Name = "Test"
                            },
                            Loop = new Loop
                            {
                                Id = 0,
                                Name = "Test",
                            },
                            LoopId = 0
                        }
                    }
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
            _lmock.Setup(data => data.Get(It.IsAny<int>()))
                .ReturnsAsync(
                new Loop
                {
                    Id = 1,
                    Name = "",
                }
            );

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

        private ILogger<RouteController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<RouteController>();
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

        private RouteController GetController()
        {
            return new RouteController(_mock.Object, _smock.Object, _lmock.Object, GetLogger());
        }

        [Fact]
        public async Task IndexHasList()
        {
            var routeController = GetController();

            var result = await routeController.Index(null) as ViewResult;

            var model = result.ViewData.Model as List<Route>;
            
            Assert.Null(model);
        }

        [Fact]
        public async Task CreateRedirects()
        {
            var routeController = GetController();

            var result = await routeController.Create(new RouteViewModel
            {
                Id = 1,
                StopId = 1,
                LoopId = 1,
            }) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task EditRedirects()
        {
            var routeController = GetController();

            var result = await routeController.Edit(new RouteEditViewModel
            {
                Id = 1,
                Order = 1,
                StopId = 1,
                LoopId = 1,
            }) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task DeleteRedirects()
        {
            var routeController = GetController();

            var result = await routeController.DeletePost(1) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }
    }
}