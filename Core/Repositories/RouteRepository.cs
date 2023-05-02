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

        public async void Add(Route route)
        {
            await _dbContext.Routes.AddAsync(route);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Route> Get(int routeId)
        {
            return await _dbContext.Routes.FindAsync(routeId);
        }

        public async Task<List<Route>> Get()
        {
            return await _dbContext.Routes.ToListAsync();
        }

        public async void Update(Route route)
        {
            var itemToUpdate = await _dbContext.Routes.FindAsync(route.Id);

            if (itemToUpdate != null)
            {
                _dbContext.Entry(itemToUpdate).CurrentValues.SetValues(route);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No route found.");
            }
        }

        public async void Delete(int id)
        {
            var itemToDelete = await _dbContext.Routes.FindAsync(id);
            if (itemToDelete == null)
            {
                throw new Exception("No route found.");
            }

            _dbContext.Routes.Remove(itemToDelete);

            await _dbContext.SaveChangesAsync();
        }
    }
}