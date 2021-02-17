using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleClient
{
    public static class Program
    {
        private static async Task Main()
        {
            // Discover endpoints from metadata
            using var client = new HttpClient();

            var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:9070/identity");
            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return;
            }

            // Request token
            using var passwordTokenRequest = new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PasswordClient",
                ClientSecret = "PasswordSecret",
                UserName = "Bobsmith@email.com",
                Password = "Pass123$",
                Scope = "Organisation",
            };

            TokenResponse tokenResponse = await client.RequestPasswordTokenAsync(passwordTokenRequest);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // Call API
            using var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync(new Uri("https://localhost:9071"));
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JsonDocument.Parse(content));
            }
        }
    }
}
