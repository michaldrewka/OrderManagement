using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Data;
using OrderManagement.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrderManagement.Services
{
    public class AllegroApiService
    {
        private static readonly string ClientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
        private static readonly string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
        private static readonly string TokenEndpoint = System.Configuration.ConfigurationManager.AppSettings["TokenEndpoint"];
        private static readonly string RedirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

        public static void RedirectToAuthorization()
        {
            var authorizationUrl = $"https://allegro.pl.allegrosandbox.pl/auth/oauth/authorize?response_type=code&client_id={ClientId}&redirect_uri={Uri.EscapeDataString(RedirectUri)}";
            Console.WriteLine(@"Please visit the following URL to authorize the application:");
            Console.WriteLine(authorizationUrl);
        }

        public static async Task<string> GetAccessTokenAsync(string authorizationCode)
        {
            using (var client = new HttpClient())
            {
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUri)
                });

                var response = await client.PostAsync(TokenEndpoint, requestContent);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                return json["access_token"]?.ToString();
            }
        }

        public static async Task GetAndStoreOrdersAsync(string accessToken)
        {
            const string apiUrl = "https://api.allegro.pl.allegrosandbox.pl/order/checkout-forms";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                using (var dbContext = new ApplicationDbContext())
                {
                    var orders = json["checkoutForms"];
                    if (orders != null)
                        foreach (var order in orders)
                        {
                            var newOrder = new Order
                            {
                                OrderId = order["id"]?.ToString(),
                                ErpOrderId = null,
                                InvoiceId = null,
                                StoreId = 1
                            };

                            dbContext.Orders.Add(newOrder);
                        }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public static async Task<List<OrderBillingEntry>> GetOrdersBillingEntries(string accessToken)
        {
            const string apiUrl = "https://api.allegro.pl.allegrosandbox.pl/billing/billing-entries";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(json);
                    return MapBillingEntries(data);
                }
                else
                {
                    throw new Exception($"Failed to fetch billing entries: {response.ReasonPhrase}");
                }   
            }
        }

        private static List<OrderBillingEntry> MapBillingEntries(JObject data)
        {
            return data["billingEntries"]
                .Select(entry => new OrderBillingEntry
                {
                    BillingEntryId = (string)entry["id"],
                    OccurredAt = (DateTime)entry["occurredAt"],
                    TypeId = (string)entry["type"]?["id"],
                    TypeName = (string)entry["type"]?["name"],
                    OfferId = (string)entry["offer"]?["id"],
                    OfferName = (string)entry["offer"]?["name"],
                    Amount = (decimal)entry["value"]?["amount"],
                    Currency = (string)entry["value"]?["currency"],
                    OrderId = (string)entry["order"]?["id"]
                })
                .ToList();
        }

        public static async Task GetAndStoreOffersAsync(string accessToken)
        {
            const string apiUrl = "https://api.allegro.pl.allegrosandbox.pl/sale/offers";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                using (var dbContext = new ApplicationDbContext())
                {
                    var offers = json["offers"];
                    if (offers != null)
                        foreach (var offer in offers)
                        {
                            var newOffer = new Offer
                            {
                                OfferId = offer["id"]?.ToString(),
                                Name = offer["name"]?.ToString()
                            };

                            dbContext.Offers.Add(newOffer);
                        }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public static async Task<OfferBillingEntry> GetOffersBillingEntries(string accessToken, string offerId)
        {
            var apiUrl = $"https://api.allegro.pl.allegrosandbox.pl/sale/offers/{offerId}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(json);
                    return MapOfferBillingEntries(data);
                }
                else
                {
                    throw new Exception($"Failed to fetch billing entries: {response.ReasonPhrase}");
                }
            }
        }

        private static OfferBillingEntry MapOfferBillingEntries(JObject json)
        {
            try
            {
                return new OfferBillingEntry()
                {
                    OfferId = (string)json["id"],
                    Name = (string)json["name"],
                    Price = (decimal)json["sellingMode"]["price"]?["amount"], 
                    Quantity = (int)json["stock"]["available"]
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while mapping JSON to Offer object: {ex.Message}");
                return null;
            }
        }
    }

}
