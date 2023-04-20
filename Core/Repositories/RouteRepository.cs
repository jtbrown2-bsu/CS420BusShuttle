﻿using Microsoft.EntityFrameworkCore;
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

        public Route CreateRoute(Route route)
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

        public Route UpdateRoute(Route route)
        {
            _dbContext.Entry(route).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return route;
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