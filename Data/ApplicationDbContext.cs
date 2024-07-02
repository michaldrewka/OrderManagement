using System.Data.Entity;
using OrderManagement.Models;

namespace OrderManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("MyDatabaseConnectionString") { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderBillingEntry> OrderBillingEntries { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferBillingEntry> OfferBillingEntries { get; set; }
    }
}