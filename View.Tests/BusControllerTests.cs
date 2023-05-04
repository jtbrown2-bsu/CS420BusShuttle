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
    public class BusControllerTests
    {
        private readonly Mock<IBusRepository> _mock;

        public BusControllerTests()
        {
            _mock = new Mock<IBusRepository>();
            _mock.Setup(data => data.Get())
                .ReturnsAsync(new List<Bus>() {
                new Bus
                {
                    Id = 1,
                    BusNumber = 1,
                },
                new Bus
                {
                    Id = 2,
                    BusNumber = 2,
                },
                new Bus
                {
                    Id = 3,
                    BusNumber = 3,
                }
            });
            _mock.Setup(data => data.Get(It.IsAny<int>()))
                .ReturnsAsync(
                new Bus
                {
                    Id = 1,
                    BusNumber = 1,
                }
            );
            _mock.Setup(data => data.Add(It.IsAny<Bus>()))
                .Verifiable();
            _mock.Setup(data => data.Update(It.IsAny<Bus>()))
                .Verifiable();
            _mock.Setup(data => data.Delete(It.IsAny<int>()))
                .Verifiable();

        }

        private ILogger<BusController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<BusController>();
        }

        private BusController GetController()
        {
            return new BusController(_mock.Object, GetLogger());
        }

        [Fact]
        public async Task IndexHasList()
        {
            var busController = GetController();

            var result = await busController.Index() as ViewResult;

            var model = result.ViewData.Model as List<Bus>;

            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task CreateRedirects()
        {
            var busController = GetController();

            var result = await busController.Create(new Bus()) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task EditRedirects()
        {
            var busController = GetController();

            var result = await busController.Edit(new Bus()) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task DeleteRedirects()
        {
            var busController = GetController();

            var result = await busController.DeletePost(1) as RedirectToActionResult;

            Assert.Equal("Index", result.ActionName);
        }
    }
}