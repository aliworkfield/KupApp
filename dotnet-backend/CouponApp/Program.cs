using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CouponApp.Data;
using CouponApp.Services;
using CouponApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<CouponAppContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add custom services
builder.Services.AddScoped<LdapService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CouponService>();
builder.Services.AddScoped<AuthService>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration["Jwt:Key"] ?? "default_secret_key_that_should_be_changed_in_production")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => new { message = "Welcome to the Coupon Management API" });

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy" });

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CouponAppContext>();
    context.Database.EnsureCreated();
    
    // Seed default users if they don't exist
    SeedDefaultUsers(context).Wait();
}

app.Run();

async Task SeedDefaultUsers(CouponAppContext context)
{
    if (!context.Users.Any())
    {
        var authService = new AuthService(builder.Configuration, new LdapService(builder.Configuration));
        
        // Create default admin user
        var admin = new User
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = authService.HashPassword("admin123"),
            Role = UserRole.Admin
        };
        context.Users.Add(admin);
        
        // Create default manager user
        var manager = new User
        {
            Username = "manager",
            Email = "manager@example.com",
            PasswordHash = authService.HashPassword("manager123"),
            Role = UserRole.Manager
        };
        context.Users.Add(manager);
        
        // Create default user
        var user = new User
        {
            Username = "user",
            Email = "user@example.com",
            PasswordHash = authService.HashPassword("user123"),
            Role = UserRole.User
        };
        context.Users.Add(user);
        
        await context.SaveChangesAsync();
    }
}