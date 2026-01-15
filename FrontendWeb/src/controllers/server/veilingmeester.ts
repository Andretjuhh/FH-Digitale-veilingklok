import { fetchResponse } from '../../utils/fetchHelpers';
import { HttpSuccess } from '../../declarations/types/HttpSuccess';
import { CreateMeesterDTO } from '../../declarations/dtos/input/CreateMeesterDTO';
import { UpdateVeilingMeesterDTO } from '../../declarations/dtos/input/UpdateVeilingMeesterDTO';
import { CreateVeilingKlokDTO } from '../../declarations/dtos/input/CreateVeilingKlokDTO';
import { AuthOutputDto } from '../../declarations/dtos/output/AuthOutputDto';
import { AccountOutputDto } from '../../declarations/dtos/output/AccountOutputDto';
import { OrderOutputDto } from '../../declarations/dtos/output/OrderOutputDto';
import { VeilingKlokDetailsOutputDto } from '../../declarations/dtos/output/VeilingKlokDetailsOutputDto';
import { VeilingKlokOutputDto } from '../../declarations/dtos/output/VeilingKlokOutputDto';
import { ProductDetailsOutputDto } from '../../declarations/dtos/output/ProductDetailsOutputDto';
import { PaginatedOutputDto } from '../../declarations/dtos/output/PaginatedOutputDto';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { VeilingKlokStatus } from '../../declarations/enums/VeilingKlokStatus';

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
	veilingDurationSeconds: number;
	products: CreateDevVeilingKlokProduct[];
};

// Create veilingmeester account (POST /api/account/meester/create)
export async function createVeilingmeesterAccount(account: CreateMeesterDTO): Promise<HttpSuccess<AuthOutputDto>> {
	return fetchResponse<HttpSuccess<AuthOutputDto>>('/api/account/meester/create?useCookies=true', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

// Update veilingmeester account (PUT /api/account/meester/update)
export async function updateVeilingmeesterAccount(account: UpdateVeilingMeesterDTO): Promise<HttpSuccess<AccountOutputDto>> {
	return fetchResponse<HttpSuccess<AccountOutputDto>>('/api/account/meester/update', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

// Update order product (PUT /api/account/meester/order/{orderId}/product/{productItemId}?quantity=)
export async function updateOrderProduct(orderId: string, productItemId: string, quantity: number): Promise<HttpSuccess<OrderOutputDto>> {
	return fetchResponse<HttpSuccess<OrderOutputDto>>(`/api/account/meester/order/${orderId}/product/${productItemId}?quantity=${quantity}`, {
		method: 'POST',
	});
}

// Get order (GET /api/account/meester/order/{orderId})
export async function getOrder(orderId: string): Promise<HttpSuccess<OrderOutputDto>> {
	return fetchResponse<HttpSuccess<OrderOutputDto>>(`/api/account/meester/order/${orderId}`);
}

// Update order status (PUT /api/account/meester/order/{orderId}/status?status=)
export async function updateOrderStatus(orderId: string, status: string): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/order/${orderId}/status?status=${status}`, {
		method: 'POST',
	});
}

// Create veilingklok (POST /api/account/meester/veilingklok)
export async function createVeilingKlok(veiling: CreateVeilingKlokDTO): Promise<HttpSuccess<VeilingKlokDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<VeilingKlokDetailsOutputDto>>('/api/account/meester/veilingklok', {
		method: 'POST',
		body: JSON.stringify(veiling),
	});
}

// Get veilingklok (GET /api/account/meester/veilingklok/{klokId})
export async function getVeilingKlok(klokId: string): Promise<HttpSuccess<VeilingKlokOutputDto>> {
	return fetchResponse<HttpSuccess<VeilingKlokOutputDto>>(`/api/account/meester/veilingklok/${klokId}`);
}

// Get veilingklok products (GET /api/account/meester/veilingklok/{klokId}/products)
export async function getVeilingKlokProducts(klokId: string): Promise<HttpSuccess<ProductOutputDto[]>> {
	return fetchResponse<HttpSuccess<ProductOutputDto[]>>(`/api/account/meester/veilingklok/${klokId}/products`);
}

// Get veilingklok orders (GET /api/account/meester/veilingklok/{klokId}/orders)
export async function getVeilingKlokOrders(klokId: string, status?: string, beforeDate?: string, afterDate?: string): Promise<HttpSuccess<OrderOutputDto[]>> {
	const params = new URLSearchParams();
	if (status) params.append('status', status);
	if (beforeDate) params.append('beforeDate', beforeDate);
	if (afterDate) params.append('afterDate', afterDate);

	return fetchResponse<HttpSuccess<OrderOutputDto[]>>(`/api/account/meester/veilingklok/${klokId}/orders?${params.toString()}`);
}

// Get product details (GET /api/account/meester/product/{productId}/details)
export async function getProductDetails(productId: string): Promise<HttpSuccess<ProductDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<ProductDetailsOutputDto>>(`/api/account/meester/product/${productId}/details`);
}

// Update product price (PUT /api/account/meester/product/{productId}/price?)
export async function updateProductPrice(productId: string, price: number): Promise<HttpSuccess<ProductDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<ProductDetailsOutputDto>>(`/api/account/meester/product/${productId}/price?price=${price}`, {
		method: 'POST',
	});
}

// Get products (GET /api/account/meester/products)
export async function getProducts(nameFilter?: string, regionFilter?: string, maxPrice?: number, kwekerId?: string, klokId?: string, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>> {
	const params = new URLSearchParams();
	if (nameFilter) params.append('nameFilter', nameFilter);
	if (maxPrice) params.append('maxPrice', maxPrice.toString());
	if (kwekerId) params.append('kwekerId', kwekerId);
	if (klokId) params.append('klokId', klokId);
	if (regionFilter) params.append('regionFilter', regionFilter);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>>(`/api/account/meester/products?${params.toString()}`);
}

// Get veilingklokken (GET /api/account/meester/veilingklokken)
export async function getVeilingKlokken(statusFilter?: VeilingKlokStatus, region?: string, scheduledAfter?: string, scheduledBefore?: string, startedAfter?: string, startedBefore?: string, endedAfter?: string, endedBefore?: string, meesterId?: string, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<VeilingKlokOutputDto>>> {
	const params = new URLSearchParams();
	if (statusFilter) params.append('statusFilter', statusFilter.toString());
	if (region) params.append('region', region);
	if (scheduledAfter) params.append('scheduledAfter', scheduledAfter);
	if (scheduledBefore) params.append('scheduledBefore', scheduledBefore);
	if (startedAfter) params.append('startedAfter', startedAfter);
	if (startedBefore) params.append('startedBefore', startedBefore);
	if (endedAfter) params.append('endedAfter', endedAfter);
	if (endedBefore) params.append('endedBefore', endedBefore);
	if (meesterId) params.append('meesterId', meesterId);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<VeilingKlokOutputDto>>>(`/api/account/meester/veilingklokken?${params.toString()}`);
}

// Start veiling product (POST /api/account/meester/veilingklok/{klokId}/start/{productId})
export async function startVeilingProduct(klokId: string, productId: string): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/veilingklok/${klokId}/start/${productId}`, {
		method: 'POST',
	});
}

// Add product to veilingklok (POST /api/account/meester/veilingklok/{klokId}/product/{productId}?auctionPrice=)
export async function addProductToVeilingKlok(klokId: string, productId: string, auctionPrice: number): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/veilingklok/${klokId}/product/${productId}?auctionPrice=${auctionPrice}`, {
		method: 'POST',
	});
}

// Remove product from veilingklok (DELETE /api/account/meester/veilingklok/{klokId}/product/{productId})
export async function removeProductFromVeilingKlok(klokId: string, productId: string): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/veilingklok/${klokId}/product/${productId}`, {
		method: 'GET',
	});
}

// Update veilingklok status (PUT /api/account/meester/veilingklok/{klokId}/status?status=)
export async function updateVeilingKlokStatus(klokId: string, status: VeilingKlokStatus): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/veilingklok/${klokId}/status?status=${status}`, {
		method: 'POST',
	});
}

// Delete veilingklok (DELETE /api/account/meester/veilingklok/{klokId})
export async function deleteVeilingKlok(klokId: string): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/meester/veilingklok/${klokId}/delete`, {
		method: 'GET',
	});
}
