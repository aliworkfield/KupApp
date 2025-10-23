# Coupon Management App - Frontend

This is the frontend for the Coupon Management App built with React and TypeScript.

## Features

- Responsive user interface
- Role-based dashboards (Admin, Manager, User)
- User authentication with JWT tokens
- Coupon management interface
- Excel upload functionality for managers

## Prerequisites

- Node.js 14 or higher
- npm (Node package manager)

## Installation

1. Navigate to the frontend directory:
   ```
   cd frontend
   ```

2. Install the required dependencies:
   ```
   npm install
   ```

## Running the Application

To start the development server:

```
npm run dev
```

The application will be available at `http://localhost:3000`.

## Building for Production

To create a production build:

```
npm run build
```

To preview the production build:

```
npm run preview
```

## Project Structure

```
frontend/
├── public/           # Static assets
├── src/
│   ├── components/   # React components
│   ├── hooks/        # Custom React hooks
│   ├── services/     # API service functions
│   ├── types/        # TypeScript types
│   ├── utils/        # Utility functions
│   ├── App.tsx       # Main App component
│   └── main.tsx      # Entry point
├── index.html        # HTML template
├── tsconfig.json     # TypeScript configuration
├── vite.config.ts    # Vite configuration
└── README.md         # This file
```

## Development

### Components

The application is organized into the following main components:

1. **Login** - Authentication interface
2. **Dashboard** - Main application interface with role-based views:
   - Admin Dashboard
   - Manager Dashboard
   - User Dashboard

### Services

API calls are handled through service classes:
- `authService.ts` - Authentication related API calls
- `couponService.ts` - Coupon related API calls

### Hooks

Custom React hooks:
- `useAuth.ts` - Authentication context and state management

## Styling

The application uses plain CSS for styling with a component-based approach. Each component has its own CSS file for scoped styling.

## Environment Variables

The application uses the following environment variables:

- `VITE_API_BASE_URL` - Base URL for the backend API (default: http://localhost:8000)

To set environment variables, create a `.env` file in the frontend directory.