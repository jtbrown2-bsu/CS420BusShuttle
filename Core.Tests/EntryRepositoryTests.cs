using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Tests
{
    public class EntryRepositoryTests
    {
        private ShuttleDbContext GetDbContext(bool seed = true)
        {
            var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ShuttleDbContext>()
            .UseInMemoryDatabase($"Shuttle{Guid.NewGuid()}")
            .UseInternalServiceProvider(serviceProvider)
            .Options;

            var ctx = new ShuttleDbContext(options);
            ctx.Database.EnsureDeleted();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (seed)
            {
                Seed(ctx);
            }
            return ctx;
        }

        private void Seed(ShuttleDbContext ctx)
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
            var bus = new Bus { Id = 1, BusNumber = 12 };
            ctx.Buses.AddRange(bus, new Bus { Id = 2, BusNumber = 13 });
            var firstLoop = new Loop { Id = 1, Name = "Green" };
            var firstStop = new Stop { Id = 1, Name = "Stop1" };
            var firstDriver = new Driver
            {
                Id = "1",
                FirstName = "TestF",
                LastName = "TestL"
            };
            var firstRoute = new Route { Id = 1, Order = 1, StopId = 1, LoopId = 1, Loop = firstLoop, Stop = firstStop };
            ctx.Loops.AddRange(firstLoop, new Loop { Id = 2, Name = "Red"});
            ctx.Stops.AddRange(firstStop, new Stop { Id = 2, Name = "Stop2"});
            ctx.Drivers.Add(firstDriver);
            ctx.Routes.AddRange(firstRoute, new Route { Id = 2, Order = 2, StopId = 2, LoopId = 1 }, new Route { Id = 3, Order = 1, StopId = 1, LoopId = 2 });
            var entry = new Entry
            {
                DriverId = "1",
                LoopId = 1,
                StopId = 1,
                BusId = 1,
                RouteId = 1,
                Boarded = 10,
                LeftBehind = 5,
                Timestamp = new DateTime(2000, 1, 1),
                Driver = firstDriver,
                Loop = firstLoop,
                Stop = firstStop,
                Bus = bus,
                Route = firstRoute
            };
            ctx.Entries.Add(entry);
            ctx.SaveChanges();
        }

        private EntryRepository GetRepository(bool seed = true)
        {
            return new EntryRepository(GetDbContext(seed));
        }

        private Entry GetAddExampleData()
        {
            return new Entry
            {
                DriverId = "1",
                LoopId = 1,
                StopId = 2,
                BusId = 1,
                RouteId = 2,
                Boarded = 10,
                LeftBehind = 5,
                Timestamp = new DateTime(2000, 1, 1)
            };
        }

        private Entry GetUpdateExampleData()
        {
            return new Entry
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
            };
        }

        [Fact]
        public async Task AddReturns()
        {
            var repo = GetRepository();
            var example = GetAddExampleData();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Add(example));
            Assert.Null(throwsException);
        }

        [Fact]
        public async Task GetSingleGets()
        {
            var repo = GetRepository();
            var exampleToGet = await repo.Get(1);
            Assert.Equal(1, exampleToGet.Stop.Id);
        }

        [Fact]
        public async Task AddGetSingleSanity()
        {
            var repo = GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get(2);
            Assert.Equal(2, exampleToGet.Stop.Id);
        }

        [Fact]
        public async Task GetAllHasCorrectCount()
        {
            var repo = GetRepository();
            var exampleToGet = repo.Get().Result;
            Assert.Single(exampleToGet);
        }

        [Fact]
        public async Task GetAllHasCorrectData()
        {
            var repo = GetRepository();
            var exampleToGet = await repo.Get();
            Assert.Equal(1, exampleToGet[0].StopId);
        }

        [Fact]
        public async Task AddSanityGetAll()
        {
            var repo = GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get();
            Assert.Equal(2, exampleToGet.Count);
        }

        [Fact]
        public async Task UpdateReturns()
        {
            var repo = GetRepository();
            var example = GetUpdateExampleData();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Update(example));
            Assert.Null(throwsException);
        }

        [Fact]
        public async Task UpdateReturnsExceptionWrongId()
        {
            var repo = GetRepository();
            var example = GetUpdateExampleData();
            example.Id = 90;
            var throwsException = await Record.ExceptionAsync(async () => await repo.Update(example));
            Assert.Equal("No entry found.", throwsException.Message);
        }

        [Fact]
        public async Task UpdateGetSingleSanity()
        {
            var repo = GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get(1);
            Assert.Equal(2, exampleToGet.StopId);
            Assert.Equal(2, exampleToGet.LoopId);
            Assert.Equal(2, exampleToGet.BusId);
            Assert.Equal(1, exampleToGet.Boarded);
            Assert.Equal(2, exampleToGet.LeftBehind);
            Assert.Equal(new DateTime(2000, 2, 1).Ticks, exampleToGet.Timestamp.Ticks);
        }

        [Fact]
        public async Task UpdateGetAllSanity()
        {
            var repo = GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);

            var exampleToGetAll = await repo.Get();
            var exampleToGet = exampleToGetAll[0];
            Assert.Equal(2, exampleToGet.StopId);
            Assert.Equal(2, exampleToGet.LoopId);
            Assert.Equal(2, exampleToGet.BusId);
            Assert.Equal(1, exampleToGet.Boarded);
            Assert.Equal(2, exampleToGet.LeftBehind);
            Assert.Equal(new DateTime(2000, 2, 1).Ticks, exampleToGet.Timestamp.Ticks);
        }

        [Fact]
        public async Task DeleteReturns()
        {
            var repo = GetRepository();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Delete(1));
            Assert.Null(throwsException);
        }

        [Fact]
        public async Task DeleteReturnsExceptionWrongId()
        {
            var repo = GetRepository();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Delete(90));
            Assert.Equal("No entry found.", throwsException.Message);
        }

        [Fact]
        public async Task DeleteGetSingleSanity()
        {
            var repo = GetRepository();
            await repo.Delete(1);
            var exampleToGet = await repo.Get(1);
            Assert.Null(exampleToGet);
        }

        [Fact]
        public async Task DeleteGetAllSanity()
        {
            var repo = GetRepository();
            await repo.Delete(1);
            var exampleToGet = await repo.Get();
            Assert.Empty(exampleToGet);
        }
    }
}