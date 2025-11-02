using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    
    static async Task Main(string[] args)
    {
        Console.WriteLine("Testing bulk assignment of coupons to users...");
        
        try
        {
            // Login as admin
            Console.WriteLine("Logging in as admin...");
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
            Console.WriteLine("Login successful!");
            
            // Set authorization header
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            // Get all users
            Console.WriteLine("Fetching all users...");
            var usersResponse = await client.GetAsync("http://localhost:8001/users");
            var users = await usersResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
            Console.WriteLine($"Found {users.Count} users");
            
            // Get all coupons
            Console.WriteLine("Fetching all coupons...");
            var couponsResponse = await client.GetAsync("http://localhost:8001/coupons");
            var coupons = await couponsResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
            Console.WriteLine($"Found {coupons.Count} coupons");
            
            // Create bulk assignment data
            var assignments = new List<Dictionary<string, int>>();
            var count = Math.Min(coupons.Count, users.Count);
            
            for (int i = 0; i < count; i++)
            {
                var assignment = new Dictionary<string, int>
                {
                    ["couponId"] = Convert.ToInt32(coupons[i]["id"]),
                    ["userId"] = Convert.ToInt32(users[i]["id"])
                };
                assignments.Add(assignment);
            }
            
            // Bulk assign coupons to users
            Console.WriteLine("Bulk assigning coupons to users...");
            var json = JsonSerializer.Serialize(assignments);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var bulkAssignResponse = await client.PostAsync("http://localhost:8001/coupons/assign-bulk", content);
            if (!bulkAssignResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Bulk assignment failed: {bulkAssignResponse.StatusCode}");
                var errorContent = await bulkAssignResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {errorContent}");
                return;
            }
            
            var assignmentResult = await bulkAssignResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
            Console.WriteLine($"Bulk assignment successful! Assigned {assignmentResult.Count} coupons");
            
            // Display the assignments
            Console.WriteLine("Assignments:");
            foreach (var assignment in assignmentResult)
            {
                Console.WriteLine($"Coupon ID: {assignment["couponId"]} assigned to User ID: {assignment["userId"]}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}