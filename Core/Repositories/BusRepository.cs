using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public interface IBusRepository
    {
        Task Add(Bus bus);
        Task Delete(int id);
        Task<List<Bus>> Get();
        Task<Bus> Get(int id);
        Task Update(Bus bus);
    }

    public class BusRepository : IBusRepository
    {
        private readonly ShuttleDbContext _dbContext;

        public BusRepository(ShuttleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Bus bus)
        {
            await _dbContext.Buses.AddAsync(bus);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Bus> Get(int id)
        {
            return await _dbContext.Buses.FindAsync(id);
        }

        public async Task<List<Bus>> Get()
        {
            return await _dbContext.Buses.ToListAsync();
        }

        public async Task Update(Bus bus)
        {
            var itemToUpdate = await _dbContext.Buses.FindAsync(bus.Id);

            if (itemToUpdate != null)
            {
                _dbContext.Entry(itemToUpdate).CurrentValues.SetValues(bus);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No bus found.");
            }
        }

        public async Task Delete(int id)
        {
            var itemToDelete = await _dbContext.Buses.FindAsync(id);
            if (itemToDelete == null)
            {
                throw new Exception("No bus found.");
            }

            _dbContext.Buses.Remove(itemToDelete);

            await _dbContext.SaveChangesAsync();
        }
    }
}
