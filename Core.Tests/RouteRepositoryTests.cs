using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Tests
{
    public class RouteRepositoryTests
    {
        private async Task<ShuttleDbContext> GetDbContext(bool seed = true)
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
            if (seed)
            {
                await Seed(ctx);
            }
            return ctx;
        }

        private async Task Seed(ShuttleDbContext ctx)
        {
            await ctx.Database.EnsureDeletedAsync();
            var loops = new List<Loop>
            {
                new Loop
                {
                    Id = 1,
                    Name = "Loop1",
                },
                new Loop
                {
                    Id = 2,
                    Name = "Loop2",
                },
            };

            var stops = new List<Stop>
            {
                new Stop
                {
                    Id = 1,
                    Name = "Stop1",
                },
                new Stop
                {
                    Id = 2,
                    Name = "Stop2",
                },
            };

            var routes = new List<Route>
            {
                new Route
                {
                    Id = 1,
                    Order = 1,
                    Loop = loops[0],
                    LoopId = loops[0].Id,
                    Stop = stops[0],
                    StopId = stops[0].Id
                },
                new Route
                {
                    Id = 2,
                    Order = 2,
                    Loop = loops[1],
                    LoopId = loops[1].Id,
                    Stop = stops[1],
                    StopId = stops[1].Id
                },
            };
            await ctx.Loops.AddRangeAsync(loops);
            await ctx.Routes.AddRangeAsync(routes);
            await ctx.SaveChangesAsync();
        }

        private async Task<RouteRepository> GetRepository(bool seed = true)
        {
            return new RouteRepository(await GetDbContext(seed));
        }

        private Route GetAddExampleData()
        {
            return new Route
            {
                Id = 3,
                Order = 3,
                LoopId = 2,
                StopId = 2
            };
        }

        private Route GetUpdateExampleData()
        {
            return new Route
            {
                Id = 1,
                Order = 3,
                LoopId = 1,
                StopId = 2
            };
        }

        [Fact]
        public async Task AddReturns()
        {
            var repo = await GetRepository();
            var example = GetAddExampleData();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Add(example));
            Assert.Null(throwsException);
        }

        [Fact]
        public async Task GetSingleGetsBus()
        {
            var repo = await GetRepository();
            var exampleToGet = await repo.Get(1);
            Assert.Equal(1, exampleToGet.Order);
        }

        [Fact]
        public async Task AddGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get(3);
            Assert.Equal(3, exampleToGet.Order);
        }

        [Fact]
        public async Task GetAllHasCorrectCount()
        {
            var repo = await GetRepository();
            var exampleToGet = await repo.Get();
            Assert.Equal(2, exampleToGet.Count);
        }

        [Fact]
        public async Task GetAllHasCorrectData()
        {
            var repo = await GetRepository();
            var exampleToGet = await repo.Get();
            Assert.Equal(1, exampleToGet[0].Order);
        }
        
        [Fact]
        public async Task AddSanityGetAll()
        {
            var repo = await GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get();
            Assert.Equal(3, exampleToGet.Count);
        }

        [Fact]
        public async Task UpdateReturns()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Update(example));
            Assert.Null(throwsException);
        }

        [Fact]
        public async Task UpdateReturnsExceptionWrongId()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            example.Id = 90;
            var throwsException = await Record.ExceptionAsync(async () => await repo.Update(example));
            Assert.Equal("No route found.", throwsException.Message);
        }

        [Fact]
        public async Task UpdateGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get(1);
            Assert.Equal(3, exampleToGet.Order);
        }

        [Fact]
        public async Task UpdateGetAllSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get();
            Assert.Equal(3, exampleToGet[0].Order);
        }

        [Fact]
        public async Task DeleteReturns()
        {
            var repo = await GetRepository();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Delete(1));
            Assert.Null(throwsException);
        }

        [Fact]
        public async Task DeleteReturnsExceptionWrongId()
        {
            var repo = await GetRepository();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Delete(90));
            Assert.Equal("No route found.", throwsException.Message);
        }

        [Fact]
        public async Task DeleteGetSingleSanity()
        {
            var repo = await GetRepository();
            await repo.Delete(1);
            var exampleToGet = await repo.Get(1);
            Assert.Null(exampleToGet);
        }

        [Fact]
        public async Task DeleteGetAllSanity()
        {
            var repo = await GetRepository();
            await repo.Delete(1);
            var exampleToGet = await repo.Get();
            Assert.Equal(2, exampleToGet[0].Order);
        }

        [Fact]
        public async Task SwapOrdersSwaps()
        {
            var repo = await GetRepository();
            var original = await repo.Get();
            await repo.SwapOrders(original[0].Id, original[1].Id);
            var exampleToGet = await repo.Get();
            Assert.Equal(2, exampleToGet[0].Order);
            Assert.Equal(1, exampleToGet[1].Order);
        }
    }
}