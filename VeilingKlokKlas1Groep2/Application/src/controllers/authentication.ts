import {fetchResponse} from '../utils/fetchHelpers';
import {LoginRequest} from '../declarations/LoginRequest';
import {AuthResponse} from '../declarations/AuthenticationResponse';
import {AccountInfo} from '../declarations/AccountInfo';

// Initialize authentication by chekcing if tokens are valid
export async function initializeAuthentication() {
	try {
		// Fetch account information
		const response = await fetchResponse<AccountInfo>('/api/auth/account');
		// Store account information in global application variable
		window.application.account = response;
		return response;
	} catch {
		localStorage.removeItem('accessToken');
		return undefined;
	}
}

// Login into account
export async function loginAccount(account: LoginRequest) {
	const response = await fetchResponse<AuthResponse>('/api/auth/login', {
		method: 'POST',
		body: JSON.stringify(account),
	});
	await saveAuthenticationResponse(response);
	return response;
}

// Logout from account
export async function logoutAccount() {
	await fetchResponse('/api/auth/logout').then(() => {
		localStorage.removeItem('accessToken');
	});
}

// Save the authentication in local storage
export async function saveAuthenticationResponse(response: AuthResponse) {
	localStorage.setItem('accessToken', response.accessToken);
}
