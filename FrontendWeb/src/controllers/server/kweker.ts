import { fetchResponse } from '../../utils/fetchHelpers';
import { HttpSuccess } from '../../declarations/types/HttpSuccess';
import { CreateKwekerDTO } from '../../declarations/dtos/input/CreateKwekerDTO';
import { UpdateKwekerDTO } from '../../declarations/dtos/input/UpdateKwekerDTO';
import { CreateProductDTO } from '../../declarations/dtos/input/CreateProductDTO';
import { UpdateProductDTO } from '../../declarations/dtos/input/UpdateProductDTO';
import { AuthOutputDto } from '../../declarations/dtos/output/AuthOutputDto';
import { AccountOutputDto } from '../../declarations/dtos/output/AccountOutputDto';
import { OrderOutputDto } from '../../declarations/dtos/output/OrderOutputDto';
import { OrderKwekerOutput } from '../../declarations/dtos/output/OrderKwekerOutput';
import { ProductDetailsOutputDto } from '../../declarations/dtos/output/ProductDetailsOutputDto';
import { PaginatedOutputDto } from '../../declarations/dtos/output/PaginatedOutputDto';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { VeilingKlokOutputDto } from '../../declarations/dtos/output/VeilingKlokOutputDto';
import { KwekerStatsOutputDto } from '../../declarations/dtos/output/KwekerStatsOutputDto';
import { KwekerProductStatsOutputDto } from '../../declarations/dtos/output/KwekerProductStatsOutputDto';
import { KwekerOrderStatsOutputDto } from '../../declarations/dtos/output/KwekerOrderStatsOutputDto';
import { OrderStatus } from '../../declarations/enums/OrderStatus';
import { getOrderStatusString } from '../../utils/standards';

// Create kweker account (POST /api/account/kweker/create)
export async function createKwekerAccount(account: CreateKwekerDTO): Promise<HttpSuccess<AuthOutputDto>> {
	return fetchResponse<HttpSuccess<AuthOutputDto>>('/api/account/kweker/create?useCookies=true', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

// Update kweker account (PUT /api/account/kweker/update)
export async function updateKwekerAccount(account: UpdateKwekerDTO): Promise<HttpSuccess<AccountOutputDto>> {
	return fetchResponse<HttpSuccess<AccountOutputDto>>('/api/account/kweker/update', {
		method: 'POST',
		body: JSON.stringify(account),
	});
}

// Update order product (PUT /api/account/kweker/order/{orderId}/product/{productItemId}?quantity=)
export async function updateOrderProduct(orderId: string, productItemId: string, quantity: number): Promise<HttpSuccess<OrderOutputDto>> {
	return fetchResponse<HttpSuccess<OrderOutputDto>>(`/api/account/kweker/order/${orderId}/product/${productItemId}?quantity=${quantity}`, {
		method: 'POST',
	});
}

// Get order (GET /api/account/kweker/order/{orderId})
export async function getOrder(orderId: string): Promise<HttpSuccess<OrderKwekerOutput>> {
	return fetchResponse<HttpSuccess<OrderKwekerOutput>>(`/api/account/kweker/order/${orderId}`);
}

// Get orders (GET /api/account/kweker/orders)
export async function getOrders(productNameFilter?: string, koperNameFilter?: string, statusFilter?: string, beforeDate?: string, afterDate?: string, productId?: string, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<OrderKwekerOutput>>> {
	const params = new URLSearchParams();
	if (productNameFilter) params.append('productNameFilter', productNameFilter);
	if (koperNameFilter) params.append('koperNameFilter', koperNameFilter);
	if (statusFilter) params.append('statusFilter', statusFilter);
	if (beforeDate) params.append('beforeDate', beforeDate);
	if (afterDate) params.append('afterDate', afterDate);
	if (productId) params.append('productId', productId);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<OrderKwekerOutput>>>(`/api/account/kweker/orders?${params.toString()}`);
}

// Update order status (PUT /api/account/kweker/order/{orderId}/status?status=)
export async function updateOrderStatus(orderId: string, status: OrderStatus): Promise<HttpSuccess<string>> {
	return fetchResponse<HttpSuccess<string>>(`/api/account/kweker/order/${orderId}/status?status=${getOrderStatusString(status)}`, {
		method: 'POST',
	});
}

// Create product (POST /api/account/kweker/create-product)
export async function createProduct(product: CreateProductDTO): Promise<HttpSuccess<ProductDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<ProductDetailsOutputDto>>('/api/account/kweker/create-product', {
		method: 'POST',
		body: JSON.stringify(product),
	});
}

// Get product details (GET /api/account/kweker/product/{productId}/details)
export async function getProductDetails(productId: string): Promise<HttpSuccess<ProductDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<ProductDetailsOutputDto>>(`/api/account/kweker/product/${productId}/details`);
}

// Get product orders (GET /api/account/kweker/product/{productId}/orders)
export async function getProductOrders(productId: string, statusFilter?: string, beforeDate?: string, afterDate?: string, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<OrderOutputDto>>> {
	const params = new URLSearchParams();
	if (statusFilter) params.append('statusFilter', statusFilter);
	if (beforeDate) params.append('beforeDate', beforeDate);
	if (afterDate) params.append('afterDate', afterDate);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<OrderOutputDto>>>(`/api/account/kweker/product/${productId}/orders?${params.toString()}`);
}

// Update product (PUT /api/account/kweker/product/{productId})
export async function updateProduct(productId: string, product: UpdateProductDTO): Promise<HttpSuccess<ProductDetailsOutputDto>> {
	return fetchResponse<HttpSuccess<ProductDetailsOutputDto>>(`/api/account/kweker/product/${productId}`, {
		method: 'POST',
		body: JSON.stringify(product),
	});
}

// Get products (GET /api/account/kweker/products)
export async function getProducts(nameFilter?: string, regionFilter?: string, maxPrice?: number, pageNumber: number = 1, pageSize: number = 10): Promise<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>> {
	const params = new URLSearchParams();
	if (nameFilter) params.append('nameFilter', nameFilter);
	if (maxPrice) params.append('maxPrice', maxPrice.toString());
	if (regionFilter) params.append('regionFilter', regionFilter);
	params.append('pageNumber', pageNumber.toString());
	params.append('pageSize', pageSize.toString());

	return fetchResponse<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>>(`/api/account/kweker/products?${params.toString()}`);
}

// Get veilingklok (GET /api/account/kweker/veilingklok/{klokId})
export async function getVeilingKlok(klokId: string): Promise<HttpSuccess<VeilingKlokOutputDto>> {
	return fetchResponse<HttpSuccess<VeilingKlokOutputDto>>(`/api/account/kweker/veilingklok/${klokId}`);
}

// Get kweker stats (GET /api/account/kweker/stats)
export async function getKwekerStats(): Promise<HttpSuccess<KwekerStatsOutputDto>> {
	return fetchResponse<HttpSuccess<KwekerStatsOutputDto>>('/api/account/kweker/stats');
}

// Get kweker product stats (GET /api/account/kweker/product-stats)
export async function getKwekerProductStats(): Promise<HttpSuccess<KwekerProductStatsOutputDto>> {
	return fetchResponse<HttpSuccess<KwekerProductStatsOutputDto>>('/api/account/kweker/product-stats');
}

// Get kweker order stats (GET /api/account/kweker/order-stats)
export async function getKwekerOrderStats(): Promise<HttpSuccess<KwekerOrderStatsOutputDto>> {
	return fetchResponse<HttpSuccess<KwekerOrderStatsOutputDto>>('/api/account/kweker/order-stats');
}
