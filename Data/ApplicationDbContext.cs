using System.Data.Entity;
using MyConsoleApp.Models;

namespace MyConsoleApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=MyDatabaseConnectionString")
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<BillingEntry> BillingEntries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .ToTable("OrderTable")
                .HasKey(e => e.Id);

            modelBuilder.Entity<BillingEntry>()
                .ToTable("BillingEntries")
                .HasKey(e => e.Id);
        }
    }
}
