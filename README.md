# Coupon Management App

A professional coupon management application built with React/TypeScript frontend and Python FastAPI backend.

## Features

- Three user roles: Admin, Manager, and User
- Admin: Full access including user management
- Manager: Upload coupons via Excel, assign to users, view assigned and unused coupons
- User: View coupons assigned to them

## Project Structure

```
coupon-app/
├── frontend/          # React/TypeScript frontend
└── backend/           # Python FastAPI backend
```

## Getting Started

### Backend
1. Navigate to the `backend` directory
2. Create a virtual environment: `python -m venv venv`
3. Activate it: `venv\Scripts\activate` (Windows) or `source venv/bin/activate` (Linux/Mac)
4. Install dependencies: `pip install -r requirements.txt`
5. Initialize the database: `python init_db.py`
6. Run the server: `uvicorn main:app --reload`

### Frontend
1. Navigate to the `frontend` directory
2. Install dependencies: `npm install`
3. Run the development server: `npm run dev`

## Architecture

This application follows a clean architecture pattern with separation of concerns:
- Backend uses service layer pattern
- Frontend follows component-based architecture
- RESTful API design

## Detailed Documentation

For more detailed information about each part of the application, please see:
- [Backend Documentation](backend/README.md)
- [Frontend Documentation](frontend/README.md)

## API Endpoints

The backend provides a complete RESTful API for all application functionality. When the backend server is running, you can access the interactive API documentation at:
- Swagger UI: `http://localhost:8000/docs`
- ReDoc: `http://localhost:8000/redoc`

## Default Users

After initializing the database, the following users will be available:

| Role    | Username | Password   |
|---------|----------|------------|
| Admin   | admin    | admin123   |
| Manager | manager  | manager123 |
| User    | user     | user123    |

## Technology Stack

### Backend
- Python 3.8+
- FastAPI (Web framework)
- SQLAlchemy (ORM)
- SQLite (Database)
- JWT (Authentication)
- Pandas (Excel processing)

### Frontend
- React 18+
- TypeScript
- Vite (Build tool)
- CSS (Styling)

## Development

### Backend Development

The backend follows a clean architecture with the following layers:

1. **Models** - Database models and schema definitions
2. **Schemas** - Pydantic models for request/response validation
3. **Services** - Business logic and data processing
4. **Routes** - API endpoint definitions
5. **Database** - Database configuration and connection management

### Frontend Development

The frontend follows a component-based architecture with:

1. **Components** - Reusable UI components
2. **Hooks** - Custom React hooks for state and logic
3. **Services** - API communication layer
4. **Types** - TypeScript type definitions

## Deployment

For production deployment, you would typically:

1. Build the frontend: `npm run build`
2. Serve the built frontend files through a web server
3. Run the backend with a production WSGI server like Gunicorn
4. Use a production database like PostgreSQL instead of SQLite
5. Set up proper environment variables and security measures