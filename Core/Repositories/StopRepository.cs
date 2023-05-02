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

        public Stop Add(Stop stop)
        {
            _dbContext.Add(stop);
            _dbContext.SaveChanges();
            return stop;
        }

        public Stop Get(int stopId)
        {
            return _dbContext.Stops.Find(stopId);
        }

        public IEnumerable<Stop> Get()
        {
            return _dbContext.Stops.ToList();
        }

        public void Update(int stopId, Stop stop)
        {
            var stopToUpdate = _dbContext.Stops.Find(stopId);

            if (stopToUpdate != null)
            {
                _dbContext.Entry(stopToUpdate).CurrentValues.SetValues(stop);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("No stop found.");
            }
        }

        public void Delete(int stopId)
        {
            Stop stop = _dbContext.Stops.Find(stopId);
            if (stop != null)
            {
                _dbContext.Stops.Remove(stop);
                _dbContext.SaveChanges();
            }
        }
    }
}
