import React, { useEffect, useState } from 'react';
import { AuthProvider, useAuth } from './hooks/useAuth';
import Dashboard from './components/Dashboard';
import './App.css';
import './components/Login.css';
import './components/Dashboard.css';

// Create a component that handles Windows authentication
const WindowsAuthWrapper: React.FC = () => {
  const { login } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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
          throw new Error('Windows authentication failed');
        }
      } catch (err: any) {
        setError(err.message || 'Authentication failed');
        console.error('Windows authentication error:', err);
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
  }, [login]);

  if (loading) {
    return <div className="login-container">
      <div className="login-form">
        <h2>Authenticating...</h2>
        <p>Please wait while we authenticate you with Windows credentials.</p>
      </div>
    </div>;
  }

  if (error) {
    return <div className="login-container">
      <div className="login-form">
        <h2>Authentication Failed</h2>
        <p>{error}</p>
        <p>Please ensure you're accessing this application from a domain-joined machine.</p>
      </div>
    </div>;
  }

  return <Dashboard />;
};

function App() {
  return (
    <AuthProvider>
      <div className="App">
        <WindowsAuthWrapper />
      </div>
    </AuthProvider>
  );
}

export default App;