import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';

const UserDashboard = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<'unused' | 'used'>('unused');

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
                  <button className="use-button">Use Coupon</button>
                </div>
                <div className="coupon-card">
                  <h3>SAVE20</h3>
                  <p>$20 off any purchase</p>
                  <p className="expires">Expires: 2025-11-30</p>
                  <button className="use-button">Use Coupon</button>
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
                  <span className="status used">Used</span>
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