import { Coupon, CouponAssignment } from '../types';

const API_BASE_URL = ''; // Use relative paths with proxy

class CouponService {
  private token: string | null = null;

  setToken(token: string): void {
    this.token = token;
  }

  private getAuthHeaders(): HeadersInit {
    return {
      'Authorization': `Bearer ${this.token}`,
      'Content-Type': 'application/json',
    };
  }

  // Admin/Manager: Get all coupons
  async getAllCoupons(): Promise<Coupon[]> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch coupons: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching coupons:', error);
      throw error;
    }
  }

  // User: Get my coupons
  async getMyCoupons(): Promise<Coupon[]> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/my-coupons`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch my coupons: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching my coupons:', error);
      throw error;
    }
  }

  // User: Get my unused coupons
  async getMyUnusedCoupons(): Promise<Coupon[]> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/my-unused-coupons`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch my unused coupons: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching my unused coupons:', error);
      throw error;
    }
  }

  // Manager: Get coupons I created
  async getMyCreatedCoupons(): Promise<Coupon[]> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/my-created`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch my created coupons: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching my created coupons:', error);
      throw error;
    }
  }

  // Manager/Admin: Get unassigned coupons
  async getUnassignedCoupons(): Promise<Coupon[]> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/unassigned`, {
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch unassigned coupons: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching unassigned coupons:', error);
      throw error;
    }
  }

  // Manager/Admin: Create a new coupon
  async createCoupon(couponData: Partial<Coupon>): Promise<Coupon> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/`, {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(couponData),
      });

      if (!response.ok) {
        throw new Error(`Failed to create coupon: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error creating coupon:', error);
      throw error;
    }
  }

  // Manager/Admin: Update a coupon
  async updateCoupon(id: number, couponData: Partial<Coupon>): Promise<Coupon> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/${id}`, {
        method: 'PUT',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(couponData),
      });

      if (!response.ok) {
        throw new Error(`Failed to update coupon: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error updating coupon:', error);
      throw error;
    }
  }

  // Manager/Admin: Delete a coupon
  async deleteCoupon(id: number): Promise<void> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/${id}`, {
        method: 'DELETE',
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to delete coupon: ${response.status} ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error deleting coupon:', error);
      throw error;
    }
  }

  // Manager/Admin: Upload coupons from Excel
  async uploadCouponsFromExcel(file: File): Promise<Coupon[]> {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await fetch(`${API_BASE_URL}/coupons/upload-excel`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.token}`,
        },
        body: formData,
      });

      if (!response.ok) {
        throw new Error(`Failed to upload coupons: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error uploading coupons:', error);
      throw error;
    }
  }

  // Manager/Admin: Assign coupon to user
  async assignCoupon(assignmentData: { coupon_id: number; user_id: number }): Promise<any> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/assign`, {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(assignmentData),
      });

      if (!response.ok) {
        throw new Error(`Failed to assign coupon: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error assigning coupon:', error);
      throw error;
    }
  }

  // Manager/Admin: Bulk assign coupons by assignment title
  async assignCouponsByAssignmentTitle(assignmentData: { assignmentTitle: string }): Promise<any> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/assign-by-title`, {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(assignmentData),
      });

      if (!response.ok) {
        throw new Error(`Failed to bulk assign coupons: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error bulk assigning coupons:', error);
      throw error;
    }
  }

  // User: Mark coupon as used
  async useCoupon(assignmentId: number): Promise<any> {
    try {
      const response = await fetch(`${API_BASE_URL}/coupons/use/${assignmentId}`, {
        method: 'POST',
        headers: this.getAuthHeaders(),
      });

      if (!response.ok) {
        throw new Error(`Failed to mark coupon as used: ${response.status} ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error marking coupon as used:', error);
      throw error;
    }
  }
}

export default new CouponService();