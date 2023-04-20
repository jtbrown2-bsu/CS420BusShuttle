using Core.Models;

namespace Core.Repositories;

public class EntryRepository
{
    private readonly ShuttleDbContext _dbContext;
    
    public EntryRepository(ShuttleDbContext dbContext) {
            _dbContext = dbContext;
    }

        public Entry Get(int entryId)
        {
            return _dbContext.Entries.FirstOrDefault(c => c.Id == entryId);
        }

        public List<Entry> Get()
        {
            var entries = _dbContext.Entries
                .ToList();
            return entries;
        }

        public Entry Add(Entry entry)
        {
            _dbContext.Entries.Add(entry);
            _dbContext.SaveChanges();
        }

        public void Update(int entryId, Entry entry)
        {
            var entryToUpdate = _dbContext.Entries.Find(entryId);

            if (entryToUpdate != null)
            {
                _dbContext.Entry(entryToUpdate).CurrentValues.SetValues(entry);
                _dbContext.SaveChanges();
            } else
            {
                throw new Exception("No entry found.");
            }
        }

        public void DeleteComment(int entryId)
        {
            var entryToDelete = _dbContext.Entries.Find(entryId);
            if (entryToDelete == null) return;
            _dbContext.Entries.Remove(entryToDelete);
            _dbContext.SaveChanges();
        }
}