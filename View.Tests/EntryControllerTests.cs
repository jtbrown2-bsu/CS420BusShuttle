using Core;
using Core.Models;
using Core.Repositories;
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
    public class EntryControllerTests
    {
        private readonly Mock<IEntryRepository> _mock;
        private readonly Mock<IBusRepository> _bmock;
        private readonly Mock<IStopRepository> _smock;
        private readonly Mock<IDriverRepository> _dmock;
        private readonly Mock<ILoopRepository> _lmock;
        private readonly Mock<UserManager<Driver>> _umock;

        private Mock<UserManager<Driver>> GetUserManager()
        {
            var mock = new Mock<UserManager<Driver>>(Mock.Of<IUserStore<Driver>>(), null, null, null, null, null, null, null, null);
            mock.Setup(userManager => userManager.GetUserId(It.IsAny<ClaimsPrincipal>()))
    .Returns("");
            return mock;
        }

        private Entry GetTestEntry()
        {
            return new Entry
            {
                Id = 1,
                DriverId = "1",
                LoopId = 1,
                StopId = 1,
                BusId = 1,
                RouteId = 1,
                Boarded = 1,
                LeftBehind = 1,
                Timestamp = new DateTime(2000, 2, 1),
                Loop = new Loop
                {
                    Id = 1,
                    Name = "",
                    Routes = new List<Route>
                    {
                        new Route
                        {
                            Id = 1,
                            Order = 1,
                            Stop = new Stop
                            {
                                Id = 1,
                                Name = "Test"
                            }
                        }
                    }
                },
                Bus = new Bus
                {
                    Id = 1,
                    BusNumber = 1
                },
                Route = new Route
                {
                    Id = 1
                },
                Driver = new Driver
                {
                    Id = "",
                    FirstName = "",
                    LastName = ""
                },
                Stop = new Stop
                {
                    Id = 1,
                    Name = ""
                }
            };
        }

        public EntryControllerTests()
        {
            _mock = new Mock<IEntryRepository>();
            _mock.Setup(data => data.Get())
                .ReturnsAsync(new List<Entry>()
                {
                    new Entry
                {
                    Id = 1,
                    DriverId = "1",
                    LoopId = 1,
                    StopId = 1,
                    BusId = 1,
                    RouteId = 1,
                    Boarded = 1,
                    LeftBehind = 1,
                    Timestamp = new DateTime(2000, 2, 1)
                }
                });
            _mock.Setup(data => data.Get(It.IsAny<int>()))
                .ReturnsAsync(
                new Entry
                {
                    Id = 1,
                    DriverId = "1",
                    LoopId = 1,
                    StopId = 1,
                    BusId = 1,
                    RouteId = 1,
                    Boarded = 1,
                    LeftBehind = 1,
                    Timestamp = new DateTime(2000, 2, 1)
                }
            );
            _mock.Setup(data => data.Add(It.IsAny<Entry>()))
                .Verifiable();
            _mock.Setup(data => data.Update(It.IsAny<Entry>()))
                .Verifiable();
            _mock.Setup(data => data.Delete(It.IsAny<int>()))
                .Verifiable();

            _umock = GetUserManager();

            _bmock = new Mock<IBusRepository>();
            _bmock.Setup(data => data.Get())
                .ReturnsAsync(new List<Bus>() {
                new Bus
                {
                    Id = 1,
                    BusNumber = 1
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

            _lmock = new Mock<ILoopRepository>();
            _lmock.Setup(data => data.Get())
                .ReturnsAsync(new List<Loop>() {
                new Loop
                {
                    Id = 1,
                    Name = "",
                    Routes = new List<Route>
                    {
                        new Route
                        {
                            Id = 1,
                            Order = 1,
                            Stop = new Stop
                            {
                                Id = 1,
                                Name = "Test"
                            }
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

            _dmock = new Mock<IDriverRepository>();
            _dmock.Setup(data => data.Get())
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
        }

        private ILogger<EntryController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<EntryController>();
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

        private EntryController GetController()
        {
            return new EntryController(_mock.Object, _smock.Object, _umock.Object, _bmock.Object, _lmock.Object, GetLogger());
        }

        [Fact]
        public async Task IndexHasList()
        {
            var entryController = GetController();

            var result = await entryController.Index(null, null, null, null, null) as ViewResult;

            var model = result.ViewData.Model as List<Entry>;

            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task DeleteRedirects()
        {
            var entryController = GetController();

            var result = await entryController.DeletePost(1) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }
    }
}