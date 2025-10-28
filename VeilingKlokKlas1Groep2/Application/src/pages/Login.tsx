import React from "react";
import "../styles/login.css"; // make sure this path is correct!

function Login() {
  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-header">
          <div className="login-icon">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M12 11c0-2.21 1.79-4 4-4s4 1.79 4 4v1h-8v-1zM5 12h14v6H5z" />
            </svg>
          </div>
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
        </form>
      </div>
    </div>
  );
}

export default Login;
