import React, { useCallback } from 'react';
import Button from '../../buttons/Button';
import '../../../styles/pages.css'; // make sure this path is correct!
import FormInputField from '../../elements/FormInputField';
import TextAreaInputField from '../../elements/TextAreaInputField';
import Login from '../../../pages/general/Login';
import FormLink from '../../buttons/FormLink';
import { useRootContext } from '../../../contexts/RootContext';

function LoginContent() {
	const { t, navigate } = useRootContext();

	/* * NOTE: The original component used the logo as a back button.
	 * I've preserved the back functionality and placed the logo
	 * inside the header for the new look, giving it a new class name.
	 */
	const handleGoBack = () => navigate('/');

	return (
		<div className="app-page login-page">
			<div className="login-card">
				<div className="login-header">
					<Button className="logo-icon" icon="/svg/logo-flori.svg" onClick={handleGoBack} aria-label={t('back_button_aria')} />
					<div className="login-text-container">
						<h2 className={'login-title'} aria-label={t('welcome_back')}>
							{t('welcome_back')}
						</h2>
						<p className={'login-subtitle'} aria-label="Please sign in to your account">
							{t('sign_in_message')}
						</p>
					</div>
				</div>
				<form className="login-form">
					<FormInputField id="email" label={t('email')} className="input-field" error="Please type an email (example@email.com)" isError={false} />
					<FormInputField id="password" label={t('password')} className="input-field" error="Incorrect Password" isError={false} />

					<Button className="btn-primary" label={t('login')} onClick={() => navigate('/user-dashboard')} aria-label={t('login_button_aria')} />

					{/* Still need to add forgotten link nav */}
					<FormLink className="forgot-link" label={t('forgot_password')} onClick={() => navigate('/')} aria-label={t('forgot_password_aria')} />
					<FormLink className="register-link" label={t('create_account')} onClick={() => navigate('/register')} aria-label={t('create_account_aria')} />
				</form>
			</div>
		</div>
	);
}

export default LoginContent;
