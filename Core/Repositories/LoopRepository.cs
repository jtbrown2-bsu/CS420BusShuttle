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

        public void Add(Loop loop)
        {
            _dbContext.Loops.Add(loop);
            _dbContext.SaveChanges();
        }
        public Loop Get(int id)
        {
            return _dbContext.Loops.Find(id);
        }

        public IEnumerable<Loop> Get()
        {
            return _dbContext.Loops.ToList();
        }

        public void Update(int loopId, Loop loop)
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

        public void Delete(int id)
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
