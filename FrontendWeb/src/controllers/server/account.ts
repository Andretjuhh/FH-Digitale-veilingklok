import {fetchResponse} from '../../utils/fetchHelpers';
import {HttpSuccess} from '../../declarations/types/HttpSuccess';
import {RequestLoginDTO} from '../../declarations/dtos/input/RequestLoginDTO';
import {AuthOutputDto} from '../../declarations/dtos/output/AuthOutputDto';
import {AccountOutputDto} from '../../declarations/dtos/output/AccountOutputDto';
import {LocalStorageService} from '../services/localStorage';

// Get regions (GET /api/account/country/region)
export async function getRegions(): Promise<HttpSuccess<string[]>> {
	return fetchResponse<HttpSuccess<string[]>>('/api/account/country/region');
}

// Login (POST /api/account/login)
export async function loginAccount(loginRequest: RequestLoginDTO): Promise<HttpSuccess<AuthOutputDto>> {
	const response = await fetchResponse<HttpSuccess<AuthOutputDto>>('/api/account/login', {
		method: 'POST',
		body: JSON.stringify(loginRequest),
	});
	saveAuthenticationResponse(response.data ?? null);
	return response;
}

// Get account info (GET /api/account/info)
export async function getAccountInfo(): Promise<HttpSuccess<AccountOutputDto>> {
	return fetchResponse<HttpSuccess<AccountOutputDto>>('/api/account/info');
}

// Logout (GET /api/account/logout)
export async function logoutAccount(): Promise<HttpSuccess<string>> {
	const response = await fetchResponse<HttpSuccess<string>>('/api/account/logout');
	LocalStorageService.removeItem('accessToken');
	return response;
}

// Reauthenticate (GET /api/account/reauthenticate)
export async function reauthenticate(): Promise<HttpSuccess<AuthOutputDto>> {
	const response = await fetchResponse<HttpSuccess<AuthOutputDto>>('/api/account/reauthenticate');
	saveAuthenticationResponse(response.data ?? null);
	return response;
}

// Revoke devices (GET /api/account/revoke-devices)
export async function revokeDevices(): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>('/api/account/revoke-devices');
}

// Save the authentication token in local storage
function saveAuthenticationResponse(token: AuthOutputDto | null) {
	if (!token) return;
	LocalStorageService.setItem('accessToken', token);
}

// Get authentication token from local storage or reauthenticate if expired
export async function getAuthentication(): Promise<AuthOutputDto | null> {
	const token = LocalStorageService.getItem<AuthOutputDto>('accessToken');
	if (token?.accessTokenExpiresAt && new Date(token.accessTokenExpiresAt) > new Date()) return token;
	else {
		const reauth = await reauthenticate().catch(() => null);
		return reauth?.data ?? null;
	}
}
