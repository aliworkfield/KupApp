# Coupon App - .NET Backend

This is the .NET 8 backend implementation for the Coupon Management application.

## Features

- User authentication with JWT tokens
- Role-based access control (Admin, Manager, User)
- Coupon management (create, read, update, delete)
- Coupon assignment to users
- RESTful API with Swagger documentation

## Technology Stack

- **Framework**: .NET 8
- **Database**: SQLite (development), PostgreSQL recommended for production
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Password Hashing**: BCrypt

## Project Structure

```
Controllers/          # API controllers
Data/                 # Database context
DTOs/                 # Data Transfer Objects
Models/               # Entity models
Services/             # Business logic services
```

## Setup Instructions

1. **Install Dependencies**
   ```bash
   dotnet restore
   ```

2. **Run the Application**
   ```bash
   dotnet run
   ```

3. **Access API Documentation**
   - Swagger UI: `http://localhost:8000/swagger`

## Default Users

After running the application, the following test accounts will be created:

| Role    | Username | Password   |
|---------|----------|------------|
| Admin   | admin    | admin123   |
| Manager | manager  | manager123 |
| User    | user     | user123    |

> Note: These credentials should be changed in production environments.

## API Endpoints

### Authentication
- `POST /auth/token` - Obtain JWT token

### Users
- `GET /users/me` - Get current user info
- `POST /users` - Create new user (Admin only)
- `GET /users` - Get all users (Admin only)
- `GET /users/{id}` - Get user by ID
- `PUT /users/{id}` - Update user (Admin only)
- `DELETE /users/{id}` - Delete user (Admin only)

### Coupons
- `POST /coupons` - Create new coupon (Admin only)
- `GET /coupons/my-coupons` - Get current user's coupons (User only)
- `GET /coupons/my-unused-coupons` - Get current user's unused coupons (User only)
- `GET /coupons/my-created` - Get manager's created coupons (Manager only)
- `GET /coupons/unassigned` - Get unassigned coupons (Manager/Admin only)
- `POST /coupons/assign` - Assign coupon to user (Manager/Admin only)
- `POST /coupons/use/{assignmentId}` - Mark coupon as used (User only)
- `GET /coupons` - Get all coupons (Admin only)
- `GET /coupons/{id}` - Get coupon by ID (Admin only)
- `PUT /coupons/{id}` - Update coupon (Admin only)
- `DELETE /coupons/{id}` - Delete coupon (Admin only)

## Database Schema

The application uses SQLite for development with the following tables:

1. **Users** - Stores user information
2. **Coupons** - Stores coupon information
3. **CouponAssignments** - Links coupons to users

## Development

To build the project:
```bash
dotnet build
```

To run tests (when implemented):
```bash
dotnet test
```