import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';

const AdminDashboard = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<'users' | 'coupons' | 'reports'>('users');

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <h1>Admin Dashboard</h1>
        <div className="user-info">
          <span>Welcome, {user?.username}!</span>
          <button onClick={logout} className="logout-button">Logout</button>
        </div>
      </header>
      <main className="dashboard-content">
        <div className="tabs">
          <button 
            className={activeTab === 'users' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('users')}
          >
            User Management
          </button>
          <button 
            className={activeTab === 'coupons' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('coupons')}
          >
            Coupon Management
          </button>
          <button 
            className={activeTab === 'reports' ? 'tab active' : 'tab'}
            onClick={() => setActiveTab('reports')}
          >
            System Reports
          </button>
        </div>
        
        <div className="tab-content">
          {activeTab === 'users' && (
            <div className="tab-pane">
              <h2>User Management</h2>
              <div className="users-management">
                <div className="users-list">
                  <h3>All Users</h3>
                  <table className="users-table">
                    <thead>
                      <tr>
                        <th>Username</th>
                        <th>Email</th>
                        <th>Role</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td>admin</td>
                        <td>admin@example.com</td>
                        <td>Admin</td>
                        <td>
                          <button className="action-button">Edit</button>
                          <button className="action-button delete">Delete</button>
                        </td>
                      </tr>
                      <tr>
                        <td>manager</td>
                        <td>manager@example.com</td>
                        <td>Manager</td>
                        <td>
                          <button className="action-button">Edit</button>
                          <button className="action-button delete">Delete</button>
                        </td>
                      </tr>
                      <tr>
                        <td>user</td>
                        <td>user@example.com</td>
                        <td>User</td>
                        <td>
                          <button className="action-button">Edit</button>
                          <button className="action-button delete">Delete</button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                
                <div className="user-form">
                  <h3>Add New User</h3>
                  <form>
                    <div className="form-group">
                      <label>Username:</label>
                      <input type="text" />
                    </div>
                    <div className="form-group">
                      <label>Email:</label>
                      <input type="email" />
                    </div>
                    <div className="form-group">
                      <label>Password:</label>
                      <input type="password" />
                    </div>
                    <div className="form-group">
                      <label>Role:</label>
                      <select>
                        <option value="user">User</option>
                        <option value="manager">Manager</option>
                        <option value="admin">Admin</option>
                      </select>
                    </div>
                    <button type="submit" className="action-button">Create User</button>
                  </form>
                </div>
              </div>
            </div>
          )}
          
          {activeTab === 'coupons' && (
            <div className="tab-pane">
              <h2>Coupon Management</h2>
              <div className="coupons-management">
                <div className="coupons-list">
                  <h3>All Coupons</h3>
                  <table className="coupons-table">
                    <thead>
                      <tr>
                        <th>Code</th>
                        <th>Description</th>
                        <th>Discount</th>
                        <th>Status</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td>WELCOME10</td>
                        <td>10% off for new users</td>
                        <td>10%</td>
                        <td><span className="status active">Active</span></td>
                        <td>
                          <button className="action-button">Edit</button>
                          <button className="action-button delete">Delete</button>
                        </td>
                      </tr>
                      <tr>
                        <td>SAVE20</td>
                        <td>$20 off any purchase</td>
                        <td>$20</td>
                        <td><span className="status inactive">Inactive</span></td>
                        <td>
                          <button className="action-button">Edit</button>
                          <button className="action-button delete">Delete</button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                
                <div className="coupon-form">
                  <h3>Create New Coupon</h3>
                  <form>
                    <div className="form-group">
                      <label>Coupon Code:</label>
                      <input type="text" />
                    </div>
                    <div className="form-group">
                      <label>Description:</label>
                      <input type="text" />
                    </div>
                    <div className="form-group">
                      <label>Discount Amount:</label>
                      <input type="number" />
                    </div>
                    <div className="form-group">
                      <label>Discount Type:</label>
                      <select>
                        <option value="percentage">Percentage</option>
                        <option value="fixed">Fixed Amount</option>
                      </select>
                    </div>
                    <div className="form-group">
                      <label>Expiration Date:</label>
                      <input type="date" />
                    </div>
                    <button type="submit" className="action-button">Create Coupon</button>
                  </form>
                </div>
              </div>
            </div>
          )}
          
          {activeTab === 'reports' && (
            <div className="tab-pane">
              <h2>System Reports</h2>
              <div className="reports-section">
                <div className="report-card">
                  <h3>User Statistics</h3>
                  <div className="stats">
                    <div className="stat-item">
                      <span className="stat-value">3</span>
                      <span className="stat-label">Total Users</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">1</span>
                      <span className="stat-label">Admins</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">1</span>
                      <span className="stat-label">Managers</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">1</span>
                      <span className="stat-label">Regular Users</span>
                    </div>
                  </div>
                </div>
                
                <div className="report-card">
                  <h3>Coupon Statistics</h3>
                  <div className="stats">
                    <div className="stat-item">
                      <span className="stat-value">2</span>
                      <span className="stat-label">Total Coupons</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">1</span>
                      <span className="stat-label">Active Coupons</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">5</span>
                      <span className="stat-label">Total Assignments</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">2</span>
                      <span className="stat-label">Redeemed Coupons</span>
                    </div>
                  </div>
                </div>
                
                <div className="report-card">
                  <h3>Recent Activity</h3>
                  <ul className="activity-list">
                    <li>User "user" redeemed coupon "WELCOME10"</li>
                    <li>Manager "manager" created coupon "SAVE20"</li>
                    <li>Admin "admin" updated user "user" role</li>
                  </ul>
                </div>
              </div>
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default AdminDashboard;