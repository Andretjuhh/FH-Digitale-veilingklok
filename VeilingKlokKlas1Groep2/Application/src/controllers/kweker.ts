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

export async function createProduct(NewProduct: ProductDetails) {
	return await fetchResponse<{ message: string; product: ProductDetails }>(
		'/api/product/create',
		{
			method: 'POST',
			body: JSON.stringify(NewProduct),
		}
	);}