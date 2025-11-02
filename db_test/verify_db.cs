using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Connection string for LocalDB
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=CouponTestDb;Trusted_Connection=true;";

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Successfully connected to the database!");

                // List users
                Console.WriteLine("\n--- Users ---");
                using (var command = new SqlCommand("SELECT Id, Username, Email, Role FROM Users", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"ID: {reader["Id"]}, Username: {reader["Username"]}, Email: {reader["Email"]}, Role: {reader["Role"]}");
                    }
                }

                // List coupons
                Console.WriteLine("\n--- Coupons ---");
                using (var command = new SqlCommand("SELECT Id, Code, Description, DiscountAmount, DiscountType FROM Coupons", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"ID: {reader["Id"]}, Code: {reader["Code"]}, Description: {reader["Description"]}, Discount: {reader["DiscountAmount"]}{(reader["DiscountType"].ToString() == "percentage" ? "%" : "")}");
                    }
                }

                // List coupon assignments
                Console.WriteLine("\n--- Coupon Assignments ---");
                using (var command = new SqlCommand(@"
                    SELECT ca.Id, c.Code, u.Username, ca.IsUsed 
                    FROM CouponAssignments ca
                    JOIN Coupons c ON ca.CouponId = c.Id
                    JOIN Users u ON ca.UserId = u.Id", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"Assignment ID: {reader["Id"]}, Coupon: {reader["Code"]}, User: {reader["Username"]}, Used: {reader["IsUsed"]}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}