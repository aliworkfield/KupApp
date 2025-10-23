# .NET Backend for Coupon App

This directory contains the .NET 8 backend implementation for the Coupon Management application.

## Overview

This is a complete backend implementation that replicates the functionality of the original Python/FastAPI backend but using .NET 8 technologies:

- **Framework**: .NET 8
- **Database**: SQLite (development) / PostgreSQL (production)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/OpenAPI

## Features Implemented

1. **User Management**
   - Role-based access control (Admin, Manager, User)
   - User authentication with JWT tokens
   - CRUD operations for users (Admin only)

2. **Coupon Management**
   - Create, read, update, delete coupons (Admin only)
   - Assign coupons to users (Manager/Admin)
   - Mark coupons as used (User)
   - View assigned coupons (User)
   - View created coupons (Manager)

3. **Security**
   - Password hashing with BCrypt
   - JWT token-based authentication
   - Role-based authorization

## Project Structure

```
CouponApp/
├── Controllers/          # API controllers
├── Data/                 # Database context
├── DTOs/                 # Data Transfer Objects
├── Models/               # Entity models
├── Services/             # Business logic services
├── Properties/           # Configuration files
├── Program.cs            # Application entry point
├── appsettings.json      # Configuration
└── README.md             # Project documentation
```

## Getting Started

1. **Navigate to the project directory**
   ```bash
   cd CouponApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the application**
   - API: http://localhost:8000
   - Swagger UI: http://localhost:8000/swagger

## Default Users

The application automatically creates these test accounts on first run:

| Role    | Username | Password   |
|---------|----------|------------|
| Admin   | admin    | admin123   |
| Manager | manager  | manager123 |
| User    | user     | user123    |

## API Endpoints

The API provides the same functionality as the original Python backend:

- Authentication endpoints (`/auth/token`)
- User management endpoints (`/users`)
- Coupon management endpoints (`/coupons`)

Refer to the `CouponApp/README.md` for detailed API documentation.