import { fetchResponse } from '../utils/fetchHelpers';
import { NewKoperAccount } from '../declarations/KoperAccount';
import { AuthResponse } from '../declarations/AuthenticationResponse';
import { ProductDetails } from '../declarations/ProductDetails';

export async function createKoperAccount(account: NewKoperAccount) {
    return await fetchResponse<AuthResponse>('/api/koper/create', {
        method: 'POST',
        body: JSON.stringify(account),
    });
}

export async function getKoperAccountInfo() {
    return await fetchResponse('/api/koper/account-info', {
        method: 'GET',
    });
}

export async function getKoperProducts() {
    return await fetchResponse<{ products: ProductDetails[] }>('/api/koper/products', {
        method: 'GET',
    });
}