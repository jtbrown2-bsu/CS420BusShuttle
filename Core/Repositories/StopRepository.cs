using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public class StopRepository
    {
        private readonly ShuttleDbContext _dbContext;

        public StopRepository(ShuttleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Stop stop)
        {
            await _dbContext.Stops.AddAsync(stop);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Stop> Get(int id)
        {
            return await _dbContext.Stops.FindAsync(id);
        }

        public async Task<List<Stop>> Get()
        {
            return await _dbContext.Stops.ToListAsync();
        }

        public async Task Update(Stop stop)
        {
            var itemToUpdate = await _dbContext.Stops.FindAsync(stop.Id);

            if (itemToUpdate != null)
            {
                _dbContext.Entry(itemToUpdate).CurrentValues.SetValues(stop);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No stop found.");
            }
        }

        public async Task Delete(int id)
        {
            var itemToDelete = await _dbContext.Stops.FindAsync(id);
            if (itemToDelete == null)
            {
                throw new Exception("No stop found.");
            }

            _dbContext.Stops.Remove(itemToDelete);

            await _dbContext.SaveChangesAsync();
        }
    }
}
