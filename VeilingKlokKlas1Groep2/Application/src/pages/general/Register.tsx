import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../../styles/register.css';

type UserType = 'koper' | 'kweker' | 'veilingmeester';

interface InputField {
	label: string;
	type: string;
	placeholder?: string;
	options?: string[];
}

function Register() {
	const navigate = useNavigate();
	const [userType, setUserType] = useState<UserType>('koper');
	const [step, setStep] = useState(1);
	const [formData, setFormData] = useState<Record<string, string>>({});

	const regions = ['Noord-Holland', 'Zuid-Holland', 'Utrecht', 'Gelderland', 'Overijssel', 'Limburg', 'Friesland', 'Drenthe', 'Flevoland', 'Groningen', 'Zeeland'];

	const steps: Record<UserType, InputField[][]> = {
		koper: [
			[
				{ label: 'Email', type: 'email', placeholder: 'you@example.com' },
				{ label: 'Password', type: 'password', placeholder: '••••••••' },
				{ label: 'First Name', type: 'text', placeholder: 'Steve' },
				{ label: 'Last Name', type: 'text', placeholder: 'Jobs' },
			],
			[
				{ label: 'Address', type: 'text', placeholder: 'Street 123' },
				{ label: 'Postcode', type: 'text', placeholder: '1234 AB' },
				{ label: 'Region', type: 'select', options: regions },
			],
		],
		kweker: [
			[
				{ label: 'Company Name', type: 'text', placeholder: 'Example BV' },
				{ label: 'Email', type: 'email', placeholder: 'you@example.com' },
				{ label: 'Password', type: 'password', placeholder: '••••••••' },
				{ label: 'kvk Number', type: 'text', placeholder: '12345678' },
			],
			[
				{ label: 'Telephone Number', type: 'text', placeholder: '+31 6 12345678' },
				{ label: 'Address', type: 'text', placeholder: 'Street 123' },
				{ label: 'Postcode', type: 'text', placeholder: '1234 AB' },
				{ label: 'Region', type: 'select', options: regions },
			],
		],
		veilingmeester: [
			[
				{ label: 'Email', type: 'email', placeholder: 'you@example.com' },
				{ label: 'Password', type: 'password', placeholder: '••••••••' },
			],
			[
				{ label: 'Region', type: 'select', options: regions },
				{ label: 'Authorisation Code', type: 'text', placeholder: '123456' },
			],
		],
	};

	const totalSteps = steps[userType].length;
	const currentFields = steps[userType][step - 1];

	const handleInputChange = (key: string, value: string) => {
		setFormData((prev) => ({ ...prev, [key]: value }));
	};

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		console.log('Final submitted data:', formData);
		alert('Account created successfully!');
	};

	return (
		<div className="app-page register-page">
			<div className="register-card">
				<button className="back-button" onClick={() => navigate('/')} aria-label="Go back to homepage">
					← Back
				</button>

				<div className="register-header">
					<h2 className="register-title">Create Account</h2>
					<p className="register-subtitle" aria-live="polite">
						Step {step} of {totalSteps}
					</p>
				</div>

				<div
					className="progress-bar"
					role="progressbar"
					aria-valuenow={step}
					aria-valuemin={1}
					aria-valuemax={totalSteps}
					aria-label={`Registration progress: step ${step} of ${totalSteps}`}
				>
					<div className="progress-bar-fill" style={{ width: `${(step / totalSteps) * 100}%` }}></div>
				</div>

				<div className="register-tabs" role="tablist" aria-label="Select user type">
					{(['koper', 'kweker', 'veilingmeester'] as UserType[]).map((type) => (
						<div
							key={type}
							className={`register-tab ${userType === type ? 'active' : ''}`}
							onClick={() => {
								setUserType(type);
								setStep(1);
							}}
							role="tab"
							aria-selected={userType === type}
							tabIndex={0}
							aria-label={`Register as ${type}`}
						>
							{type.charAt(0).toUpperCase() + type.slice(1)}
						</div>
					))}
				</div>

				<form className="register-form" onSubmit={handleSubmit}>
					{currentFields.map((field, idx) => {
						const key = `${userType}-${field.label}`;
						const ariaLabel = `${field.label} for ${userType}`;

						if (field.type === 'select') {
							return (
								<div key={idx}>
									<label htmlFor={key}>{field.label}</label>
									<select
										id={key}
										className="input-field"
										aria-label={ariaLabel}
										aria-required="true"
										value={formData[key] || ''}
										onChange={(e) => handleInputChange(key, e.target.value)}
									>
										<option value="" disabled>
											Select {field.label.toLowerCase()}
										</option>
										{field.options!.map((option) => (
											<option key={option} value={option}>
												{option}
											</option>
										))}
									</select>
								</div>
							);
						}

						return (
							<div key={idx}>
								<label htmlFor={key}>{field.label}</label>
								<input
									id={key}
									type={field.type}
									placeholder={field.placeholder}
									className="input-field"
									value={formData[key] || ''}
									onChange={(e) => handleInputChange(key, e.target.value)}
									aria-label={ariaLabel}
									aria-required="true"
								/>
							</div>
						);
					})}

					<div className="form-buttons">
						{step > 1 && (
							<button
								type="button"
								className="btn-secondary"
								onClick={(e) => {
									e.preventDefault();
									setStep(step - 1);
								}}
								aria-label="Go back to previous step"
							>
								Back
							</button>
						)}

						{step < totalSteps ? (
							<button
								type="button"
								className="btn-primary"
								onClick={(e) => {
									e.preventDefault();
									setStep(step + 1);
								}}
								aria-label="Go to next step"
							>
								Next
							</button>
						) : (
							<button type="submit" className="btn-primary" aria-label="Submit registration form and create account">
								Create Account
							</button>
						)}
					</div>

					<button type="button" className="login-link" onClick={() => navigate('/login')} aria-label="Go to login page instead">
						Login instead?
					</button>
				</form>
			</div>
		</div>
	);
}

export default Register;
