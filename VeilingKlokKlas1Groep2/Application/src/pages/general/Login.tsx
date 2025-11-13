// External imports
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';

// Internal imports
import { LoginRequest } from '../../declarations/LoginRequest';
import { useRootContext } from '../../contexts/RootContext';
import { FetchError } from '../../types/FetchError';

function Login() {
	const navigate = useNavigate();
	const { authenticate } = useRootContext();

	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm<LoginRequest>({
		defaultValues: { email: '', password: '' },
		mode: 'onTouched',
	});

	const [submitting, setSubmitting] = React.useState(false);
	const [submitError, setSubmitError] = React.useState<string | undefined>();

	const onSubmit = async (data: LoginRequest) => {
		setSubmitError(undefined);
		setSubmitting(true);
		try {
			// Delegate to auth flow; endpoint wiring can be adjusted later
			await authenticate(new LoginRequest(data.email, data.password));
			// Navigate to dashboard on success (adjust route as needed)
			navigate('/user-dashboard');
		} catch (err: any) {
			const e = err as FetchError<any>;
			if (typeof e?.data === 'string') setSubmitError(e.data);
			else if (e?.statusCode) setSubmitError(`Login failed (${e.statusCode}). Please try again.`);
			else setSubmitError('Login failed. Please check your credentials and try again.');
		} finally {
			setSubmitting(false);
		}
	};

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

				<form className="login-form" onSubmit={handleSubmit(onSubmit)} noValidate>
					<label htmlFor="email">Email</label>
					<input
						id="email"
						type="email"
						placeholder="you@example.com"
						className="input-field"
						aria-label="Email input field"
						aria-invalid={!!errors.email || undefined}
						{...register('email', {
							required: 'Email is required',
							pattern: {
								value: /[^\s@]+@[^\s@]+\.[^\s@]+/,
								message: 'Enter a valid email address',
							},
						})}
					/>
					{errors.email?.message && (
						<p role="alert" className="field-error">
							{errors.email.message}
						</p>
					)}

					<label htmlFor="password">Password</label>
					<input
						id="password"
						type="password"
						placeholder="••••••••"
						className="input-field"
						aria-label="Password input field"
						aria-invalid={!!errors.password || undefined}
						{...register('password', {
							required: 'Password is required',
							minLength: { value: 6, message: 'Password must be at least 6 characters' },
						})}
					/>
					{errors.password?.message && (
						<p role="alert" className="field-error">
							{errors.password.message}
						</p>
					)}

					{submitError && (
						<div role="alert" className="form-error" aria-live="polite">
							{submitError}
						</div>
					)}

					<button type="submit" className="btn-primary" aria-label="Log in button" disabled={submitting}>
						{submitting ? 'Logging in…' : 'Log In'}
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
