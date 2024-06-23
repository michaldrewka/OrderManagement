using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MyConsoleApp.Models;
using Newtonsoft.Json;

namespace MyConsoleApp.Services
{
    public class AllegroApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _environment;
        private string _accessToken;

        public AllegroApiService(string clientId, string clientSecret, string environment)
        {
            _httpClient = new HttpClient();
            _clientId = clientId;
            _clientSecret = clientSecret;
            _environment = environment;
        }

        public async Task AuthenticateAsync()
        {
            var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://{_environment}.allegro.pl/auth/oauth/token"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                requestMessage.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
                    _accessToken = tokenResponse.AccessToken;
                }
                else
                {
                    throw new Exception("Unable to retrieve access token.");
                }
            }
        }

        public async Task<List<BillingEntry>> GetBillingEntries(string orderId)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                await AuthenticateAsync();
            }

            var requestUrl = $"https://{_environment}.allegro.pl/billing/billing-entries?order.id={orderId}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to fetch billing entries.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var billingEntries = JsonConvert.DeserializeObject<List<BillingEntry>>(content);

            return billingEntries;
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }
        }
    }
}
