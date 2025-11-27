import React, {useCallback, useState} from 'react';
import Button from '../../buttons/Button';
import FormInputField from '../../elements/FormInputField';
import FormLink from '../../buttons/FormLink';
import {useRootContext} from '../../../contexts/RootContext';
import {AccountType} from '../../../types/AccountTypes';
import {RegisterSteps} from '../../../constant/forms';
import {useForm} from 'react-hook-form';
import {NewKwekerAccount} from '../../../declarations/KwekerAccount';
import {NewKoperAccount} from '../../../declarations/KoperAccount';
import {NewVeilingmeesterAccount} from '../../../declarations/VeilingmeesterAccount';
import {createKoperAccount} from '../../../controllers/koper';
import {createKwekerAccount} from '../../../controllers/kweker';
import {createVeilingmeesterAccount} from '../../../controllers/veilingmeester';
import {LayoutGroup, motion} from 'framer-motion';
import {Spring} from "../../../constant/animation";
import {useComponentStateReducer} from "../../../hooks/useComponentStateReducer";

function RegisterContent() {
	const {t, navigate, authenticateAccount} = useRootContext();
	const [state, updateState] = useComponentStateReducer({type: 'idle', message: ''});

	const {
		register,
		handleSubmit,
		trigger,
		formState: {errors},
	} = useForm();

	const [step, setStep] = useState(1);
	const [selectedAccountType, setAccountType] = useState<AccountType>(AccountType.Koper);

	const handleGoBack = () => navigate('/');

	const totalSteps = RegisterSteps[selectedAccountType].length;
	const currentFields = RegisterSteps[selectedAccountType][step - 1];

	// Validate current step fields before going next
	const validateStep = async () => {
		const fieldNames = currentFields.map((f) => f.label);
		const isValid = await trigger(fieldNames);
		return isValid;
	};

	// Form submission handler (only triggered on Create Account)
	const handleFormSubmittion = useCallback(
		async (data: any) => {
			try {
				let dashboardDestination = '/';

				switch (selectedAccountType) {
					case AccountType.Koper: {
						dashboardDestination = '/user-dashboard';
						const account: NewKoperAccount = {
							firstname: data['first_name'],
							lastname: data['last_name'],
							email: data['email'],
							password: data['password'],
							address: data['address'],
							postcode: data['postcode'],
							regio: data['region'],
							createdAt: new Date(),
						};
						const authResponse = await createKoperAccount(account);
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
						const authResponse = await createKwekerAccount(account);
						authenticateAccount(authResponse);
						break;
					}

					case AccountType.Veilingmeester: {
						dashboardDestination = '/dashboard';
						const account: NewVeilingmeesterAccount = {
							email: data['email'],
							password: data['password'],
							regio: data['region'],
							authorisatieCode: data['authorisation_code'],
							createdAt: new Date(),
						};
						const authResponse = await createVeilingmeesterAccount(account);
						authenticateAccount(authResponse);

						break;
					}
				}

				navigate(dashboardDestination, {replace: true});
			} catch (error) {
				console.error('Error during registration:', error);
			}
		},
		[navigate, selectedAccountType, authenticateAccount]
	);

	return (
		<LayoutGroup>
			<motion.div layout className="auth-card auto-width" transition={Spring}>
				{state.type === 'idle' && (
					<>
						<div className="auth-header">
							<img className="auth-header-logo" src="/svg/logo-flori.svg" alt={t('back_button_aria')} onClick={handleGoBack}/>
							<div className="auth-text-ctn">
								<h2 className="auth-header-h1" aria-label={t('create_account')}>
									{t('create_account')}
								</h2>
								<p className="auth-text-subtitle" aria-live="polite">
									{t('step')} {step} {t('of')} {totalSteps}
								</p>
							</div>

							{/* Stepper */}
							<div className="registration-stepper" aria-label={`Registration step ${step} of ${totalSteps}`}>
								{Array.from({length: totalSteps}, (_, index) => {
									const stepNumber = index + 1;
									const isActive = stepNumber === step;
									const isCompleted = stepNumber < step;

									return (
										<React.Fragment key={stepNumber}>
											<div className={`stepper-dot ${isActive ? 'active' : ''} ${isCompleted ? 'completed' : ''}`}>{isCompleted ?
												<span className="checkmark">✓</span> : stepNumber}</div>
											{stepNumber < totalSteps && <div className={`stepper-connector ${isCompleted ? 'completed' : ''}`}/>}
										</React.Fragment>
									);
								})}
							</div>

							{/* Tabs */}
							<div className="auth-tabs" role="tablist" aria-label="Select user type">
								{Object.values(AccountType).map((type) => (
									<div
										key={type}
										className={`auth-tab ${selectedAccountType === type ? 'active' : ''}`}
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
						</div>

						<form
							className="auth-form"
							onSubmit={handleSubmit(handleFormSubmittion)}
							autoComplete="off"
							onKeyDown={(e) => {
								// Prevent Enter from submitting form prematurely
								if (e.key === 'Enter' && step < totalSteps) e.preventDefault();
							}}
						>
							{/* Current step fields */}
							{currentFields.map((field) => {
								const name = field.label;
								const isRequired = field.required === true;
								const errorMsg = errors[name]?.message as string | undefined;

								return (
									<FormInputField
										key={`${selectedAccountType}-${step}-${name}`}
										id={name}
										label={isRequired ? `${t(field.label)} *` : t(field.label)}
										placeholder={field.placeholder}
										type={field.type}
										className="input-field"
										aria-label={t(field.label)}
										autoComplete={field.type === 'password' ? 'new-password' : 'off'}
										error={errorMsg}
										isError={!!errorMsg}
										{...register(name, {
											required: isRequired ? `${t(field.label)} is ${t('required')}` : false,
											...(name === 'email' && {
												pattern: {
													value: /^\S+@\S+\.\S+$/,
													message: t('email_invalid_error'),
												},
											}),
										})}
									/>
								);
							})}

							{/* Navigation Buttons */}
							<div className="auth-form-buttons">
								{step > 1 && <Button
									label={t('previous')}
									onClick={() => setStep(step - 1)} type="button"/>
								}

								{step < totalSteps ? (
									<Button
										label={t('next')}
										type="button"
										onClick={async () => {
											const valid = await validateStep();
											if (valid) setStep(step + 1);
										}}
									/>
								) : (
									<Button
										className="auth-submit-btn" label={t('create_account')}
										type="button" // ← change to button
										onClick={handleSubmit(handleFormSubmittion)} // ← manually trigger submit
									/>
								)}
							</div>

							<FormLink className="back-to-login-link" label={t('login_message')} onClick={() => navigate('/login')} type="button"/>
						</form>
					</>
				)}
			</motion.div>
		</LayoutGroup>
	);
}

export default RegisterContent;
