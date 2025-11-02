export interface User {
  id: number;
  username: string;
  email: string;
  role: 'admin' | 'manager' | 'user';
  createdAt: string;
}

export interface Coupon {
  id: number;
  code: string;
  description: string;
  discountAmount: number;
  discountType: 'percentage' | 'fixed';
  expirationDate: string | null;
  isActive: boolean;
  createdAt: string;
  createdById: number;
  // New properties
  brand: string;
  assignmentTitle: string;
}

export interface CouponAssignment {
  id: number;
  couponId: number;
  userId: number;
  isUsed: boolean;
  assignedAt: string;
  usedAt: string | null;
  couponCode: string;
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