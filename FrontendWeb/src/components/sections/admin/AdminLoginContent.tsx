import React from 'react';
import Button from '../../buttons/Button';
import '../../../styles/pages.css';
import FormInputField from '../../form-elements/FormInputField';
import {SubmitHandler, useForm} from 'react-hook-form';
import {useRootContext} from '../../contexts/RootContext';
import {loginAccount} from '../../../controllers/server/account';
import {useComponentStateReducer} from '../../../hooks/useComponentStateReducer';
import {LayoutGroup, motion} from 'framer-motion';
import {Spring} from '../../../constant/animation';
import Spinner from '../../elements/Spinner';
import {delay} from '../../../utils/standards';
import {isHttpError} from '../../../declarations/types/HttpError';
import {RequestLoginDTO} from '../../../declarations/dtos/input/RequestLoginDTO';
import {AccountType} from '../../../declarations/enums/AccountTypes';

function AdminLoginContent() {
	const {navigate, authenticateAccount} = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	const handleGoBack = () => navigate('/');

	const {
		register,
		handleSubmit,
		formState: {errors},
	} = useForm<RequestLoginDTO>();

	const onSubmit: SubmitHandler<RequestLoginDTO> = async (data) => {
		try {
			updateState({type: 'loading', message: 'Logging in...'});
			await delay(1000);
			const authResponse = await loginAccount({email: data.email, password: data.password});

			// Check if account type is Admin
			if (authResponse.data.accountType !== AccountType.Admin) {
				updateState({type: 'error', message: 'Access denied. Admin accounts only.'});
				await delay(2000);
				updateState({type: 'idle', message: ''});
				return;
			}

			updateState({type: 'succeed', message: 'Logged in successfully'});
			authenticateAccount(authResponse.data);
			await delay(1000);
			navigate('/admin/dashboard');
		} catch (error) {
			console.error('Login failed:', error);
			if (isHttpError(error)) {
				const message = error.message === 'CUSTOM.ACCOUNT_SOFT_DELETED'
					? 'This account has been deactivated. Please contact support.'
					: error.message;
				updateState({type: 'error', message});
			} else updateState({type: 'error', message: 'Something went wrong'});
			await delay(2000);
		} finally {
			updateState({type: 'idle', message: ''});
		}
	};

	return (
		<LayoutGroup>
			<motion.div layout className="auth-card" transition={Spring}>
				{state.type === 'idle' && (
					<>
						<div className="auth-header">
							<Button className="auth-header-back-button" icon="bi-x" onClick={handleGoBack} type="button" aria-label="Go back"/>
							<img className="auth-header-logo" src="/svg/logo-flori.svg" alt="Logo" onClick={handleGoBack}/>
							<div className="auth-text-ctn">
								<h2 className={'auth-header-h1'}>Admin Login</h2>
								<p className={'auth-text-subtitle'}>Sign in to admin dashboard</p>
							</div>
						</div>
						<form className="auth-form" onSubmit={handleSubmit(onSubmit)}>
							<FormInputField
								id="email"
								label="Email"
								className="input-field"
								type="email"
								icon="envelope-fill"
								{...register('email', {
									required: 'Email is required',
									pattern: {
										value: /^\S+@\S+\.\S+$/,
										message: 'Invalid email format',
									},
								})}
								isError={!!errors.email}
								error={errors.email?.message || 'Please type an email'}
							/>
							<FormInputField
								id="password"
								label="Password"
								className="input-field"
								type="password"
								icon="lock-fill"
								{...register('password', {
									required: 'Password is required',
								})}
								isError={!!errors.password}
								error={errors.password?.message || 'Incorrect Password'}
							/>

							<Button type="submit" className="auth-submit-btn" label="Login" aria-label="Login to admin dashboard"/>
						</form>
					</>
				)}

				{state.type !== 'idle' && (
					<div className="auth-state">
						{state.type === 'loading' && <Spinner/>}
						{state.type === 'succeed' && <i className="bi bi-check-circle-fill text-green-500"></i>}
						{state.type === 'error' && <i className="bi bi-x-circle-fill text-red-500"></i>}
						<p className="auth-state-text">{state.message}</p>
					</div>
				)}
			</motion.div>
		</LayoutGroup>
	);
}

export default AdminLoginContent;
