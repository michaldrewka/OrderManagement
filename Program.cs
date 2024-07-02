using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using OrderManagement.Data;
using OrderManagement.Models;
using OrderManagement.Services;

namespace OrderManagement
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Step 1: Redirect user to authorization URL
                AllegroApiService.RedirectToAuthorization();

                // Step 2: Get authorization_code from URL redirection
                Console.WriteLine(@"Please enter the authorization code:");
                var authorizationCode = Console.ReadLine();

                // Step 3: Get access token using authorization_code
                var accessToken = await AllegroApiService.GetAccessTokenAsync(authorizationCode);

                // Step 4: User selects what information to retrieve
                Console.WriteLine(@"Select an option:");
                Console.WriteLine(@"1. Retrieve orders");
                Console.WriteLine(@"2. Retrieve order billing entries");
                Console.WriteLine(@"3. Retrieve offers");
                Console.WriteLine(@"4. Retrieve offers billing entries");
                var choice = Console.ReadLine();

                using (var db = new ApplicationDbContext())
                {
                    switch (choice)
                    {
                        case "1":
                            // Retrieve and store orders
                            await AllegroApiService.GetAndStoreOrdersAsync(accessToken);
                            Console.WriteLine(@"Orders have been successfully retrieved and stored in the database.");
                            break;
                        case "2":
                            // Retrieve and store orders billing entries
                            var billingEntries = await AllegroApiService.GetOrdersBillingEntries(accessToken);
                            db.OrderBillingEntries.AddRange(billingEntries);
                            await db.SaveChangesAsync();
                            Console.WriteLine(@"Billing entries fetched and saved to the database.");
                            break;
                        case "3":
                            // Retrieve and store offers
                            await AllegroApiService.GetAndStoreOffersAsync(accessToken);
                            Console.WriteLine(@"Offers have been successfully retrieved and stored in the database.");
                            break;
                        case "4":
                            // Retrieve and store offer billing entries
                            var offers = await db.Offers
                                .ToListAsync();
                            foreach (var offer in offers)
                            {
                                var offerbillingEntries = await AllegroApiService.GetOffersBillingEntries(accessToken, offer.OfferId);
                                db.OfferBillingEntries.Add(offerbillingEntries);
                                await db.SaveChangesAsync();
                            }
                            Console.WriteLine(@"Offers have been successfully retrieved and stored in the database.");
                            break;
                        default:
                            Console.WriteLine(@"Invalid choice. Exiting.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Error: {ex.Message}");
                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}
