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

        public Driver Add(Driver driver)
        {
            _dbContext.Add(driver);
            _dbContext.SaveChanges();
            return driver;
        }

        public Driver Get(int id)
        {
            return _dbContext.Drivers.Find(id);
        }

        public IEnumerable<Driver> Get()
        {
            return _dbContext.Set<Driver>().ToList();
        }

        public void Update(int driverId, Driver driver)
        {
            var driverToUpdate = _dbContext.Drivers.Find(driverId);

            if (driverToUpdate != null)
            {
                _dbContext.Entry(driverToUpdate).CurrentValues.SetValues(driver);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("No driver found.");
            }
        }

        public void Delete(int id)
        {
            Driver driver = _dbContext.Drivers.Find(id);
            if (driver != null)
            {
                _dbContext.Set<Driver>().Remove(driver);
                _dbContext.SaveChanges();
            }
        }
    
    }
}
