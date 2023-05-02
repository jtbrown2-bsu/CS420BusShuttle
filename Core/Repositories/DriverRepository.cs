using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public class DriverRepository
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

        public async Task<Driver> Get(int id)
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
                _dbContext.Entry(itemToUpdate).CurrentValues.SetValues(driver);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No driver found.");
            }
        }

        public async Task Delete(int id) 
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
