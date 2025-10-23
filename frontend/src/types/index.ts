export interface User {
  id: number;
  username: string;
  email: string;
  role: 'admin' | 'manager' | 'user';
  created_at?: string;
}

export interface Coupon {
  id: number;
  code: string;
  description: string;
  discount_amount: number;
  discount_type: 'percentage' | 'fixed';
  expiration_date: string | null;
  is_active: boolean;
  created_at: string;
  created_by: number;
}

export interface CouponAssignment {
  id: number;
  coupon_id: number;
  user_id: number;
  is_used: boolean;
  assigned_at: string;
  used_at: string | null;
  coupon_code: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
}

export interface LoginCredentials {
  username: string;
  password: string;
}