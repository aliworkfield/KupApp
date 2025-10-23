import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';

const ManagerDashboard = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<'upload' | 'assign' | 'view'>('upload');

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <h1>Manager Dashboard</h1>
        <div className="user-info">
          <span>Welcome, {user?.username}!</span>
          <button onClick={logout} className="logout-button">Logout</button>
        </div>
      </header>
      <main className="dashboard-content">
        <div className="tabs">
          <button 
            className={activeTab === 'upload' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('upload')}
          >
            Upload Coupons
          </button>
          <button 
            className={activeTab === 'assign' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('assign')}
          >
            Assign Coupons
          </button>
          <button 
            className={activeTab === 'view' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('view')}
          >
            View Coupons
          </button>
        </div>
        
        <div className="tab-content">
          {activeTab === 'upload' && (
            <div className="tab-pane">
              <h2>Upload Coupons from Excel</h2>
              <p>Upload an Excel file to create multiple coupons at once.</p>
              <div className="upload-area">
                <input type="file" accept=".xlsx,.xls" />
                <button className="upload-button">Upload</button>
              </div>
            </div>
          )}
          
          {activeTab === 'assign' && (
            <div className="tab-pane">
              <h2>Assign Coupons to Users</h2>
              <p>Select coupons and users to assign them.</p>
              <div className="assignment-form">
                <div className="form-group">
                  <label>Select Coupon:</label>
                  <select>
                    <option>Coupon 1</option>
                    <option>Coupon 2</option>
                  </select>
                </div>
                <div className="form-group">
                  <label>Select User:</label>
                  <select>
                    <option>User 1</option>
                    <option>User 2</option>
                  </select>
                </div>
                <button className="assign-button">Assign Coupon</button>
              </div>
            </div>
          )}
          
          {activeTab === 'view' && (
            <div className="tab-pane">
              <h2>View Created Coupons</h2>
              <p>View all coupons you've created.</p>
              <div className="coupons-list">
                <div className="coupon-card">
                  <h3>WELCOME10</h3>
                  <p>10% off for new users</p>
                  <span className="status active">Active</span>
                </div>
                <div className="coupon-card">
                  <h3>SAVE20</h3>
                  <p>$20 off any purchase</p>
                  <span className="status inactive">Inactive</span>
                </div>
              </div>
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default ManagerDashboard;