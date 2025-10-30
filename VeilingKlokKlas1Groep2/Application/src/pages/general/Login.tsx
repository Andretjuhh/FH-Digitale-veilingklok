import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../../styles/login.css'; // make sure this path is correct!

function Login() {
	const navigate = useNavigate();
	return (
		<div className="login-page">
			<div className="login-card">
				{/* Back Button */}
				<button className="back-button" aria-label="Back button brings you back to previous page" onClick={() => navigate('/')}>
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
					<input type="email" placeholder="you@example.com" className="input-field" aria-label="Input field for email" />

					<label>Password</label>
					<input type="password" placeholder="••••••••" className="input-field" aria-label="Input field for password" />

					<button type="submit" aria-label="Login button" className="btn-primary" onClick={() => navigate('/')}>
						Log In
					</button>

					<button type="button" aria-label="Forgot password link" className="forgot-link">
						Forgot password?
					</button>
					<button type="button" aria-label="Create an account link" className="register-link" onClick={() => navigate('/register')}>
						Create an account
					</button>
				</form>
			</div>
		</div>
	);
}

export default Login;
