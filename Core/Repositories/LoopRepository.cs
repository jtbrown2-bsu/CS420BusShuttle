using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core.Repositories
{
    public class LoopRepository
    {
        private readonly ShuttleDbContext _dbContext;

        public LoopRepository(ShuttleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddLoop(Loop loop)
        {
            _dbContext.Loops.Add(loop);
            _dbContext.SaveChanges();
        }
        public Loop GetLoop(int id)
        {
            return _dbContext.Loops.Find(id);
        }

        public IEnumerable<Loop> GetAllLoops()
        {
            return _dbContext.Loops.ToList();
        }

        public void UpdateLoop(int loopId, Loop loop)
        {
            var loopToUpdate = _dbContext.Loops.Find(loopId);

            if (loopToUpdate != null)
            {
                _dbContext.Entry(loopToUpdate).CurrentValues.SetValues(loop);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("No loop found.");
            }
        }

        public void DeleteLoop(int id)
        {
            var loop = _dbContext.Loops.Find(id);
            if (loop != null)
            {
                _dbContext.Loops.Remove(loop);
                _dbContext.SaveChanges();
            }
        }
    }
}
