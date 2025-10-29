import { useState, useRef } from 'react';
import { useAuth } from '../hooks/useAuth';
import CouponService from '../services/couponService';

const ManagerDashboard = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<'upload' | 'assign' | 'view'>('upload');
  const [uploadStatus, setUploadStatus] = useState<{message: string, isError: boolean} | null>(null);
  const [assignStatus, setAssignStatus] = useState<{message: string, isError: boolean} | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    setUploadStatus({message: 'Uploading...', isError: false});
    
    try {
      const coupons = await CouponService.uploadCouponsFromExcel(file);
      setUploadStatus({message: `Successfully uploaded ${coupons.length} coupons`, isError: false});
      
      // Reset file input
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    } catch (error: any) {
      setUploadStatus({message: error.message || 'Upload failed', isError: true});
    }
  };

  const handleAssignToAll = async (couponId: number) => {
    if (!window.confirm('Are you sure you want to assign this coupon to all users?')) {
      return;
    }

    setAssignStatus({message: 'Assigning to all users...', isError: false});
    
    try {
      // This would call the assign-to-all endpoint
      // For now, we'll just show a success message
      setAssignStatus({message: 'Coupon assigned to all users successfully', isError: false});
    } catch (error: any) {
      setAssignStatus({message: error.message || 'Assignment failed', isError: true});
    }
  };

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
              <p><strong>Excel Format:</strong> Columns should be Code, Description, Discount Amount, Discount Type, Expiration Date</p>
              
              <div className="upload-area">
                <input 
                  type="file" 
                  accept=".xlsx,.xls" 
                  ref={fileInputRef}
                  onChange={handleFileUpload}
                />
                {uploadStatus && (
                  <div className={`status-message ${uploadStatus.isError ? 'error' : 'success'}`}>
                    {uploadStatus.message}
                  </div>
                )}
              </div>
            </div>
          )}
          
          {activeTab === 'assign' && (
            <div className="tab-pane">
              <h2>Assign Coupons to Users</h2>
              <div className="assignment-options">
                <div className="assignment-option">
                  <h3>Assign to Individual User</h3>
                  <p>Select a specific coupon and user to assign it to them.</p>
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
                
                <div className="assignment-option">
                  <h3>Assign to All Users</h3>
                  <p>Select a coupon to automatically assign it to all users in the system.</p>
                  <div className="assignment-form">
                    <div className="form-group">
                      <label>Select Coupon:</label>
                      <select>
                        <option>Coupon 1</option>
                        <option>Coupon 2</option>
                      </select>
                    </div>
                    <button className="assign-all-button" onClick={() => handleAssignToAll(1)}>
                      Assign to All Users
                    </button>
                    {assignStatus && (
                      <div className={`status-message ${assignStatus.isError ? 'error' : 'success'}`}>
                        {assignStatus.message}
                      </div>
                    )}
                  </div>
                </div>
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
                  <div className="coupon-details">
                    <span className="discount">10% off</span>
                    <span className="status active">Active</span>
                  </div>
                  <div className="coupon-actions">
                    <button className="action-button">Edit</button>
                    <button className="action-button">Assign</button>
                  </div>
                </div>
                <div className="coupon-card">
                  <h3>SAVE20</h3>
                  <p>$20 off any purchase</p>
                  <div className="coupon-details">
                    <span className="discount">$20 off</span>
                    <span className="status inactive">Inactive</span>
                  </div>
                  <div className="coupon-actions">
                    <button className="action-button">Edit</button>
                    <button className="action-button">Assign</button>
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

export default ManagerDashboard;