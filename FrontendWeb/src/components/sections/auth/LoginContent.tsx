import React from 'react';
import Button from '../../buttons/Button';
import FormInputField from '../../form-elements/FormInputField';
import FormLink from '../../buttons/FormLink';
import { SubmitHandler, useForm } from 'react-hook-form';
import { useRootContext } from '../../contexts/RootContext';
import { loginAccount } from '../../../controllers/server/account';
import { useComponentStateReducer } from '../../../hooks/useComponentStateReducer';
import { LayoutGroup, motion } from 'framer-motion';
import { Spring } from '../../../constant/animation';
import { delay } from '../../../utils/standards';
import { isHttpError } from '../../../declarations/types/HttpError';
import { RequestLoginDTO } from '../../../declarations/dtos/input/RequestLoginDTO';
import { AccountType } from '../../../declarations/enums/AccountTypes';
import ComponentState from '../../elements/ComponentState';

function LoginContent() {
	const { t, navigate, authenticateAccount } = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	/* * NOTE: The original component used the logo as a back button.
	 * I've preserved the back functionality and placed the logo
	 * inside the header for the new look, giving it a new class name.
	 */
	const handleGoBack = () => navigate('/');

	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm<RequestLoginDTO>();

	// 2. Define the submit handler function
	const onSubmit: SubmitHandler<RequestLoginDTO> = async (data) => {
		try {
			updateState({ type: 'loading', message: t('logging_in') });
			await delay(1000); // Simulate loading delay
			const authResponse = await loginAccount({ email: data.email, password: data.password });
			updateState({ type: 'succeed', message: t('logged_in') });
			authenticateAccount(authResponse.data);
			await delay(1000); // Simulate loading delay

			console.log(authResponse.data);
			// Determine dashboard by account type
			switch (authResponse.data.accountType) {
				case AccountType.Koper:
					navigate('/koper/veilingen');
					break;

				case AccountType.Kweker:
					navigate('/kweker/dashboard');
					break;

				case AccountType.Veilingmeester:
					navigate('/veilingmeester/veilingen-beheren');
					break;

				case AccountType.Admin:
					navigate('/admin/dashboard');
					break;

				default:
					console.error('Unknown account type:', authResponse.data.accountType);
					navigate('/');
			}
		} catch (error) {
			console.error('Login failed:', error);
			if (isHttpError(error)) updateState({ type: 'error', message: error.message });
			else updateState({ type: 'error', message: t('something_went_wrong') });
			await delay(2000); // Simulate loading delay
		} finally {
			updateState({ type: 'idle', message: '' });
		}
	};

	return (
		<LayoutGroup>
			<motion.div layout className="modal-card auth-card" transition={Spring}>
				{state.type === 'idle' && (
					<>
						<div className="auth-header">
							<Button className="auth-header-back-button" icon="bi-x" onClick={handleGoBack} type="button" aria-label={t('aria_back_button')} />
							<img className="auth-header-logo" src="/svg/logo-flori.svg" alt={t('aria_back_button')} onClick={handleGoBack} />
							<div className="auth-text-ctn">
								<h2 className={'auth-header-h1'} aria-label={t('login_title')}>
									{t('login_title')}
								</h2>
								<p className={'auth-text-subtitle'} aria-label="Please sign in to your account">
									{t('sign_in_message')}
								</p>
							</div>
						</div>
						<form className="auth-form" onSubmit={handleSubmit(onSubmit)}>
							<FormInputField
								id="email"
								label={t('email')}
								className="input-field"
								type="email"
								icon="envelope-fill"
								// Pass RHF props: register('field-name', { rules })
								{...register('email', {
									required: t('email_required_error'),
									pattern: {
										value: /^\S+@\S+\.\S+$/,
										message: t('email_invalid_error'),
									},
								})}
								// Check RHF errors object for this field
								isError={!!errors.email}
								// Pass RHF error message to the component
								error={errors.email?.message || 'Please type an email (example@email.com)'}
							/>
							<FormInputField
								id="password"
								label={t('password')}
								className="input-field"
								type="password"
								icon="lock-fill"
								// Pass RHF props: register('field-name', { rules })
								{...register('password', {
									required: t('password_required_error'),
								})}
								// Check RHF errors object for this field
								isError={!!errors.password}
								// Pass RHF error message to the component
								error={errors.password?.message || 'Incorrect Password'}
							/>

							<Button type="submit" className="auth-submit-btn" label={t('login')} aria-label={t('aria_login_button')} />

							{/* Still need to add forgotten link nav */}
							<div className="flex flex-col">
								<FormLink className="auth-form-link" label={t('forgot_password')} onClick={() => navigate('/')} aria-label={t('aria_forgot_password')} />
								<FormLink className="auth-form-link" label={t('create_account')} onClick={() => navigate('/register')} aria-label={t('aria_create_account')} />
							</div>
						</form>
					</>
				)}

				{state.type !== 'idle' && <ComponentState state={state} />}
			</motion.div>
		</LayoutGroup>
	);
}

export default LoginContent;
