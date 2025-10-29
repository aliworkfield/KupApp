using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CouponApp.Data;
using CouponApp.Models;
using CouponApp.Services;

namespace CouponApp
{
    public class DatabaseInitializer
    {
        public static async Task InitializeDatabase(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CouponAppContext>();
            optionsBuilder.UseSqlite(connectionString);

            using var context = new CouponAppContext(optionsBuilder.Options);
            
            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();

            // Check if users already exist
            if (!context.Users.Any())
            {
                // Create default users
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] = "coupon_app_secret_key_that_should_be_changed_in_production",
                        ["Ldap:Server"] = "ldap.company.com",
                        ["Ldap:Port"] = "389",
                        ["Ldap:BaseDn"] = "dc=company,dc=com",
                        ["Ldap:UseSsl"] = "false"
                    })
                    .Build();
                    
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
        }
    }
}