import { fetchResponse } from '../utils/fetchHelpers';
import { LoginRequest } from '../declarations/LoginRequest';

export async function loginAccount(account: LoginRequest) {
	return await fetchResponse('/api/auth/login', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}
