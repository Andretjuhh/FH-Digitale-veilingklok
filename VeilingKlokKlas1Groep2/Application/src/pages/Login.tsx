import React from "react";
import { useNavigate } from 'react-router-dom';
import "../styles/login.css"; // make sure this path is correct!

function Login() {
  const navigate = useNavigate();
  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-header">
          <h2 className="login-title">Welcome Back</h2>
          <p className="login-subtitle">Please sign in to your account</p>
        </div>

        <form className="login-form">
          <label>Email</label>
          <input type="email" placeholder="you@example.com" className="input-field" />

          <label>Password</label>
          <input type="password" placeholder="••••••••" className="input-field" />

          <button type="submit" className="btn-primary">
            Log In
          </button>

          <button type="button" className="forgot-link">
            Forgot password?
          </button>
          <button type="button" className="register-link" 					
            onClick={() => navigate('/register')}
          >
            Create an account
          </button>
        </form>
      </div>
    </div>
  );
}

export default Login;
