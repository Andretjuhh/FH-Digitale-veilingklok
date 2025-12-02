import React from 'react';
import Button from '../../buttons/Button';
import '../../../styles/pages.css'; // make sure this path is correct!
import FormInputField from '../../elements/FormInputField';
import FormLink from '../../buttons/FormLink';
import { SubmitHandler, useForm } from 'react-hook-form';
import { useRootContext } from '../../../contexts/RootContext';
import { loginAccount, saveAuthenticationResponse } from '../../../controllers/authentication';
import { useComponentStateReducer } from '../../../hooks/useComponentStateReducer';
import { LayoutGroup, motion } from 'framer-motion';
import { Spring } from '../../../constant/animation';
import Spinner from '../../elements/Spinner';
// import {delay} from '../../../utils/standards'

type LoginFormData = {
	email: string;
	password: string;
};

function LoginContent() {
	const { t, navigate } = useRootContext();
	const [state, updateState] = useComponentStateReducer({ type: 'idle', message: '' });

	/* * NOTE: The original component used the logo as a back button.
	 * I've preserved the back functionality and placed the logo
	 * inside the header for the new look, giving it a new class name.
	 */
	const handleGoBack = () => navigate('/');

	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm<LoginFormData>();

	// 2. Define the submit handler function
	const onSubmit: SubmitHandler<LoginFormData> = async (data) => {
		try {
			// updateState({type: 'loading', message: 'Logging in...'});
			// await delay(2000); // Simulate loading delay
			const authResponse = await loginAccount({ email: data.email, password: data.password });

			// Save auth session into context
			saveAuthenticationResponse(authResponse);

			// Determine dashboard by account type
			switch (authResponse.accountType) {
				case 'Koper':
					navigate('/user-dashboard');
					break;

				case 'Kweker':
					navigate('/kweker');
					break;

				case 'Veilingmeester':
					navigate('/dashboard');
					break;

				default:
					console.error('Unknown account type:', authResponse.accountType);
					navigate('/');
			}
		} catch (error) {
			console.error('Login failed:', error);

			// Show a user-friendly error
			alert(t('something_went_wrong') || 'Invalid email or password');
		} finally {
			// updateState({type: 'succeed', message: 'Logging in...'});
		}
	};

	return (
		<LayoutGroup>
			<motion.div layout className="auth-card" transition={Spring}>
				{state.type === 'idle' && (
					<>
						<div className="auth-header">
							<Button className="auth-header-back-button" icon="bi-x" onClick={handleGoBack} type="button" aria-label={t('back_button_aria')} />
							<img className="auth-header-logo" src="/svg/logo-flori.svg" alt={t('back_button_aria')} onClick={handleGoBack} />
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

							<Button type="submit" className="auth-submit-btn" label={t('login')} aria-label={t('login_button_aria')} />

							{/* Still need to add forgotten link nav */}
							<div className="flex flex-col">
								<FormLink className="auth-form-link" label={t('forgot_password')} onClick={() => navigate('/')} aria-label={t('forgot_password_aria')} />
								<FormLink className="auth-form-link" label={t('create_account')} onClick={() => navigate('/register')} aria-label={t('create_account_aria')} />
							</div>
						</form>
					</>
				)}

				{state.type === 'loading' && (
					<div className="form-state">
						<Spinner />
					</div>
				)}
			</motion.div>
		</LayoutGroup>
	);
}

export default LoginContent;
