using System;
using System.Configuration;
using System.Threading.Tasks;
using MyConsoleApp.Data;
using MyConsoleApp.Models;
using MyConsoleApp.Services;

namespace MyConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            var environment = ConfigurationManager.AppSettings["Environment"];
            var connectionString = ConfigurationManager.ConnectionStrings["MyDatabaseConnectionString"].ConnectionString;

            var dbContext = new ApplicationDbContext();
            var orderRepository = new Repository<Order>(dbContext);
            var billingEntryRepository = new Repository<BillingEntry>(dbContext);
            var allegroApiService = new AllegroApiService(clientId, clientSecret, environment);

            await allegroApiService.AuthenticateAsync();

            var orders = orderRepository.GetAll();

            foreach (var order in orders)
            {
                var billingEntries = await allegroApiService.GetBillingEntries(order.OrderId);

                foreach (var billingEntry in billingEntries)
                {
                    billingEntryRepository.Add(new BillingEntry
                    {
                        OrderId = order.OrderId,
                        Type = billingEntry.Type,
                        Amount = billingEntry.Amount,
                        Date = billingEntry.Date
                    });
                }
            }

            Console.WriteLine("Billing entries have been fetched and saved.");
        }
    }
}
