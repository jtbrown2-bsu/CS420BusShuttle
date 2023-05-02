using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public interface IEntryRepository
{
    Task Add(Entry entry);
    Task Delete(int id);
    Task<List<Entry>> Get();
    Task<Entry> Get(int id);
    Task Update(Entry entry);
}

public class EntryRepository : IEntryRepository
{
    private readonly ShuttleDbContext _dbContext;

    public EntryRepository(ShuttleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Entry> Get(int id)
    {
        return await _dbContext.Entries.FindAsync(id);
    }

    public async Task<List<Entry>> Get()
    {
        return await _dbContext.Entries.ToListAsync();
    }

    public async Task Add(Entry entry)
    {
        await _dbContext.Entries.AddAsync(entry);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Entry entry)
    {
        var itemToUpdate = await _dbContext.Entries.FindAsync(entry);

        if (itemToUpdate != null)
        {
            _dbContext.Entry(itemToUpdate).CurrentValues.SetValues(entry);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("No entry found.");
        }
    }

    public async Task Delete(int id)
    {
        var itemToDelete = await _dbContext.Entries.FindAsync(id);
        if (itemToDelete == null)
        {
            throw new Exception("No entry found.");
        }

        _dbContext.Entries.Remove(itemToDelete);

        await _dbContext.SaveChangesAsync();
    }
}