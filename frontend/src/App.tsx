import React, { useEffect, useState } from 'react';
import { AuthProvider, useAuth } from './hooks/useAuth';
import Dashboard from './components/Dashboard';
import Login from './components/Login';
import './App.css';
import './components/Login.css';
import './components/Dashboard.css';

// Create a component that handles authentication
const AuthWrapper: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [loading, setLoading] = useState(true);
  const [showLogin, setShowLogin] = useState(false);

  useEffect(() => {
    const authenticateWithWindows = async () => {
      try {
        // Try to authenticate with Windows authentication
        const response = await fetch('http://localhost:8001/auth/windows-auth', {
          method: 'GET',
          credentials: 'include', // This is important for Windows auth
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if (response.ok) {
          const data = await response.json();
          const token = data.accessToken;
          
          // Get user info from the backend
          const userResponse = await fetch('http://localhost:8001/users/me', {
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json',
            },
          });

          if (userResponse.ok) {
            const user = await userResponse.json();
            
            // Save to localStorage
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(user));
            
            // Reload the page to trigger the auth context update
            window.location.reload();
          } else {
            throw new Error('Failed to fetch user info');
          }
        } else {
          // Windows auth failed, show login form for testing
          setShowLogin(true);
        }
      } catch (err: any) {
        // Windows auth failed, show login form for testing
        setShowLogin(true);
      } finally {
        setLoading(false);
      }
    };

    // Only try Windows auth if we don't have a token already
    const token = localStorage.getItem('token');
    if (!token) {
      authenticateWithWindows();
    } else {
      setLoading(false);
    }
  }, []);

  // If user is authenticated, show dashboard
  if (isAuthenticated) {
    return <Dashboard />;
  }

  if (loading) {
    return <div className="login-container">
      <div className="login-form">
        <h2>Authenticating...</h2>
        <p>Please wait while we authenticate you.</p>
      </div>
    </div>;
  }

  if (showLogin) {
    return <Login />;
  }

  return <Dashboard />;
};

function App() {
  return (
    <AuthProvider>
      <div className="App">
        <AuthWrapper />
      </div>
    </AuthProvider>
  );
}

export default App;