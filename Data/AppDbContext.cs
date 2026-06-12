using FiloTakipWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiloTakipWebApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Arac> Araclar { get; set; }
        public DbSet<Sofor> Soforler { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Arac>().HasIndex(a => a.Plaka).IsUnique();
        }
    }
}
