using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public class BusRepository
    {
        private readonly ShuttleDbContext _dbContext;

        public BusRepository(ShuttleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddBus(Bus bus)
        {
            _dbContext.Busses.Add(bus);
            _dbContext.SaveChanges();
        }

        public Bus GetBus(int id)
        {
            return _dbContext.Busses.Find(id);
        }

        public IEnumerable<Bus> GetAllBusses()
        {
            return _dbContext.Busses.ToList();
        }

        public void UpdateBus(int busId, Bus bus)
        {
            var busToUpdate = _dbContext.Busses.Find(busId);

            if (busToUpdate != null)
            {
                _dbContext.Entry(busToUpdate).CurrentValues.SetValues(bus);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("No bus found.");
            }
        }

        public void DeleteBus(int id)
        {
            var bus = _dbContext.Busses.Find(id);
            if (bus != null)
            {
                _dbContext.Busses.Remove(bus);
                _dbContext.SaveChanges();
            }
        }
    }
}
