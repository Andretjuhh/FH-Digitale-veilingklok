import React, { useCallback, useMemo, useState } from 'react';
import Button from '../../buttons/Button';
import FormInputField from '../../elements/FormInputField';
import FormLink from '../../buttons/FormLink';
import { useRootContext } from '../../contexts/RootContext';
import { AccountType } from '../../../declarations/enums/AccountTypes';
import { buildFieldLayout, RegisterSteps } from '../../../constant/forms';
import { useForm } from 'react-hook-form';
import { InputField } from '../../../declarations/types/FormField';
import { CreateKoperDTO } from '../../../declarations/dtos/input/CreateKoperDTO';
import { CreateKwekerDTO } from '../../../declarations/dtos/input/CreateKwekerDTO';
import { CreateMeesterDTO } from '../../../declarations/dtos/input/CreateMeesterDTO';
import { createKoperAccount } from '../../../controllers/server/koper';
import { createKwekerAccount } from '../../../controllers/server/kweker';
import { createVeilingmeesterAccount } from '../../../controllers/server/veilingmeester';
import { LayoutGroup, motion } from 'framer-motion';
import { Spring } from '../../../constant/animation';
import { useComponentStateReducer } from '../../../hooks/useComponentStateReducer';
import Spinner from '../../elements/Spinner';
import { delay } from '../../../utils/standards';
import { isHttpError } from '../../../declarations/types/HttpError';

function RegisterContent() {
	const { t, navigate, authenticateAccount } = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	const {
		register,
		handleSubmit,
		trigger,
		formState: { errors },
	} = useForm();

	const [step, setStep] = useState(1);
	const [selectedAccountType, setAccountType] = useState<AccountType>(AccountType.Koper);

	const totalSteps = RegisterSteps[selectedAccountType].length;
	const currentFields = RegisterSteps[selectedAccountType][step - 1];

	const orderedFields = useMemo(() => buildFieldLayout(currentFields), [currentFields]);
	const handleGoBack = () => navigate('/');
	const renderField = useCallback(
		(field: InputField, key: string) => {
			const name = field.label;
			const isRequired = field.required;
			const errorMsg = errors[name]?.message as string | undefined;
			return (
				<FormInputField
					key={key}
					id={name}
					icon={field.icon}
					label={isRequired ? `${t(field.label)} *` : t(field.label)}
					placeholder={field.placeholder || ''}
					type={field.type === 'select' ? 'text' : field.type}
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
		},
		[errors, register, selectedAccountType, step, t]
	);

	// Validate current step fields before going next
	const validateStep = async () => {
		const fieldNames = currentFields.map((f) => f.label);
		return await trigger(fieldNames);
	};

	// Form submission handler (only triggered on Create Account)
	const handleFormSubmittion = useCallback(
		async (data: any) => {
			try {
				updateState({ type: 'loading', message: t('creating_account') });
				await delay(1000); // Simulate loading delay
				let dashboardDestination = '/';
				switch (selectedAccountType) {
					case AccountType.Koper: {
						dashboardDestination = '/koper/dashboard';
						const account: CreateKoperDTO = {
							firstName: data['first_name'],
							lastName: data['last_name'],
							email: data['email'],
							password: data['password'],
							telephone: data['phonenumber'],
							address: {
								street: data['address'],
								city: data['region'],
								regionOrState: data['region'],
								postalCode: data['postcode'],
								country: data['country'] === 'Nederland' ? 'NL' : data['country'],
							},
						};
						const authResponse = await createKoperAccount(account);
						authenticateAccount(authResponse.data);
						break;
					}

					case AccountType.Kweker: {
						dashboardDestination = '/kweker/dashboard';
						const account: CreateKwekerDTO = {
							companyName: data['company_name'],
							firstName: data['first_name'],
							lastName: data['last_name'],
							email: data['email'],
							password: data['password'],
							telephone: data['phonenumber'],
							kvkNumber: data['kvk_number'],
							address: {
								street: data['address'],
								city: data['region'],
								regionOrState: data['region'],
								postalCode: data['postcode'],
								country: data['country'] === 'Nederland' ? 'NL' : data['country'],
							},
						};
						const authResponse = await createKwekerAccount(account);
						authenticateAccount(authResponse.data);
						break;
					}

					case AccountType.Veilingmeester: {
						dashboardDestination = '/veilingmeester/dashboard';
						const account: CreateMeesterDTO = {
							email: data['email'],
							password: data['password'],
							region: data['region'],
							countryCode: data['country'] === 'Nederland' ? 'NL' : data['country'],
							authorisatieCode: data['authorisation_code'],
						};
						const authResponse = await createVeilingmeesterAccount(account);
						authenticateAccount(authResponse.data);

						break;
					}
				}

				updateState({ type: 'succeed', message: t('account_created') });
				await delay(1000); // Simulate loading delay
				navigate(dashboardDestination, { replace: true });
			} catch (error) {
				console.error('Login failed:', error);
				if (isHttpError(error)) updateState({ type: 'error', message: error.message });
				else updateState({ type: 'error', message: t('something_went_wrong') });
				await delay(2000); // Simulate loading delay
			} finally {
				updateState({ type: 'idle', message: '' });
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
							<Button className="auth-header-back-button" icon="bi-x" onClick={handleGoBack} type="button" aria-label={t('back_button_aria')} />
							<img className="auth-header-logo" src="/svg/logo-flori.svg" alt={t('back_button_aria')} onClick={handleGoBack} />
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
								{Array.from({ length: totalSteps }, (_, index) => {
									const stepNumber = index + 1;
									const isActive = stepNumber === step;
									const isCompleted = stepNumber < step;

									return (
										<React.Fragment key={stepNumber}>
											<div className={`stepper-dot ${isActive ? 'active' : ''} ${isCompleted ? 'completed' : ''}`}>{isCompleted ? <span className="checkmark">✓</span> : stepNumber}</div>
											{stepNumber < totalSteps && <div className={`stepper-connector ${isCompleted ? 'completed' : ''}`} />}
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
							onKeyDown={(e) => {
								if (e.key === 'Enter' && step < totalSteps) e.preventDefault();
							}}
						>
							{/* Current step fields */}
							{orderedFields.map((item) =>
								item.type === 'group' ? (
									<div key={`group-${item.groupName}`} className="form-field-group">
										{item.fields.map((field, fieldIndex) => renderField(field, `${item.groupName}-${field.label}-${fieldIndex}`))}
									</div>
								) : (
									renderField(item.field, `field-${item.field.label}`)
								)
							)}

							{/* Navigation Buttons */}
							<div className="auth-move-btns">
								{step > 1 && <Button label={t('previous')} className="auth-form-move-button prev-btn" icon="bi-arrow-left" onClick={() => setStep(step - 1)} type="button" />}
								{step < totalSteps && (
									<Button
										label={t('next')}
										className="auth-form-move-button"
										icon="bi-arrow-right"
										type="button"
										onClick={async () => {
											const valid = await validateStep();
											if (valid) setStep(step + 1);
										}}
									/>
								)}
							</div>

							{step == totalSteps && <Button className="auth-submit-btn" label={t('create_account')} type="button" onClick={handleSubmit(handleFormSubmittion)} />}
							{step <= 1 && <FormLink className="back-to-login-link" label={t('login_message')} onClick={() => navigate('/login')} type="button" />}
						</form>
					</>
				)}

				{state.type !== 'idle' && (
					<div className="auth-state">
						{state.type === 'loading' && <Spinner />}
						{state.type === 'succeed' && <i className="bi bi-check-circle-fill text-green-500"></i>}
						{state.type === 'error' && <i className="bi bi-x-circle-fill text-red-500"></i>}
						<p className="auth-state-text">{state.message}</p>
					</div>
				)}
			</motion.div>
		</LayoutGroup>
	);
}

export default RegisterContent;
