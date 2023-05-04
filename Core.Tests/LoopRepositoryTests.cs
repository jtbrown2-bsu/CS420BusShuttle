using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Tests
{
    public class LoopRepositoryTests
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
                new Loop
                {
                    Id = 3,
                    Name = "Loop3",
                }
            };
            await ctx.Loops.AddRangeAsync(loops);
            await ctx.SaveChangesAsync();
        }

        private async Task<LoopRepository> GetRepository(bool seed = true)
        {
            return new LoopRepository(await GetDbContext(seed));
        }

        private Loop GetAddExampleData()
        {
            return new Loop
            {
                Id = 4,
                Name = "Loop4",
            };
        }

        private Loop GetUpdateExampleData()
        {
            return new Loop
            {
                Id = 1,
                Name = "Loop5",
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
            Assert.Equal("Loop1", exampleToGet.Name);
        }

        [Fact]
        public async Task AddGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetAddExampleData();
            await repo.Add(example);
            var exampleToGet = await repo.Get(4);
            Assert.Equal("Loop4", exampleToGet.Name);
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
            Assert.Equal("Loop1", exampleToGet[0].Name);
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
            Assert.Equal("No loop found.", throwsException.Message);
        }

        [Fact]
        public async Task UpdateGetSingleSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get(1);
            Assert.Equal("Loop5", exampleToGet.Name);
        }

        [Fact]
        public async Task UpdateGetAllSanity()
        {
            var repo = await GetRepository();
            var example = GetUpdateExampleData();
            await repo.Update(example);
            var exampleToGet = await repo.Get();
            Assert.Equal("Loop5", exampleToGet[0].Name);
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
            Assert.Equal("No loop found.", throwsException.Message);
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
            Assert.Equal("Loop2", exampleToGet[0].Name);
        }
    }
}