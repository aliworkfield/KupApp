# Coupon Management App

A professional coupon management application built with React/TypeScript frontend and .NET backend.

## Features

- Three user roles: Admin, Manager, and User
- **Admin**: Full access including user management, coupon management, and system reports
- **Manager**: Upload coupons via Excel, assign to individual users or all users, view created coupons
- **User**: View and redeem coupons assigned to them
- **LDAP Authentication**: Domain users can authenticate with corporate credentials
- **Excel Import**: Bulk upload coupons from Excel files
- **Auto-Assignment**: Assign coupons to all users with a single click

## Project Structure

```
coupon-app/
├── dotnet-backend/    # .NET 8 backend
│   └── CouponApp/     # Main application
│       ├── Controllers/    # API controllers
│       ├── Data/           # Database context
│       ├── DTOs/           # Data transfer objects
│       ├── Models/         # Entity models
│       ├── Services/       # Business logic services
│       └── Properties/     # Configuration files
├── frontend/          # React/TypeScript frontend
│   ├── public/        # Static assets
│   └── src/           # Source code
│       ├── components/     # React components
│       ├── hooks/          # Custom hooks
│       ├── services/       # API services
│       └── types/          # TypeScript types
└── database/          # Database files
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 16+
- npm 8+

### Backend (.NET)

1. Navigate to the `dotnet-backend/CouponApp` directory
2. Restore dependencies: `dotnet restore`
3. Build the project: `dotnet build`
4. Run the server: `dotnet run`

The backend will start on `http://localhost:8001` by default.

### Frontend (React/TypeScript)

1. Navigate to the `frontend` directory
2. Install dependencies: `npm install`
3. Run the development server: `npm run dev`

The frontend will start on `http://localhost:3000` by default.

## Architecture

This application follows a clean architecture pattern with separation of concerns:

### Backend Architecture
- **Controllers**: Handle HTTP requests and responses
- **Services**: Contain business logic and data processing
- **Data**: Database context and entity framework configuration
- **Models**: Entity models representing database tables
- **DTOs**: Data transfer objects for API communication
- **Dependency Injection**: Used throughout for loose coupling

### Frontend Architecture
- **Components**: Reusable UI components organized by feature
- **Hooks**: Custom React hooks for state and logic
- **Services**: API communication layer
- **Types**: TypeScript type definitions for type safety

## API Endpoints

The backend provides a complete RESTful API for all application functionality. When the backend server is running, you can access the interactive API documentation at:
- Swagger UI: `http://localhost:8001/swagger`
- Direct access: `http://localhost:8001`

### Authentication
- `POST /auth/token` - Authenticate user and receive JWT token

### User Management (Admin only)
- `GET /users` - Get all users
- `GET /users/{id}` - Get specific user
- `POST /users` - Create new user
- `PUT /users/{id}` - Update user
- `DELETE /users/{id}` - Delete user

### Coupon Management
- `GET /coupons` - Get all coupons (Admin only)
- `POST /coupons` - Create new coupon (Admin only)
- `GET /coupons/{id}` - Get specific coupon (Admin only)
- `PUT /coupons/{id}` - Update coupon (Admin only)
- `DELETE /coupons/{id}` - Delete coupon (Admin only)

### Manager Features
- `POST /coupons/upload-excel` - Upload coupons from Excel file
- `POST /coupons/assign` - Assign coupon to specific user
- `POST /coupons/assign-to-all` - Assign coupon to all users
- `GET /coupons/my-created` - Get coupons created by current manager
- `GET /coupons/unassigned` - Get unassigned coupons

### User Features
- `GET /coupons/my-coupons` - Get all coupons assigned to current user
- `GET /coupons/my-unused-coupons` - Get unused coupons assigned to current user
- `POST /coupons/use/{assignmentId}` - Mark coupon as used

## Default Users

After initializing the database, the following users will be available:

| Role    | Username | Password   |
|---------|----------|------------|
| Admin   | admin    | admin123   |
| Manager | manager  | manager123 |
| User    | user     | user123    |

## Technology Stack

### Backend
- .NET 8
- C#
- Entity Framework Core (ORM)
- SQLite (Development), SQL Server/PostgreSQL (Production)
- JWT (Authentication)
- ClosedXML (Excel processing)
- System.DirectoryServices.Protocols (LDAP)

### Frontend
- React 18+
- TypeScript
- Vite (Build tool)
- CSS (Styling)

## Excel Import Format

To import coupons via Excel, create a file with the following columns:
1. **Code** (Required) - Unique coupon code
2. **Description** (Optional) - Description of the coupon
3. **Discount Amount** (Required) - Numeric discount value
4. **Discount Type** (Required) - "percentage" or "fixed"
5. **Expiration Date** (Optional) - Date in YYYY-MM-DD format

Example:
| Code | Description | Discount Amount | Discount Type | Expiration Date |
|------|-------------|-----------------|---------------|-----------------|
| WELCOME10 | 10% off for new users | 10 | percentage | 2025-12-31 |
| SAVE20 | $20 off any purchase | 20 | fixed | 2025-11-30 |

## LDAP Configuration

To configure LDAP authentication, update the `appsettings.json` file in the backend:

```json
"Ldap": {
  "Server": "your-ldap-server.com",
  "Port": "389",
  "BaseDn": "dc=yourcompany,dc=com",
  "UseSsl": "false"
}
```

## Next Steps and Recommendations

### 1. Security Enhancements
- Implement HTTPS in production
- Add rate limiting to prevent abuse
- Implement more robust password policies
- Add two-factor authentication (2FA)
- Regularly rotate JWT secret keys

### 2. Performance Improvements
- Add database indexing for frequently queried fields
- Implement caching for frequently accessed data
- Add pagination for large data sets
- Optimize Excel import for large files

### 3. Feature Enhancements
- Add coupon categories and filtering
- Implement coupon usage analytics and reporting
- Add email notifications for coupon assignments
- Create coupon templates for recurring campaigns
- Add coupon expiration notifications

### 4. Deployment Considerations
- Set up CI/CD pipeline for automated deployments
- Configure proper logging and monitoring
- Implement database backups
- Set up staging environment for testing
- Containerize application with Docker

### 5. Testing
- Add unit tests for backend services
- Add integration tests for API endpoints
- Add end-to-end tests for frontend components
- Implement code coverage requirements

### 6. Documentation
- Create user guides for each role
- Document API endpoints with examples
- Create deployment and maintenance guides
- Add troubleshooting documentation

## Development

### Backend Development

The backend follows a clean architecture with dependency injection:

1. **Models** - Entity models representing database tables
2. **DTOs** - Data transfer objects for API communication
3. **Data** - Database context and configuration
4. **Services** - Business logic and data processing
5. **Controllers** - API endpoint definitions

To add a new feature:
1. Create models if needed
2. Create DTOs for request/response
3. Add service methods
4. Add controller endpoints
5. Update database context if needed

### Frontend Development

The frontend follows a component-based architecture:

1. **Components** - Reusable UI components organized by feature
2. **Hooks** - Custom React hooks for state and logic
3. **Services** - API communication layer
4. **Types** - TypeScript type definitions

To add a new feature:
1. Create new components in the appropriate directory
2. Add necessary types
3. Create service methods for API calls
4. Add routes if needed
5. Update state management as required

## Deployment

For production deployment, you would typically:

1. Build the frontend: `npm run build`
2. Serve the built frontend files through a web server (nginx, Apache, etc.)
3. Deploy the backend as a service or container
4. Use a production database like SQL Server or PostgreSQL instead of SQLite
5. Configure proper environment variables and security measures
6. Set up reverse proxy (nginx, IIS) for HTTPS termination
7. Implement monitoring and logging solutions

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.