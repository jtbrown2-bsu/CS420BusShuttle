using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Tests
{
    public class DriverRepositoryTests
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
                await ctx.Database.EnsureDeletedAsync();
                await Seed(ctx);
            }
            return ctx;
        }

        private async Task Seed(ShuttleDbContext ctx)
        {
            await ctx.Database.EnsureDeletedAsync();
            var drivers = new List<Driver>
            {
                new Driver
                {
                    Id = "1",
                    FirstName = "TestF1",
                    LastName = "TestL1",
                },
                new Driver
                {
                    Id = "2",
                    FirstName = "TestF2",
                    LastName = "TestL2",
                },
                new Driver
                {
                    Id = "3",
                    FirstName = "TestF3",
                    LastName = "TestL3",
                }
            };
            await ctx.Drivers.AddRangeAsync(drivers);
            await ctx.SaveChangesAsync();
        }

        private async Task<DriverRepository> GetRepository(bool seed = true)
        {
            return new DriverRepository(await GetDbContext(seed));
        }

        private Driver GetAddExampleData()
        {
            return new Driver
            {
                Id = "4",
                FirstName = "TestF4",
                LastName = "TestL4",
            };
        }

        private Driver GetUpdateExampleData()
        {
            return new Driver
            {
                Id = "1",
                FirstName = "UpdatedF",
                LastName = "UpdatedL",
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
        public async Task GetSingleGets()
        {
            var repo = await GetRepository();
            var exampleToGet = await repo.Get("1");
            Assert.Equal("TestF1", exampleToGet.FirstName);
        }

        [Fact]
        public async Task AddGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get("4");
            Assert.Equal("TestF4", exampleToGet.FirstName);
        }

        [Fact]
        public async Task GetAllHasCorrectCount()
        {
            var repo = await GetRepository();
            var exampleToGet = await repo.Get();
            Assert.Equal(3, exampleToGet.Count);
        }

        [Fact]
        public async Task GetAllHasCorrectData()
        {
            var repo = await GetRepository();
            var exampleToGet = await repo.Get();
            Assert.Equal("TestF1", exampleToGet[0].FirstName);
        }

        [Fact]
        public async Task AddSanityGetAll()
        {
            var repo = await GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get();
            Assert.Equal(4, exampleToGet.Count);
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
            example.Id = "90";
            var throwsException = await Record.ExceptionAsync(async () => await repo.Update(example));
            Assert.Equal("No driver found.", throwsException.Message);
        }

        [Fact]
        public async Task UpdateGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get("1");
            Assert.Equal("UpdatedF", exampleToGet.FirstName);
            Assert.Equal("UpdatedL", exampleToGet.LastName);
        }

        [Fact]
        public async Task UpdateGetAllSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get();
            Assert.Equal("UpdatedF", exampleToGet[0].FirstName);
        }

        [Fact]
        public async Task DeleteReturns()
        {
            var repo = await GetRepository();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Delete("1"));
            Assert.Null(throwsException);
        }

        [Fact]
        public async Task DeleteReturnsExceptionWrongId()
        {
            var repo = await GetRepository();
            var throwsException = await Record.ExceptionAsync(async () => await repo.Delete("90"));
            Assert.Equal("No driver found.", throwsException.Message);
        }

        [Fact]
        public async Task DeleteGetSingleSanity()
        {
            var repo = await GetRepository();
            await repo.Delete("1");
            var exampleToGet = await repo.Get("1");
            Assert.Null(exampleToGet);
        }

        [Fact]
        public async Task DeleteGetAllSanity()
        {
            var repo = await GetRepository();
            await repo.Delete("1");
            var exampleToGet = await repo.Get();
            Assert.Equal("TestF2", exampleToGet[0].FirstName);
        }
    }
}