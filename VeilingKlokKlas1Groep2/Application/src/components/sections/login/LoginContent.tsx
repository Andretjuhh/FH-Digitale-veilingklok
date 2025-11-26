import React, { useCallback } from 'react';
import Button from '../../buttons/Button';
import '../../../styles/pages.css'; // make sure this path is correct!
import FormInputField from '../../elements/FormInputField';
import FormLink from '../../buttons/FormLink';
import { useForm, SubmitHandler } from 'react-hook-form';
import { useRootContext } from '../../../contexts/RootContext';

type LoginFormData = {
    email: string;
    password: string;
};

function LoginContent() {
	const { t, navigate } = useRootContext();

	/* * NOTE: The original component used the logo as a back button.
	 * I've preserved the back functionality and placed the logo
	 * inside the header for the new look, giving it a new class name.
	 */
	const handleGoBack = () => navigate('/');

	const { 
        register, 
        handleSubmit, 
        formState: { errors } 
    } = useForm<LoginFormData>();

	// 2. Define the submit handler function
    const onSubmit: SubmitHandler<LoginFormData> = (data) => {
        console.log('Form Submitted (Client-Side Validated):', data);
        
        // This is where you would call your backend API for authentication
        // For now, we simulate navigation on success
        navigate('/user-dashboard'); 
    };

	return (
		<div className="app-page login-page">
			<div className="login-card">
				<div className="login-header">
					<Button className="logo-icon" img="/svg/logo-flori.svg" onClick={handleGoBack} aria-label={t('back_button_aria')} />
					<div className="login-text-container">
						<h2 className={'login-title'} aria-label={t('welcome_back')}>
							{t('welcome_back')}
						</h2>
						<p className={'login-subtitle'} aria-label="Please sign in to your account">
							{t('sign_in_message')}
						</p>
					</div>
				</div>
				<form className="login-form" onSubmit={handleSubmit(onSubmit)}>
					<FormInputField id="email" label={t('email')} className="input-field" type="email" 
					// Pass RHF props: register('field-name', { rules })
                        {...register('email', { 
                            required: t('email_required_error'), 
                            pattern: {
                                value: /^\S+@\S+\.\S+$/, 
                                message: t('email_invalid_error') 
                            }
                        })}
                        // Check RHF errors object for this field
                        isError={!!errors.email} 
                        // Pass RHF error message to the component
                        error={errors.email?.message || 'Please type an email (example@email.com)'}/>
					<FormInputField id="password" label={t('password')} className="input-field" type="password" 
					// Pass RHF props: register('field-name', { rules })
                        {...register('password', { 
                            required: t('password_required_error'),
                        })}
                        // Check RHF errors object for this field
                        isError={!!errors.password} 
                        // Pass RHF error message to the component
                        error={errors.password?.message || 'Incorrect Password'} 
                    />

					<Button type="submit"  className="btn-primary" label={t('login')} aria-label={t('login_button_aria')} />

					{/* Still need to add forgotten link nav */}
					<FormLink className="forgot-link" label={t('forgot_password')} onClick={() => navigate('/')} aria-label={t('forgot_password_aria')} />
					<FormLink className="register-link" label={t('create_account')} onClick={() => navigate('/register')} aria-label={t('create_account_aria')} />
				</form>
			</div>
		</div>
	);
}

export default LoginContent;
