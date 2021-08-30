using Microsoft.EntityFrameworkCore;
using System;

namespace Zh.Db
{
    public class PlayerContext : DbContext
    {
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<CovidTest> CovidTests { get; set; }

        public PlayerContext()
        {
            this.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\PlayerDb.mdf;Integrated Security=True;MultipleActiveResultSets=true");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CovidTest>(entity =>
            {
                entity.
                    HasOne(test => test.Player).
                    WithMany(player => player.Tests).
                    HasForeignKey(test=>test.PlayerId);
            });
        }
    }
}
