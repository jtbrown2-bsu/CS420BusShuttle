using Microsoft.EntityFrameworkCore;
using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public class ShuttleDbContext : IdentityDbContext<Driver>
    {
        protected readonly IConfiguration Configuration;
        public ShuttleDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
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
            modelBuilder.Entity<Route>().Navigation(e => e.Loop).AutoInclude();
            modelBuilder.Entity<Loop>().Navigation(e => e.Routes).AutoInclude();
            modelBuilder.Entity<Entry>().Navigation(e => e.Bus).AutoInclude();
            modelBuilder.Entity<Entry>().Navigation(e => e.Driver).AutoInclude();
            modelBuilder.Entity<Entry>().Navigation(e => e.Loop).AutoInclude();
            modelBuilder.Entity<Entry>().Navigation(e => e.Route).AutoInclude();
            modelBuilder.Entity<Entry>().Navigation(e => e.Stop).AutoInclude();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        }
    }
}