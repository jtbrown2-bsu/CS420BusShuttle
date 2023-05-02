using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core
{
    public class ShuttleDbContext : DbContext
    {
        public ShuttleDbContext()
        {

        }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Loop> Loops { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Route> Routes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Route>().Navigation(e => e.Stop).AutoInclude();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "MyDatabase")
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }
}