import React, {useState} from 'react';
import {useNavigate} from 'react-router-dom';
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

	// ✅ All form data stored here (controlled inputs)
	const [formData, setFormData] = useState<Record<string, string>>({});

	const regions = ['Noord-Holland', 'Zuid-Holland', 'Utrecht', 'Gelderland', 'Overijssel', 'Limburg', 'Friesland', 'Drenthe', 'Flevoland', 'Groningen', 'Zeeland'];

	// Define steps for each user type
	const steps: Record<UserType, InputField[][]> = {
		koper: [
			[
				{label: 'Email', type: 'email', placeholder: 'you@example.com'},
				{label: 'Password', type: 'password', placeholder: '••••••••'},
				{label: 'First Name', type: 'text', placeholder: 'Steve'},
				{label: 'Last Name', type: 'text', placeholder: 'Jobs'},
			],
			[
				{label: 'Address', type: 'text', placeholder: 'Street 123'},
				{label: 'Postcode', type: 'text', placeholder: '1234 AB'},
				{label: 'Region', type: 'select', options: regions},
			],
		],
		kweker: [
			[
				{label: 'Company Name', type: 'text', placeholder: 'Example BV'},
				{label: 'Email', type: 'email', placeholder: 'you@example.com'},
				{label: 'Password', type: 'password', placeholder: '••••••••'},
				{label: 'kvk Number', type: 'text', placeholder: '12345678'},
			],
			[
				{label: 'Telephone Number', type: 'text', placeholder: '+31 6 12345678'},
				{label: 'Address', type: 'text', placeholder: 'Street 123'},
				{label: 'Postcode', type: 'text', placeholder: '1234 AB'},
				{label: 'Region', type: 'select', options: regions},
			],
		],
		veilingmeester: [
			[
				{label: 'Email', type: 'email', placeholder: 'you@example.com'},
				{label: 'Password', type: 'password', placeholder: '••••••••'},
			],
			[
				{label: 'Region', type: 'select', options: regions},
				{label: 'Authorisation Code', type: 'text', placeholder: '123456'},
			],
		],
	};

	const totalSteps = steps[userType].length;
	const currentFields = steps[userType][step - 1];

	const handleInputChange = (key: string, value: string) => {
		setFormData((prev) => ({...prev, [key]: value}));
	};

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		console.log('Final submitted data:', formData);
		alert('Account created successfully!');
	};


	return (
		<div className="app-page register-page">
			<div className="register-card">
				{/* Back Button */}
				<button className="back-button" onClick={() => navigate('/')}>
					← Back
				</button>
				<div className="register-header">
					<h2 className="register-title">Create Account</h2>
					<p className="register-subtitle">
						Step {step} of {totalSteps}
					</p>
				</div>

				{/* Progress Bar */}
				<div className="progress-bar">
					<div className="progress-bar-fill" style={{width: `${(step / totalSteps) * 100}%`}}></div>
				</div>

				{/* Tabs */}
				<div className="register-tabs">
					{(['koper', 'kweker', 'veilingmeester'] as UserType[]).map((type) => (
						<div
							key={type}
							className={`register-tab ${userType === type ? 'active' : ''}`}
							onClick={() => {
								setUserType(type);
								setStep(1); // reset to first step when switching user type
							}}
						>
							{type.charAt(0).toUpperCase() + type.slice(1)}
						</div>
					))}
				</div>

				{/* Form */}
				<form className="register-form" onSubmit={handleSubmit}>
					{currentFields.map((field, idx) => {
						const key = `${userType}-${field.label}`;

						if (field.type === 'select') {
							return (
								<div key={idx}>
									<label>{field.label}</label>
									<select className="input-field" value={formData[key] || ''} onChange={(e) => handleInputChange(key, e.target.value)}>
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
								<label>{field.label}</label>
								<input
									type={field.type}
									placeholder={field.placeholder}
									className="input-field"
									value={formData[key] || ''}
									onChange={(e) => handleInputChange(key, e.target.value)}
								/>
							</div>
						);
					})}

					{/* Step Buttons */}
					<div className="form-buttons">
						{step > 1 && (
							<button
								type="button"
								className="btn-secondary"
								onClick={(e) => {
									e.preventDefault();
									setStep(step - 1);
								}}
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
							>
								Next
							</button>
						) : (
							<button type="submit" className="btn-primary">
								Create Account
							</button>
						)}
					</div>

					<button type="button" className="login-link" onClick={() => navigate('/login')}>
						Login instead?
					</button>
				</form>
			</div>
		</div>
	);
}

export default Register;
