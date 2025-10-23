import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { AuthState, User, LoginCredentials } from '../types';
import AuthService from '../services/authService';
import CouponService from '../services/couponService';

interface AuthContextType extends AuthState {
  login: (credentials: LoginCredentials) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [authState, setAuthState] = useState<AuthState>({
    user: null,
    token: null,
    isAuthenticated: false,
  });

  useEffect(() => {
    // Check if user is already logged in (from localStorage, etc.)
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    
    if (token && user) {
      try {
        const parsedUser = JSON.parse(user);
        setAuthState({
          user: parsedUser,
          token,
          isAuthenticated: true,
        });
        CouponService.setToken(token);
      } catch (error) {
        // If there's an error parsing, clear the localStorage
        localStorage.removeItem('token');
        localStorage.removeItem('user');
      }
    }
  }, []);

  const login = async (credentials: LoginCredentials) => {
    try {
      // Validate input
      if (!credentials.username || !credentials.password) {
        throw new Error('Username and password are required');
      }
      
      const { token, user } = await AuthService.login(credentials);
      
      setAuthState({
        user,
        token,
        isAuthenticated: true,
      });
      
      // Save to localStorage
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
      
      // Set token in services
      CouponService.setToken(token);
    } catch (error) {
      // Re-throw the error so it can be handled by the calling component
      throw error;
    }
  };

  const logout = () => {
    AuthService.logout();
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    
    setAuthState({
      user: null,
      token: null,
      isAuthenticated: false,
    });
  };

  return React.createElement(AuthContext.Provider, { value: { ...authState, login, logout } }, children);
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}