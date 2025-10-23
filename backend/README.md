# Coupon Management App - Backend

This is the backend for the Coupon Management App built with Python FastAPI.

## Features

- RESTful API with proper error handling
- User authentication with JWT tokens
- Role-based access control (Admin, Manager, User)
- Coupon management with Excel upload functionality
- SQLite database for data persistence

## Prerequisites

- Python 3.8 or higher
- pip (Python package installer)

## Installation

1. Navigate to the backend directory:
   ```
   cd backend
   ```

2. Create a virtual environment:
   ```
   python -m venv venv
   ```

3. Activate the virtual environment:
   - On Windows:
     ```
     venv\Scripts\activate
     ```
   - On macOS/Linux:
     ```
     source venv/bin/activate
     ```

4. Install the required dependencies:
   ```
   pip install -r requirements.txt
   ```

## Database Setup

The application uses SQLite for data persistence. To initialize the database with sample data:

```
python init_db.py
```

This will create:
- An admin user (username: admin, password: admin123)
- A manager user (username: manager, password: manager123)
- A regular user (username: user, password: user123)
- Sample coupons

## Running the Application

To start the development server:

```
uvicorn main:app --reload
```

The API will be available at `http://localhost:8000`.

## API Documentation

Once the server is running, you can access the API documentation at:
- Swagger UI: `http://localhost:8000/docs`
- ReDoc: `http://localhost:8000/redoc`

## Project Structure

```
backend/
├── app/
│   ├── database/     # Database configuration
│   ├── models/       # Database models
│   ├── schemas/      # Pydantic schemas for validation
│   ├── services/     # Business logic
│   └── routes/       # API endpoints
├── main.py           # Application entry point
├── init_db.py        # Database initialization script
├── requirements.txt  # Python dependencies
└── README.md         # This file
```

## API Endpoints

### Authentication
- `POST /auth/token` - Login and obtain access token

### Users (Admin only)
- `GET /users/` - Get all users
- `POST /users/` - Create a new user
- `GET /users/{user_id}` - Get a specific user
- `PUT /users/{user_id}` - Update a user
- `DELETE /users/{user_id}` - Delete a user

### Coupons
- `GET /coupons/` - Get all coupons (Admin only)
- `POST /coupons/` - Create a new coupon (Admin/Manager)
- `POST /coupons/upload-excel` - Upload coupons from Excel (Admin/Manager)
- `POST /coupons/assign` - Assign a coupon to a user (Admin/Manager)
- `GET /coupons/my-coupons` - Get coupons assigned to current user
- `GET /coupons/my-unused-coupons` - Get unused coupons assigned to current user
- `GET /coupons/my-created` - Get coupons created by current manager
- `GET /coupons/unassigned` - Get unassigned coupons (Admin/Manager)
- `POST /coupons/use/{assignment_id}` - Mark a coupon as used