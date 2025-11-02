using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CouponApp.Data;
using CouponApp.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        // Set the current directory to the location of this executable
        var currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Directory.SetCurrentDirectory(currentDir);

        // Create configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Create context options
        var optionsBuilder = new DbContextOptionsBuilder<CouponAppContext>();
        optionsBuilder.UseSqlServer(connectionString);

        // Create context
        using var context = new CouponAppContext(optionsBuilder.Options);

        try
        {
            // Test connection
            await context.Database.CanConnectAsync();
            Console.WriteLine("Database connection successful!");

            // List all users
            var users = await context.Users.ToListAsync();
            Console.WriteLine($"\nFound {users.Count} users:");
            foreach (var user in users)
            {
                Console.WriteLine($"- {user.Username} ({user.Email}) - Role: {user.Role}");
            }

            // List all coupons
            var coupons = await context.Coupons.ToListAsync();
            Console.WriteLine($"\nFound {coupons.Count} coupons:");
            foreach (var coupon in coupons)
            {
                Console.WriteLine($"- {coupon.Code}: {coupon.Description} ({coupon.DiscountAmount}{(coupon.DiscountType == "percentage" ? "%" : "")})");
            }

            // List all coupon assignments
            var assignments = await context.CouponAssignments
                .Include(ca => ca.User)
                .Include(ca => ca.Coupon)
                .ToListAsync();
            
            Console.WriteLine($"\nFound {assignments.Count} coupon assignments:");
            foreach (var assignment in assignments)
            {
                Console.WriteLine($"- {assignment.Coupon.Code} assigned to {assignment.User.Username} (Used: {assignment.IsUsed})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to database: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}