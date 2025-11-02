using System;
using Microsoft.EntityFrameworkCore;
using CouponApp.Data;
using CouponApp.Models;
using CouponApp.Services;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Initializing database...");
        
        // Create configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        // Create DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<CouponAppContext>();
        optionsBuilder.UseSqlite(connectionString);
        
        // Create context and ensure database is created
        using var context = new CouponAppContext(optionsBuilder.Options);
        await context.Database.EnsureCreatedAsync();
        
        Console.WriteLine("Database created successfully.");
        
        // Check if users already exist
        if (!context.Users.Any())
        {
            Console.WriteLine("Creating default users...");
            
            // Create LDAP service and auth service
            var ldapService = new LdapService(configuration);
            var authService = new AuthService(configuration, ldapService);
            
            var users = new User[]
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = authService.HashPassword("admin123"),
                    Role = UserRole.Admin
                },
                new User
                {
                    Username = "manager",
                    Email = "manager@example.com",
                    PasswordHash = authService.HashPassword("manager123"),
                    Role = UserRole.Manager
                },
                new User
                {
                    Username = "user",
                    Email = "user@example.com",
                    PasswordHash = authService.HashPassword("user123"),
                    Role = UserRole.User
                }
            };
            
            context.Users.AddRange(users);
            await context.SaveChangesAsync();
            Console.WriteLine("Default users created successfully.");
        }
        else
        {
            Console.WriteLine("Users already exist in the database.");
        }
        
        Console.WriteLine("Database initialization completed.");
    }
}