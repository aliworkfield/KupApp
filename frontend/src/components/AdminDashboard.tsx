import { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { User, Coupon } from '../types';
import CouponService from '../services/couponService';
import UserService from '../services/userService';

const AdminDashboard = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<'users' | 'coupons' | 'reports'>('users');
  const [users, setUsers] = useState<User[]>([]);
  const [coupons, setCoupons] = useState<Coupon[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Form states
  const [newUser, setNewUser] = useState({
    username: '',
    email: '',
    password: '',
    role: 'user' as 'user' | 'manager' | 'admin'
  });
  
  const [editingUser, setEditingUser] = useState<User | null>(null);
  
  const [newCoupon, setNewCoupon] = useState({
    code: '',
    description: '',
    discountAmount: 0,
    discountType: 'fixed' as 'fixed' | 'percentage',
    expirationDate: '',
    isActive: true,
    brand: '',
    assignmentTitle: ''
  });
  
  const [editingCoupon, setEditingCoupon] = useState<Coupon | null>(null);
  
  const [bulkAssignmentTitle, setBulkAssignmentTitle] = useState('');

  // Fetch users when the users tab is active
  useEffect(() => {
    if (activeTab === 'users') {
      fetchUsers();
    }
  }, [activeTab]);

  // Fetch coupons when the coupons tab is active
  useEffect(() => {
    if (activeTab === 'coupons') {
      fetchCoupons();
    }
  }, [activeTab]);

  const fetchUsers = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await UserService.getAllUsers();
      setUsers(data);
    } catch (err) {
      setError('Failed to fetch users');
      console.error('Error fetching users:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchCoupons = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await CouponService.getAllCoupons();
      setCoupons(data);
    } catch (err) {
      setError('Failed to fetch coupons');
      console.error('Error fetching coupons:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      await UserService.createUser(newUser);
      setNewUser({ username: '', email: '', password: '', role: 'user' });
      fetchUsers(); // Refresh the user list
    } catch (err) {
      setError('Failed to create user');
      console.error('Error creating user:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingUser) return;
    
    setLoading(true);
    setError(null);
    try {
      await UserService.updateUser(editingUser.id, {
        email: editingUser.email,
        role: editingUser.role
      });
      setEditingUser(null);
      fetchUsers(); // Refresh the user list
    } catch (err) {
      setError('Failed to update user');
      console.error('Error updating user:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateCoupon = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      await CouponService.createCoupon(newCoupon);
      setNewCoupon({
        code: '',
        description: '',
        discountAmount: 0,
        discountType: 'fixed',
        expirationDate: '',
        isActive: true,
        brand: '',
        assignmentTitle: ''
      });
      fetchCoupons(); // Refresh the coupon list
    } catch (err) {
      setError('Failed to create coupon');
      console.error('Error creating coupon:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateCoupon = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingCoupon) return;
    
    setLoading(true);
    setError(null);
    try {
      await CouponService.updateCoupon(editingCoupon.id, {
        code: editingCoupon.code,
        description: editingCoupon.description,
        discountAmount: editingCoupon.discountAmount,
        discountType: editingCoupon.discountType,
        expirationDate: editingCoupon.expirationDate,
        isActive: editingCoupon.isActive,
        brand: editingCoupon.brand,
        assignmentTitle: editingCoupon.assignmentTitle
      });
      setEditingCoupon(null);
      fetchCoupons(); // Refresh the coupon list
    } catch (err) {
      setError('Failed to update coupon');
      console.error('Error updating coupon:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteUser = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this user?')) {
      setLoading(true);
      setError(null);
      try {
        await UserService.deleteUser(id);
        fetchUsers(); // Refresh the user list
      } catch (err) {
        setError('Failed to delete user');
        console.error('Error deleting user:', err);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleDeleteCoupon = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this coupon?')) {
      setLoading(true);
      setError(null);
      try {
        await CouponService.deleteCoupon(id);
        fetchCoupons(); // Refresh the coupon list
      } catch (err) {
        setError('Failed to delete coupon');
        console.error('Error deleting coupon:', err);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleBulkAssignByTitle = async () => {
    if (!bulkAssignmentTitle) {
      setError('Please enter an assignment title');
      return;
    }
    
    setLoading(true);
    setError(null);
    try {
      await CouponService.assignCouponsByAssignmentTitle({ assignmentTitle: bulkAssignmentTitle });
      setBulkAssignmentTitle('');
      alert('Bulk assignment completed successfully!');
    } catch (err) {
      setError('Failed to perform bulk assignment');
      console.error('Error performing bulk assignment:', err);
    } finally {
      setLoading(false);
    }
  };

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
              {loading && <div className="loading">Loading users...</div>}
              {error && <div className="error">{error}</div>}
              <div className="users-management">
                <div className="users-list">
                  <h3>All Users ({users.length})</h3>
                  <table className="users-table">
                    <thead>
                      <tr>
                        <th>Username</th>
                        <th>Email</th>
                        <th>Role</th>
                        <th>Created At</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {users.map((userData) => (
                        <tr key={userData.id}>
                          <td>{userData.username}</td>
                          <td>{userData.email}</td>
                          <td>{userData.role}</td>
                          <td>{userData.createdAt ? new Date(userData.createdAt).toLocaleDateString() : 'N/A'}</td>
                          <td>
                            <button 
                              className="action-button" 
                              onClick={() => setEditingUser(userData)}
                            >
                              Edit
                            </button>
                            <button 
                              className="action-button delete" 
                              onClick={() => handleDeleteUser(userData.id)}
                            >
                              Delete
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
                
                <div className="user-form">
                  <h3>{editingUser ? 'Edit User' : 'Add New User'}</h3>
                  {editingUser ? (
                    <form onSubmit={handleUpdateUser}>
                      <div className="form-group">
                        <label>Username:</label>
                        <input 
                          type="text" 
                          value={editingUser.username}
                          disabled
                        />
                      </div>
                      <div className="form-group">
                        <label>Email:</label>
                        <input 
                          type="email" 
                          value={editingUser.email}
                          onChange={(e) => setEditingUser({...editingUser, email: e.target.value})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Role:</label>
                        <select 
                          value={editingUser.role}
                          onChange={(e) => setEditingUser({...editingUser, role: e.target.value as 'admin' | 'manager' | 'user'})}
                        >
                          <option value="user">User</option>
                          <option value="manager">Manager</option>
                          <option value="admin">Admin</option>
                        </select>
                      </div>
                      <button type="submit" className="action-button" disabled={loading}>
                        {loading ? 'Updating...' : 'Update User'}
                      </button>
                      <button 
                        type="button" 
                        className="action-button secondary"
                        onClick={() => setEditingUser(null)}
                      >
                        Cancel
                      </button>
                    </form>
                  ) : (
                    <form onSubmit={handleCreateUser}>
                      <div className="form-group">
                        <label>Username:</label>
                        <input 
                          type="text" 
                          value={newUser.username}
                          onChange={(e) => setNewUser({...newUser, username: e.target.value})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Email:</label>
                        <input 
                          type="email" 
                          value={newUser.email}
                          onChange={(e) => setNewUser({...newUser, email: e.target.value})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Password:</label>
                        <input 
                          type="password" 
                          value={newUser.password}
                          onChange={(e) => setNewUser({...newUser, password: e.target.value})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Role:</label>
                        <select 
                          value={newUser.role}
                          onChange={(e) => setNewUser({...newUser, role: e.target.value as 'user' | 'manager' | 'admin'})}
                        >
                          <option value="user">User</option>
                          <option value="manager">Manager</option>
                          <option value="admin">Admin</option>
                        </select>
                      </div>
                      <button type="submit" className="action-button" disabled={loading}>
                        {loading ? 'Creating...' : 'Create User'}
                      </button>
                    </form>
                  )}
                </div>
              </div>
            </div>
          )}
          
          {activeTab === 'coupons' && (
            <div className="tab-pane">
              <h2>Coupon Management</h2>
              {loading && <div className="loading">Loading coupons...</div>}
              {error && <div className="error">{error}</div>}
              <div className="coupons-management">
                <div className="coupons-list">
                  <h3>All Coupons ({coupons.length})</h3>
                  <table className="coupons-table">
                    <thead>
                      <tr>
                        <th>Code</th>
                        <th>Brand</th>
                        <th>Assignment Title</th>
                        <th>Description</th>
                        <th>Discount</th>
                        <th>Status</th>
                        <th>Created At</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {coupons.map((coupon) => (
                        <tr key={coupon.id}>
                          <td>{coupon.code}</td>
                          <td>{coupon.brand}</td>
                          <td>{coupon.assignmentTitle}</td>
                          <td>{coupon.description}</td>
                          <td>{coupon.discountAmount}{coupon.discountType === 'percentage' ? '%' : ''}</td>
                          <td>
                            <span className={`status ${coupon.isActive ? 'active' : 'inactive'}`}>
                              {coupon.isActive ? 'Active' : 'Inactive'}
                            </span>
                          </td>
                          <td>{new Date(coupon.createdAt).toLocaleDateString()}</td>
                          <td>
                            <button 
                              className="action-button" 
                              onClick={() => setEditingCoupon(coupon)}
                            >
                              Edit
                            </button>
                            <button 
                              className="action-button delete" 
                              onClick={() => handleDeleteCoupon(coupon.id)}
                            >
                              Delete
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                  
                  <div className="bulk-assignment-section">
                    <h3>Bulk Assignment by Assignment Title</h3>
                    <div className="form-group">
                      <label>Assignment Title:</label>
                      <input 
                        type="text" 
                        value={bulkAssignmentTitle}
                        onChange={(e) => setBulkAssignmentTitle(e.target.value)}
                        placeholder="Enter assignment title"
                      />
                      <button 
                        className="action-button" 
                        onClick={handleBulkAssignByTitle}
                        disabled={loading || !bulkAssignmentTitle}
                      >
                        {loading ? 'Assigning...' : 'Assign to All Users'}
                      </button>
                    </div>
                  </div>
                </div>
                
                <div className="coupon-form">
                  <h3>{editingCoupon ? 'Edit Coupon' : 'Create New Coupon'}</h3>
                  {editingCoupon ? (
                    <form onSubmit={handleUpdateCoupon}>
                      <div className="form-group">
                        <label>Coupon Code:</label>
                        <input 
                          type="text" 
                          value={editingCoupon.code}
                          onChange={(e) => setEditingCoupon({...editingCoupon, code: e.target.value})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Brand:</label>
                        <input 
                          type="text" 
                          value={editingCoupon.brand}
                          onChange={(e) => setEditingCoupon({...editingCoupon, brand: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>Assignment Title:</label>
                        <input 
                          type="text" 
                          value={editingCoupon.assignmentTitle}
                          onChange={(e) => setEditingCoupon({...editingCoupon, assignmentTitle: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>Description:</label>
                        <input 
                          type="text" 
                          value={editingCoupon.description}
                          onChange={(e) => setEditingCoupon({...editingCoupon, description: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>Discount Amount:</label>
                        <input 
                          type="number" 
                          value={editingCoupon.discountAmount}
                          onChange={(e) => setEditingCoupon({...editingCoupon, discountAmount: parseInt(e.target.value) || 0})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Discount Type:</label>
                        <select 
                          value={editingCoupon.discountType}
                          onChange={(e) => setEditingCoupon({...editingCoupon, discountType: e.target.value as 'percentage' | 'fixed'})}
                        >
                          <option value="percentage">Percentage</option>
                          <option value="fixed">Fixed Amount</option>
                        </select>
                      </div>
                      <div className="form-group">
                        <label>Expiration Date:</label>
                        <input 
                          type="date" 
                          value={editingCoupon.expirationDate || ''}
                          onChange={(e) => setEditingCoupon({...editingCoupon, expirationDate: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>
                          <input 
                            type="checkbox" 
                            checked={editingCoupon.isActive}
                            onChange={(e) => setEditingCoupon({...editingCoupon, isActive: e.target.checked})}
                          />
                          Active
                        </label>
                      </div>
                      <button type="submit" className="action-button" disabled={loading}>
                        {loading ? 'Updating...' : 'Update Coupon'}
                      </button>
                      <button 
                        type="button" 
                        className="action-button secondary"
                        onClick={() => setEditingCoupon(null)}
                      >
                        Cancel
                      </button>
                    </form>
                  ) : (
                    <form onSubmit={handleCreateCoupon}>
                      <div className="form-group">
                        <label>Coupon Code:</label>
                        <input 
                          type="text" 
                          value={newCoupon.code}
                          onChange={(e) => setNewCoupon({...newCoupon, code: e.target.value})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Brand:</label>
                        <input 
                          type="text" 
                          value={newCoupon.brand}
                          onChange={(e) => setNewCoupon({...newCoupon, brand: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>Assignment Title:</label>
                        <input 
                          type="text" 
                          value={newCoupon.assignmentTitle}
                          onChange={(e) => setNewCoupon({...newCoupon, assignmentTitle: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>Description:</label>
                        <input 
                          type="text" 
                          value={newCoupon.description}
                          onChange={(e) => setNewCoupon({...newCoupon, description: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>Discount Amount:</label>
                        <input 
                          type="number" 
                          value={newCoupon.discountAmount}
                          onChange={(e) => setNewCoupon({...newCoupon, discountAmount: parseInt(e.target.value) || 0})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Discount Type:</label>
                        <select 
                          value={newCoupon.discountType}
                          onChange={(e) => setNewCoupon({...newCoupon, discountType: e.target.value as 'percentage' | 'fixed'})}
                        >
                          <option value="percentage">Percentage</option>
                          <option value="fixed">Fixed Amount</option>
                        </select>
                      </div>
                      <div className="form-group">
                        <label>Expiration Date:</label>
                        <input 
                          type="date" 
                          value={newCoupon.expirationDate}
                          onChange={(e) => setNewCoupon({...newCoupon, expirationDate: e.target.value})}
                        />
                      </div>
                      <div className="form-group">
                        <label>
                          <input 
                            type="checkbox" 
                            checked={newCoupon.isActive}
                            onChange={(e) => setNewCoupon({...newCoupon, isActive: e.target.checked})}
                          />
                          Active
                        </label>
                      </div>
                      <button type="submit" className="action-button" disabled={loading}>
                        {loading ? 'Creating...' : 'Create Coupon'}
                      </button>
                    </form>
                  )}
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
                      <span className="stat-value">{users.length}</span>
                      <span className="stat-label">Total Users</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">{users.filter(u => u.role === 'admin').length}</span>
                      <span className="stat-label">Admins</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">{users.filter(u => u.role === 'manager').length}</span>
                      <span className="stat-label">Managers</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">{users.filter(u => u.role === 'user').length}</span>
                      <span className="stat-label">Regular Users</span>
                    </div>
                  </div>
                </div>
                
                <div className="report-card">
                  <h3>Coupon Statistics</h3>
                  <div className="stats">
                    <div className="stat-item">
                      <span className="stat-value">{coupons.length}</span>
                      <span className="stat-label">Total Coupons</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">{coupons.filter(c => c.isActive).length}</span>
                      <span className="stat-label">Active Coupons</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">0</span>
                      <span className="stat-label">Total Assignments</span>
                    </div>
                    <div className="stat-item">
                      <span className="stat-value">0</span>
                      <span className="stat-label">Redeemed Coupons</span>
                    </div>
                  </div>
                </div>
                
                <div className="report-card">
                  <h3>Recent Activity</h3>
                  <ul className="activity-list">
                    <li>No recent activity</li>
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