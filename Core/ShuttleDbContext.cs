using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core
{
    public class ShuttleDbContext : DbContext
    {
        public ShuttleDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Bus> Busses { get; set; }
        public DbSet<Loop> Loops { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Route> Routes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}