import {fetchResponse} from '../../utils/fetchHelpers';
import {HttpSuccess} from '../../declarations/types/HttpSuccess';
import {CreateMeesterDTO} from '../../declarations/dtos/input/CreateMeesterDTO';
import {UpdateVeilingMeesterDTO} from '../../declarations/dtos/input/UpdateVeilingMeesterDTO';
import {CreateVeilingKlokDTO} from '../../declarations/dtos/input/CreateVeilingKlokDTO';
import {UpdateProductDTO} from '../../declarations/dtos/input/UpdateProductDTO';
import {AuthOutputDto} from '../../declarations/dtos/output/AuthOutputDto';
import {AccountOutputDto} from '../../declarations/dtos/output/AccountOutputDto';
import {OrderOutputDto} from '../../declarations/dtos/output/OrderOutputDto';
import {VeilingKlokDetailsOutputDto} from '../../declarations/dtos/output/VeilingKlokDetailsOutputDto';
import {VeilingKlokOutputDto} from '../../declarations/dtos/output/VeilingKlokOutputDto';
import {ProductDetailsOutputDto} from '../../declarations/dtos/output/ProductDetailsOutputDto';
import {PaginatedOutputDto} from '../../declarations/dtos/output/PaginatedOutputDto';
import {ProductOutputDto} from '../../declarations/dtos/output/ProductOutputDto';

export type CreateDevVeilingKlokProduct = {
	id: string;
	name: string;
	description: string;
	imageUrl?: string;
	dimension?: string | null;
	stock: number;
	companyName: string;
	maxPrice: number;
};

export type CreateDevVeilingKlokRequest = {
	scheduledAt: string;
	veilingDurationMinutes: number;
	products: CreateDevVeilingKlokProduct[];
};

// Create veilingmeester account (POST /api/account/meester/create)
export async function createVeilingmeesterAccount(account: CreateMeesterDTO): Promise<HttpSuccess<AuthOutputDto>> {
	return fetchResponse<HttpSuccess<AuthOutputDto>>('/api/account/meester/create', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

// Update veilingmeester account (PUT /api/account/meester/update)
export async function updateVeilingmeesterAccount(account: UpdateVeilingMeesterDTO): Promise<HttpSuccess<AccountOutputDto>> {
	return fetchResponse<HttpSuccess<AccountOutputDto>>('/api/account/meester/update', {
		method: 'PUT',
		body: JSON.stringify(account),
	});
}

// Update order product (PUT /api/account/meester/order/{orderId}/product/{productItemId}?quantity=)
export async function updateOrderProduct(orderId: string, productItemId: string, quantity: number): Promise<HttpSuccess<OrderOutputDto>> {
	return fetchResponse<HttpSuccess<OrderOutputDto>>(`/api/account/meester/order/${orderId}/product/${productItemId}?quantity=${quantity}`, {
		method: 'PUT',
	});
}

// Get order (GET /api/account/meester/order/{orderId})
export async function getOrder(orderId: string): Promise<HttpSuccess<OrderOutputDto>> {
	return fetchResponse<HttpSuccess<OrderOutputDto>>(`/api/account/meester/order/${orderId}`);
}

// Update order status (PUT /api/account/meester/order/{orderId}/status?status=)
export async function updateOrderStatus(orderId: string, status: string): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/order/${orderId}/status?status=${status}`, {
		method: 'PUT',
	});
}

// Create veilingklok (POST /api/account/meester/veilingklok)
export async function createVeilingKlok(veiling: CreateVeilingKlokDTO): Promise<HttpSuccess<VeilingKlokDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<VeilingKlokDetailsOutputDto>>('/api/account/meester/veilingklok', {
		method: 'POST',
		body: JSON.stringify(veiling),
	});
}

// Create dev veilingklok (POST /api/dev/veilingklok/dummy)
export async function createDevVeilingKlok(payload: CreateDevVeilingKlokRequest): Promise<HttpSuccess<VeilingKlokDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<VeilingKlokDetailsOutputDto>>('/api/dev/veilingklok/dummy', {
		method: 'POST',
		body: JSON.stringify(payload),
	});
}

// Get veilingklok (GET /api/account/meester/veilingklok/{klokId})
export async function getVeilingKlok(klokId: string): Promise<HttpSuccess<VeilingKlokOutputDto>> {
	return fetchResponse<HttpSuccess<VeilingKlokOutputDto>>(`/api/account/meester/veilingklok/${klokId}`);
}

// Get product details (GET /api/account/meester/product/{productId}/details)
export async function getProductDetails(productId: string): Promise<HttpSuccess<ProductDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<ProductDetailsOutputDto>>(`/api/account/meester/product/${productId}/details`);
}

// Update product price (PUT /api/account/meester/product/{productId}/price?kwekerId=)
export async function updateProductPrice(productId: string, kwekerId: string, product: UpdateProductDTO): Promise<HttpSuccess<ProductDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<ProductDetailsOutputDto>>(`/api/account/meester/product/${productId}/price?kwekerId=${kwekerId}`, {
		method: 'PUT',
		body: JSON.stringify(product),
	});
}

// Get products (GET /api/account/meester/products)
export async function getProducts(nameFilter?: string, regionFilter?: string, maxPrice?: number, kwekerId?: string, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>> {
	const params = new URLSearchParams();
	if (nameFilter) params.append('nameFilter', nameFilter);
	if (maxPrice) params.append('maxPrice', maxPrice.toString());
	if (kwekerId) params.append('kwekerId', kwekerId);
	if (regionFilter) params.append('regionFilter', regionFilter);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>>(`/api/account/meester/products?${params.toString()}`);
}

// Start veiling product (POST /api/account/meester/veilingklok/{klokId}/start/{productId})
export async function startVeilingProduct(klokId: string, productId: string): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/veilingklok/${klokId}/start/${productId}`, {
		method: 'POST',
	});
}

// Update veilingklok status (PUT /api/account/meester/veilingklok/{klokId}/status?status=)
export async function updateVeilingKlokStatus(klokId: string, status: string): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/veilingklok/${klokId}/status?status=${status}`, {
		method: 'PUT',
	});
}
