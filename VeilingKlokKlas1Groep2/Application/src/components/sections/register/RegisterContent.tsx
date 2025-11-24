import React, { useCallback, useState } from 'react';
import Button from '../../buttons/Button';
import FormInputField from '../../elements/FormInputField';
import FormLink from '../../buttons/FormLink';
import { useRootContext } from '../../../contexts/RootContext';
import { AccountType } from '../../../types/AccountTypes';
import { RegisterSteps } from '../../../constant/forms';
import { useForm } from 'react-hook-form';
import { NewKwekerAccount } from '../../../declarations/KwekerAccount';
import { NewKoperAccount } from '../../../declarations/KoperAccount';
import { createKoperAccount} from '../../../controllers/koper';
import { createKwekerAccount } from '../../../controllers/kweker';

function RegisterContent() {
	const { t, navigate, authenticateAccount } = useRootContext();

	const { register, handleSubmit } = useForm();

	// State management
	const [step, setStep] = useState(1);
	const [selectedAccountType, setAccountType] = useState<AccountType>(AccountType.Koper);
	const handleGoBack = () => navigate('/');

	// Define the steps and their corresponding fields
	const totalSteps = RegisterSteps[selectedAccountType].length;
	const currentFields = RegisterSteps[selectedAccountType][step - 1];

	// Handle form submission
	// console.log('Form Errors:', formState.errors);
	// console.log('Form Values:', getValues());

	const handleFormSubmittion = useCallback(
		async (data: any) => {
			try {
				console.log('Final submitted data:', data);

				// Redirect based on account type
				let dashboardDestination = '/';

				// Implement authentication logic here (e.g., API call to register the user)
				switch (selectedAccountType) {
					case AccountType.Koper: {
						dashboardDestination = '/user-dashboard';
						const account: NewKoperAccount = {
							name: data['company_name'],
							email: data['email'],
							password: data['password'],
							telephone: data['phonenumber'],
							address: data['address'],
							postcode: data['postcode'],
							regio: data['region'],
							kvkNumber: data['kvk_number'],
							createdAt: new Date(),
						}

						// Call the API to create the kweker account
						const authResponse = await createKoperAccount(account);

						//Authenticate the account in the app
						authenticateAccount(authResponse);
						break;
					}
					case AccountType.Kweker: {
						dashboardDestination = '/kweker';
						const account: NewKwekerAccount = {
							name: data['company_name'],
							email: data['email'],
							password: data['password'],
							telephone: data['phonenumber'],
							address: data['address'],
							postcode: data['postcode'],
							regio: data['region'],
							kvkNumber: data['kvk_number'],
							createdAt: new Date(),
						};

						// Call the API to create the kweker account
						const authResponse = await createKwekerAccount(account);

						// Authenticate the account in the app
						authenticateAccount(authResponse);
						break;
					}
					case AccountType.Veilingmeester:
						dashboardDestination = '/dashboard';
						break;
				}

				// Navigate to the appropriate dashboard
				navigate(dashboardDestination, { replace: true });
			} catch (error) {
				console.error('Error during registration:', error);
				// Handle errors (e.g., show error message to the user)
			}
		},
		[navigate, selectedAccountType]
	);

	return (
		<div className="app-page register-page">
			<div className="register-card">
				<div className="register-header">
					<Button className="logo-icon" img="/svg/logo-flori.svg" onClick={handleGoBack} aria-label={t('back_button_aria')} />
					<div className="register-text-container">
						<h2 className="register-title" aria-label={t('create_account')}>
							{t('create_account')}
						</h2>
						<p className="register-subtitle" aria-live="polite">
							{t('step')} {step} {t('of')} {totalSteps}
						</p>
					</div>
				</div>

				<div className="registration-stepper" aria-label={`Registration step ${step} of ${totalSteps}`}>
					{/* Map over the total number of steps (1 and 2) */}
					{Array.from({ length: totalSteps }, (_, index) => {
						const stepNumber = index + 1;
						const isActive = stepNumber === step;
						const isCompleted = stepNumber < step;

						return (
							<React.Fragment key={stepNumber}>
								<div className={`stepper-dot ${isActive ? 'active' : ''} ${isCompleted ? 'completed' : ''}`} role="presentation">
									{/* Optional: Add a checkmark for completed steps */}
									{isCompleted ? <span className="checkmark">✓</span> : stepNumber}
								</div>

								{/* Connector line between dots */}
								{stepNumber < totalSteps && <div className={`stepper-connector ${isCompleted ? 'completed' : ''}`} />}
							</React.Fragment>
						);
					})}
				</div>

				<div className="register-tabs" role="tablist" aria-label="Select user type">
					{Object.values(AccountType).map((type, index) => (
						<div
							key={type}
							className={`register-tab ${selectedAccountType === type ? 'active' : ''}`}
							onClick={() => {
								setAccountType(type);
								setStep(1);
							}}
							role="tab"
							aria-selected={selectedAccountType === type}
							tabIndex={0}
							aria-label={`Register as ${type}`}
						>
							{type.charAt(0).toUpperCase() + type.slice(1)}
						</div>
					))}
				</div>

				<form className="register-form" onSubmit={handleSubmit(handleFormSubmittion)}>
					{
						// Map over the fields for the current step
						currentFields.map((field, idx) => {
							const ariaLabel = `${t(field.label)} ${t('for')} ${t(selectedAccountType)}`;

							return (
								<FormInputField
									key={idx}
									id={field.label}
									label={t(field.label)}
									placeholder={field.placeholder}
									type={field.type}
									className="input-field"
									aria-label={ariaLabel}
									{...register(field.label, { required: true })}
								/>
							);
						})
					}

					<div className="form-buttons">
						{step > 1 && <Button className="btn-secondary" label={t('previous')} onClick={() => setStep(step - 1)} aria-label={t('previous')} type="button" />}

						{step < totalSteps ? (
							<Button className="btn-secondary" label={t('next')} onClick={() => setStep(step + 1)} aria-label={t('next')} type="button" />
						) : (
							<Button className="btn-primary" label={t('create_account')} aria-label={t('create_account')} type="submit" />
						)}
					</div>
					<FormLink className="back-to-login-link" label={t('login_message')} onClick={() => navigate('/login')} aria-label={t('login_message_aria')} type="button" />
				</form>
			</div>
		</div>
	);
}

export default RegisterContent;
