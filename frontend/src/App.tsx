import { AuthProvider } from './hooks/useAuth'
import Login from './components/Login'
import './App.css'
import './components/Login.css'
import './components/Dashboard.css'

function App() {
  // For now, we'll just show the login page
  // In a real app, you'd check auth state and show appropriate component
  return (
    <AuthProvider>
      <div className="App">
        <Login />
      </div>
    </AuthProvider>
  )
}

export default App