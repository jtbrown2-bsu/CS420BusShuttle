using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public interface IDriverRepository
    {
        Task Add(Driver driver);
        Task Delete(string id);
        Task<List<Driver>> Get();
        Task<Driver> Get(string id);
        Task Update(Driver driver);
    }

    public class DriverRepository : IDriverRepository
    {
        private readonly ShuttleDbContext _dbContext;

        public DriverRepository(ShuttleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Driver driver)
        {
            await _dbContext.Drivers.AddAsync(driver);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Driver> Get(string id)
        {
            return await _dbContext.Drivers.FindAsync(id);
        }

        public async Task<List<Driver>> Get()
        {
            return await _dbContext.Drivers.ToListAsync();
        }

        public async Task Update(Driver driver)
        {
            var itemToUpdate = await _dbContext.Drivers.FindAsync(driver.Id);

            if (itemToUpdate != null)
            {
                itemToUpdate.FirstName = driver.FirstName;
                itemToUpdate.LastName = driver.LastName;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No driver found.");
            }
        }

        public async Task Delete(string id)
        {
            var itemToDelete = await _dbContext.Drivers.FindAsync(id);
            if (itemToDelete == null)
            {
                throw new Exception("No driver found.");
            }

            _dbContext.Drivers.Remove(itemToDelete);

            await _dbContext.SaveChangesAsync();
        }

    }
}
