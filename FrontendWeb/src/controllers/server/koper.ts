import {fetchResponse} from '../../utils/fetchHelpers';
import {HttpSuccess} from '../../declarations/types/HttpSuccess';
import {CreateKoperDTO} from '../../declarations/dtos/input/CreateKoperDTO';
import {UpdateKoperDTO} from '../../declarations/dtos/input/UpdateKoperDTO';
import {AddressInputDTO} from '../../declarations/dtos/input/AddressInputDTO';
import {CreateOrderDTO} from '../../declarations/dtos/input/CreateOrderDTO';
import {AddressOutputDto} from '../../declarations/dtos/output/AddressOutputDto';
import {AuthOutputDto} from '../../declarations/dtos/output/AuthOutputDto';
import {AccountOutputDto} from '../../declarations/dtos/output/AccountOutputDto';
import {OrderOutputDto} from '../../declarations/dtos/output/OrderOutputDto';
import {OrderDetailsOutputDto} from '../../declarations/dtos/output/OrderDetailsOutputDto';
import {PaginatedOutputDto} from '../../declarations/dtos/output/PaginatedOutputDto';
import {OrderItemOutputDto} from '../../declarations/dtos/output/OrderItemOutputDto';
import {ProductOutputDto} from '../../declarations/dtos/output/ProductOutputDto';
import {VeilingKlokOutputDto} from '../../declarations/dtos/output/VeilingKlokOutputDto';
import {VeilingKlokStatus} from "../../declarations/enums/VeilingKlokStatus";

// Create koper account (POST /api/account/koper/create)
export async function createKoperAccount(account: CreateKoperDTO): Promise<HttpSuccess<AuthOutputDto>> {
	return fetchResponse<HttpSuccess<AuthOutputDto>>('/api/account/koper/create?useCookies=true', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

// Update koper account (PUT /api/account/koper/update)
export async function updateKoperAccount(account: UpdateKoperDTO): Promise<HttpSuccess<AccountOutputDto>> {
	return fetchResponse<HttpSuccess<AccountOutputDto>>('/api/account/koper/update', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

// Create address (POST /api/account/koper/address)
export async function createKoperAddress(address: AddressInputDTO): Promise<HttpSuccess<AddressOutputDto>> {
	return fetchResponse<HttpSuccess<AddressOutputDto>>('/api/account/koper/address', {
		method: 'POST',
		body: JSON.stringify(address),
	});
}

// Update primary address (PUT /api/account/koper/address/primary/{addressId})
export async function updatePrimaryAddress(addressId: number): Promise<HttpSuccess<AddressOutputDto>> {
	return fetchResponse<HttpSuccess<AddressOutputDto>>(`/api/account/koper/address/primary/${addressId}`, {
		method: 'POST',
	});
}

// Create order (POST /api/account/koper/create-order)
export async function createOrder(order: CreateOrderDTO): Promise<HttpSuccess<OrderOutputDto>> {
	return fetchResponse<HttpSuccess<OrderOutputDto>>('/api/account/koper/create-order', {
		method: 'POST',
		body: JSON.stringify(order),
	});
}

// Get order (GET /api/account/koper/order/{orderId})
export async function getOrder(orderId: string): Promise<HttpSuccess<OrderDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<OrderDetailsOutputDto>>(`/api/account/koper/order/${orderId}`);
}

// Get orders (GET /api/account/koper/orders)
export async function getOrders(status?: string, beforeDate?: string, afterDate?: string, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<OrderOutputDto>>> {
	const params = new URLSearchParams();
	if (status) params.append('status', status);
	if (beforeDate) params.append('beforeDate', beforeDate);
	if (afterDate) params.append('afterDate', afterDate);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<OrderOutputDto>>>(`/api/account/koper/orders?${params.toString()}`);
}

// Order product (POST /api/account/koper/order/{orderId}/product)
export async function orderProduct(orderId: string, productId: string, quantity: number): Promise<HttpSuccess<OrderItemOutputDto>> {
	return fetchResponse<HttpSuccess<OrderItemOutputDto>>(`/api/account/koper/order/${orderId}/product?productId=${productId}&quantity=${quantity}`, {
		method: 'POST',
	});
}

// Get product (GET /api/account/koper/product/{productId})
export async function getProduct(productId: string): Promise<HttpSuccess<ProductOutputDto>> {
	return fetchResponse<HttpSuccess<ProductOutputDto>>(`/api/account/koper/product/${productId}`);
}

// Get products (GET /api/account/koper/products)
export async function getProducts(nameFilter?: string, regionFilter?: string, maxPrice?: number, kwekerId?: string, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>> {
	const params = new URLSearchParams();
	if (nameFilter) params.append('nameFilter', nameFilter);
	if (regionFilter) params.append('regionFilter', regionFilter);
	if (maxPrice) params.append('maxPrice', maxPrice.toString());
	if (kwekerId) params.append('kwekerId', kwekerId);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>>(`/api/account/koper/products?${params.toString()}`);
}

// Get veilingklok (GET /api/account/koper/veilingklok/{klokId})
export async function getVeilingKlok(klokId: string): Promise<HttpSuccess<VeilingKlokOutputDto>> {
	return fetchResponse<HttpSuccess<VeilingKlokOutputDto>>(`/api/account/koper/veilingklok/${klokId}`);
}

// Get veilingklokken (GET /api/account/koper/veilingklokken)
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

	return fetchResponse<HttpSuccess<PaginatedOutputDto<VeilingKlokOutputDto>>>(`/api/account/koper/veilingklokken?${params.toString()}`);
}

