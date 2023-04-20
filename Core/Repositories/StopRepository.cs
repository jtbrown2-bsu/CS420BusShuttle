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

        public Stop CreateStop(Stop stop)
        {
            _dbContext.Add(stop);
            _dbContext.SaveChanges();
            return stop;
        }

        public Stop GetStop(int stopId)
        {
            return _dbContext.Set<Stop>().Find(stopId);
        }

        public IEnumerable<Stop> GetAllStops()
        {
            return _dbContext.Set<Stop>().ToList();
        }

        public Stop UpdateStop(Stop stop)
        {
            _dbContext.Entry(stop).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return stop;
        }

        public void DeleteStop(int stopId)
        {
            Stop stop = _dbContext.Set<Stop>().Find(stopId);
            if (stop != null)
            {
                _dbContext.Set<Stop>().Remove(stop);
                _dbContext.SaveChanges();
            }
        }
    }
}
