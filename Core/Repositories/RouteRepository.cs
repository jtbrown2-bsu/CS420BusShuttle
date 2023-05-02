using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public class RouteRepository
    {
        private readonly ShuttleDbContext _dbContext;

        public RouteRepository(ShuttleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Route Add(Route route)
        {
            _dbContext.Add(route);
            _dbContext.SaveChanges();
            return route;
        }

        public Route Get(int routeId)
        {
            return _dbContext.Routes.Find(routeId);
        }

        public IEnumerable<Route> Get()
        {
            return _dbContext.Routes.ToList();
        }

        public void Update(int routeId, Route route)
        {
            var routeToUpdate = _dbContext.Routes.Find(routeId);

            if (routeToUpdate != null)
            {
                _dbContext.Entry(routeToUpdate).CurrentValues.SetValues(route);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("No route found.");
            }
        }

        public void Delete(int routeId)
        {
            Route route = _dbContext.Routes.Find(routeId);
            if (route != null)
            {
                _dbContext.Routes.Remove(route);
                _dbContext.SaveChanges();
            }
        }
    }
}