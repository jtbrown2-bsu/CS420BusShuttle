using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Tests
{
    public class StopRepositoryTests
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
                new Stop
                {
                    Id = 3,
                    Name = "Stop3",
                }
            };
            await ctx.Stops.AddRangeAsync(stops);
            await ctx.SaveChangesAsync();
        }

        private async Task<StopRepository> GetRepository(bool seed = true)
        {
            return new StopRepository(await GetDbContext(seed));
        }

        private Stop GetAddExampleData()
        {
            return new Stop
            {
                Id = 4,
                Name = "Stop4",
            };
        }

        private Stop GetUpdateExampleData()
        {
            return new Stop
            {
                Id = 1,
                Name = "Stop5",
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
            Assert.Equal("Stop1", exampleToGet.Name);
        }

        [Fact]
        public async Task AddGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get(4);
            Assert.Equal("Stop4", exampleToGet.Name);
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
            Assert.Equal("Stop1", exampleToGet[0].Name);
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
            example.Id = 90;
            var throwsException = await Record.ExceptionAsync(async () => await repo.Update(example));
            Assert.Equal("No stop found.", throwsException.Message);
        }

        [Fact]
        public async Task UpdateGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get(1);
            Assert.Equal("Stop5", exampleToGet.Name);
        }

        [Fact]
        public async Task UpdateGetAllSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get();
            Assert.Equal("Stop5", exampleToGet[0].Name);
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
            Assert.Equal("No stop found.", throwsException.Message);
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
            Assert.Equal("Stop2", exampleToGet[0].Name);
        }
    }
}