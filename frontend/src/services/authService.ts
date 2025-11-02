import { LoginCredentials, AuthState, User } from '../types';

const API_BASE_URL = ''; // Use relative paths with proxy

class AuthService {
  async login(credentials: LoginCredentials): Promise<{ token: string; user: User }> {
    const response = await fetch(`${API_BASE_URL}/auth/token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: new URLSearchParams({
        username: credentials.username,
        password: credentials.password,
      }),
    });

    if (!response.ok) {
      throw new Error('Invalid credentials');
    }

    const data = await response.json();
    // Fix: Use the correct property name from the backend response
    const token = data.AccessToken;
    
    // Get user info from the backend
    const userResponse = await fetch(`${API_BASE_URL}/users/me`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!userResponse.ok) {
      throw new Error('Failed to fetch user info');
    }

    const user = await userResponse.json();
    return { token, user };
  }

  logout(): void {
    // Logout is handled in the auth context
  }
}

export default new AuthService();