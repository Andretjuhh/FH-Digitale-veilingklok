import { fetchResponse } from '../utils/fetchHelpers';
import { NewKwekerAccount } from '../declarations/KwekerAccount';
import { AuthResponse } from '../declarations/AuthenticationResponse';
import { ProductDetails } from '../declarations/ProductDetails';

export async function createKwekerAccount(account: NewKwekerAccount) {
	return await fetchResponse<AuthResponse>('/api/kweker/create', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

export async function getKwekerAccountInfo() {
	return await fetchResponse('/api/kweker/account-info', {
		method: 'GET',
	});
}

export async function getKwekerProducts() {
	return await fetchResponse<{ products: ProductDetails[] }>('/api/kweker/products', {
		method: 'GET',
	});
}

// Optional stats endpoint: backend may or may not implement this. Frontend will call it and
// gracefully fall back to client-side derived stats if the call fails.
export async function getKwekerStats() {
	return await fetchResponse<{
		totalProducts: number;
		activeAuctions: number;
		totalRevenue: number;
		stemsSold: number;
	}>('/api/kweker/stats', {
		method: 'GET',
	});
}