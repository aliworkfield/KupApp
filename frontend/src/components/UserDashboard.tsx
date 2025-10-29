import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import CouponService from '../services/couponService';

const UserDashboard = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<'unused' | 'used'>('unused');

  const handleUseCoupon = async (assignmentId: number) => {
    try {
      await CouponService.useCoupon(assignmentId);
      // In a real app, we would update the UI to reflect the change
      alert('Coupon marked as used successfully!');
      // Refresh the data
      window.location.reload();
    } catch (error) {
      alert('Failed to mark coupon as used');
    }
  };

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <h1>User Dashboard</h1>
        <div className="user-info">
          <span>Welcome, {user?.username}!</span>
          <button onClick={logout} className="logout-button">Logout</button>
        </div>
      </header>
      <main className="dashboard-content">
        <div className="tabs">
          <button 
            className={activeTab === 'unused' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('unused')}
          >
            Unused Coupons
          </button>
          <button 
            className={activeTab === 'used' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('used')}
          >
            Used Coupons
          </button>
        </div>
        
        <div className="tab-content">
          {activeTab === 'unused' && (
            <div className="tab-pane">
              <h2>Your Unused Coupons</h2>
              <div className="coupons-list">
                <div className="coupon-card">
                  <h3>WELCOME10</h3>
                  <p>10% off for new users</p>
                  <p className="expires">Expires: 2025-12-31</p>
                  <div className="coupon-footer">
                    <span className="discount-type">Percentage</span>
                    <button 
                      className="use-button" 
                      onClick={() => handleUseCoupon(1)}
                    >
                      Use Coupon
                    </button>
                  </div>
                </div>
                <div className="coupon-card">
                  <h3>SAVE20</h3>
                  <p>$20 off any purchase</p>
                  <p className="expires">Expires: 2025-11-30</p>
                  <div className="coupon-footer">
                    <span className="discount-type">Fixed Amount</span>
                    <button 
                      className="use-button" 
                      onClick={() => handleUseCoupon(2)}
                    >
                      Use Coupon
                    </button>
                  </div>
                </div>
              </div>
            </div>
          )}
          
          {activeTab === 'used' && (
            <div className="tab-pane">
              <h2>Your Used Coupons</h2>
              <div className="coupons-list">
                <div className="coupon-card used">
                  <h3>SAVE15</h3>
                  <p>15% off any purchase</p>
                  <p className="used-date">Used: 2025-10-15</p>
                  <div className="coupon-footer">
                    <span className="discount-type">Percentage</span>
                    <span className="status used">Used</span>
                  </div>
                </div>
                <div className="coupon-card used">
                  <h3>FIRST50</h3>
                  <p>$50 off first purchase</p>
                  <p className="used-date">Used: 2025-09-22</p>
                  <div className="coupon-footer">
                    <span className="discount-type">Fixed Amount</span>
                    <span className="status used">Used</span>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default UserDashboard;