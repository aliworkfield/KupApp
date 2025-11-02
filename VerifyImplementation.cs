using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    
    static async Task Main(string[] args)
    {
        Console.WriteLine("Verifying implementation...");
        
        try
        {
            // Test 1: Login as admin
            Console.WriteLine("Test 1: Logging in as admin...");
            var loginContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", "admin"),
                new KeyValuePair<string, string>("password", "admin123")
            });
            
            var loginResponse = await client.PostAsync("http://localhost:8001/auth/token", loginContent);
            if (!loginResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Login failed: {loginResponse.StatusCode}");
                return;
            }
            
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var token = loginResult["accessToken"];
            Console.WriteLine("✓ Login successful!");
            
            // Test 2: Get users
            Console.WriteLine("Test 2: Fetching users...");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var usersResponse = await client.GetAsync("http://localhost:8001/users");
            if (usersResponse.IsSuccessStatusCode)
            {
                var users = await usersResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
                Console.WriteLine($"✓ Found {users.Count} users");
            }
            else
            {
                Console.WriteLine($"Failed to fetch users: {usersResponse.StatusCode}");
            }
            
            // Test 3: Get coupons
            Console.WriteLine("Test 3: Fetching coupons...");
            var couponsResponse = await client.GetAsync("http://localhost:8001/coupons");
            if (couponsResponse.IsSuccessStatusCode)
            {
                var coupons = await couponsResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
                Console.WriteLine($"✓ Found {coupons.Count} coupons");
            }
            else
            {
                Console.WriteLine($"Failed to fetch coupons: {couponsResponse.StatusCode}");
            }
            
            Console.WriteLine("Verification complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}