import { useAuth } from '../hooks/useAuth';

const AdminDashboard = () => {
  const { user, logout } = useAuth();

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
        <div className="dashboard-section">
          <h2>User Management</h2>
          <p>Admins can manage all users in the system.</p>
          <button className="action-button">Manage Users</button>
        </div>
        <div className="dashboard-section">
          <h2>Coupon Management</h2>
          <p>Admins can create, edit, and delete all coupons.</p>
          <button className="action-button">Manage Coupons</button>
        </div>
        <div className="dashboard-section">
          <h2>System Reports</h2>
          <p>View system-wide reports and analytics.</p>
          <button className="action-button">View Reports</button>
        </div>
      </main>
    </div>
  );
};

export default AdminDashboard;