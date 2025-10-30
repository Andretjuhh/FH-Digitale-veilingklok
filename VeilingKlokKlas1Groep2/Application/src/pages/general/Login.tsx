import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../../styles/login.css'; // make sure this path is correct!

function Login() {
	const navigate = useNavigate();
	return (
		<div className="app-page login-page">
			<div className="login-card">
				{/* Back Button */}
				<button aria-label="Back button" className="back-button" onClick={() => navigate('/')}>
					← Back
				</button>
				<div className="login-header">
					<h2 className="login-title" aria-label="Welcome back">
						Welcome Back
					</h2>
					<p className="login-subtitle" aria-label="Please sign in to your account">
						Please sign in to your account
					</p>
				</div>

				<form className="login-form">
					<label>Email</label>
					<input type="email" placeholder="you@example.com" className="input-field" aria-label="Email input field" />

					<label>Password</label>
					<input type="password" placeholder="••••••••" className="input-field" aria-label="Password input field" />

					<button type="submit" className="btn-primary" aria-label="Log in button" onClick={() => navigate('/user-dashboard')}>
						Log In
					</button>

					<button type="button" className="forgot-link" aria-label="Forgot password link">
						Forgot password?
					</button>
					<button type="button" className="register-link" aria-label="Create an account link" onClick={() => navigate('/register')}>
						Create an account
					</button>
				</form>
			</div>
		</div>
	);
}

export default Login;
