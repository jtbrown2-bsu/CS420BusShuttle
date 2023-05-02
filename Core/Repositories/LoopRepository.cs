using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public class LoopRepository
    {
        private readonly ShuttleDbContext _dbContext;

        public LoopRepository(ShuttleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async void Add(Loop loop)
        {
            await _dbContext.Loops.AddAsync(loop);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Loop> Get(int id)
        {
            return await _dbContext.Loops.FindAsync(id);
        }

        public async Task<List<Loop>> Get()
        {
            return await _dbContext.Loops.ToListAsync();
        }

        public async void Update(Loop loop)
        {
            var itemToUpdate = await _dbContext.Loops.FindAsync(loop.Id);

            if (itemToUpdate != null)
            {
                _dbContext.Entry(itemToUpdate).CurrentValues.SetValues(loop);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No loop found.");
            }
        }

        public async void Delete(int id)
        {
            var itemToDelete = await _dbContext.Loops.FindAsync(id);
            if (itemToDelete == null)
            {
                throw new Exception("No loop found.");
            }

            _dbContext.Loops.Remove(itemToDelete);

            await _dbContext.SaveChangesAsync();
        }
    }
}
