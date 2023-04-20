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

        public Route AddRoute(Route route)
        {
            _dbContext.Add(route);
            _dbContext.SaveChanges();
            return route;
        }

        public Route GetRoute(int routeId)
        {
            return _dbContext.Routes.Find(routeId);
        }

        public IEnumerable<Route> GetAllRoutes()
        {
            return _dbContext.Routes.ToList();
        }

        public void UpdateRoute(int routeId, Route route)
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

        public void DeleteRoute(int routeId)
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