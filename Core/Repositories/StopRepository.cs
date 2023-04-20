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

        public Stop AddStop(Stop stop)
        {
            _dbContext.Add(stop);
            _dbContext.SaveChanges();
            return stop;
        }

        public Stop GetStop(int stopId)
        {
            return _dbContext.Stops.Find(stopId);
        }

        public IEnumerable<Stop> GetAllStops()
        {
            return _dbContext.Stops.ToList();
        }

        public void UpdateStop(int stopId, Stop stop)
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

        public void DeleteStop(int stopId)
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
