import { useAuth } from './useAuth';

interface ApiClient {
  get: <T>(url: string) => Promise<{ data: T }>;
  post: <T>(url: string, data: any) => Promise<{ data: T }>;
  put: <T>(url: string, data: any) => Promise<{ data: T }>;
  delete: <T>(url: string) => Promise<{ data: T }>;
}

export function useApi(): { api: ApiClient } {
  const { token } = useAuth();
  const API_BASE_URL = 'http://localhost:8001';

  const api: ApiClient = {
    get: async <T>(url: string) => {
      const response = await fetch(`${API_BASE_URL}${url}`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch ${url}: ${response.status} ${response.statusText}`);
      }

      const data = await response.json();
      return { data };
    },

    post: async <T>(url: string, data: any) => {
      const response = await fetch(`${API_BASE_URL}${url}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        throw new Error(`Failed to post ${url}: ${response.status} ${response.statusText}`);
      }

      const result = await response.json();
      return { data: result };
    },

    put: async <T>(url: string, data: any) => {
      const response = await fetch(`${API_BASE_URL}${url}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        throw new Error(`Failed to put ${url}: ${response.status} ${response.statusText}`);
      }

      const result = await response.json();
      return { data: result };
    },

    delete: async <T>(url: string) => {
      const response = await fetch(`${API_BASE_URL}${url}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to delete ${url}: ${response.status} ${response.statusText}`);
      }

      const result = await response.json();
      return { data: result };
    },
  };

  return { api };
}