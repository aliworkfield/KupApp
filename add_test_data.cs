using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CouponApp.Data;
using CouponApp.Models;
using CouponApp.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

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
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create auth service for password hashing
            var authService = new AuthService(configuration, new LdapService(configuration));

            // Add test users if they don't exist
            if (!await context.Users.AnyAsync(u => u.Username == "user1"))
            {
                var user1 = new User
                {
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = authService.HashPassword("user123"),
                    Role = UserRole.User
                };
                context.Users.Add(user1);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "user2"))
            {
                var user2 = new User
                {
                    Username = "user2",
                    Email = "user2@example.com",
                    PasswordHash = authService.HashPassword("user123"),
                    Role = UserRole.User
                };
                context.Users.Add(user2);
            }

            if (!await context.Users.AnyAsync(u => u.Username == "user3"))
            {
                var user3 = new User
                {
                    Username = "user3",
                    Email = "user3@example.com",
                    PasswordHash = authService.HashPassword("user123"),
                    Role = UserRole.User
                };
                context.Users.Add(user3);
            }

            await context.SaveChangesAsync();

            // Get manager user
            var manager = await context.Users.FirstOrDefaultAsync(u => u.Username == "manager");
            if (manager == null)
            {
                Console.WriteLine("Manager user not found!");
                return;
            }

            // Add test coupons if they don't exist
            if (!await context.Coupons.AnyAsync(c => c.Code == "SAVE10"))
            {
                var coupon1 = new Coupon
                {
                    Code = "SAVE10",
                    Description = "10% off your purchase",
                    DiscountAmount = 10,
                    DiscountType = "percentage",
                    CreatedById = manager.Id
                };
                context.Coupons.Add(coupon1);
            }

            if (!await context.Coupons.AnyAsync(c => c.Code == "SAVE20"))
            {
                var coupon2 = new Coupon
                {
                    Code = "SAVE20",
                    Description = "20% off your purchase",
                    DiscountAmount = 20,
                    DiscountType = "percentage",
                    CreatedById = manager.Id
                };
                context.Coupons.Add(coupon2);
            }

            if (!await context.Coupons.AnyAsync(c => c.Code == "FLAT5"))
            {
                var coupon3 = new Coupon
                {
                    Code = "FLAT5",
                    Description = "£5 off your purchase",
                    DiscountAmount = 5,
                    DiscountType = "fixed",
                    CreatedById = manager.Id
                };
                context.Coupons.Add(coupon3);
            }

            if (!await context.Coupons.AnyAsync(c => c.Code == "WELCOME"))
            {
                var coupon4 = new Coupon
                {
                    Code = "WELCOME",
                    Description = "Welcome discount",
                    DiscountAmount = 15,
                    DiscountType = "percentage",
                    CreatedById = manager.Id
                };
                context.Coupons.Add(coupon4);
            }

            await context.SaveChangesAsync();

            // Assign some coupons to users
            var user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user1");
            var user2 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user2");
            var user3 = await context.Users.FirstOrDefaultAsync(u => u.Username == "user3");
            var save10Coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Code == "SAVE10");
            var save20Coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Code == "SAVE20");
            var flat5Coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Code == "FLAT5");

            if (user1 != null && save10Coupon != null && !await context.CouponAssignments.AnyAsync(ca => ca.UserId == user1.Id && ca.CouponId == save10Coupon.Id))
            {
                var assignment1 = new CouponAssignment
                {
                    CouponId = save10Coupon.Id,
                    UserId = user1.Id
                };
                context.CouponAssignments.Add(assignment1);
            }

            if (user1 != null && save20Coupon != null && !await context.CouponAssignments.AnyAsync(ca => ca.UserId == user1.Id && ca.CouponId == save20Coupon.Id))
            {
                var assignment2 = new CouponAssignment
                {
                    CouponId = save20Coupon.Id,
                    UserId = user1.Id
                };
                context.CouponAssignments.Add(assignment2);
            }

            if (user2 != null && flat5Coupon != null && !await context.CouponAssignments.AnyAsync(ca => ca.UserId == user2.Id && ca.CouponId == flat5Coupon.Id))
            {
                var assignment3 = new CouponAssignment
                {
                    CouponId = flat5Coupon.Id,
                    UserId = user2.Id
                };
                context.CouponAssignments.Add(assignment3);
            }

            if (user3 != null && save10Coupon != null && !await context.CouponAssignments.AnyAsync(ca => ca.UserId == user3.Id && ca.CouponId == save10Coupon.Id))
            {
                var assignment4 = new CouponAssignment
                {
                    CouponId = save10Coupon.Id,
                    UserId = user3.Id
                };
                context.CouponAssignments.Add(assignment4);
            }

            await context.SaveChangesAsync();

            Console.WriteLine("Test data added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding test data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}