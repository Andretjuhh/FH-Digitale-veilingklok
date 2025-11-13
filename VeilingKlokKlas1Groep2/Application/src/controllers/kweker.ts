import { fetchResponse } from '../utils/fetchHelpers';
import { NewKwekerAccount } from '../declarations/KwekerAccount';
import { AuthResponse } from '../declarations/AuthenticationResponse';

export async function createKwekerAccount(account: NewKwekerAccount) {
	return await fetchResponse<AuthResponse>('/kweker/create', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

export async function getKwekerAccountInfo() {
	return await fetchResponse('/kweker/account-info', {
		method: 'GET',
	});
}

export async function getKwekerProducts() {
	return await fetchResponse('/kweker/products', {
		method: 'GET',
	});
}
