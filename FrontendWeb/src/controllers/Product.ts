import { fetchResponse } from '../utils/fetchHelpers';
import { ProductDetails } from '../declarations/ProductDetails';

// Payload expected by the backend when creating a product
export interface NewProduct {
    name: string;
    description?: string | null;
    price: number;
    minimumPrice?: number | null;
    quantity?: number | null;
    imageUrl?: string | null;
    size?: string | null;
    // Note: KwekerId is determined server-side from the authenticated account; do not send it from the client
}

export async function createProduct(product: NewProduct) {
    return await fetchResponse<{ message: string; product: ProductDetails }>(
        '/api/product/create',
        {
            method: 'POST',
            body: JSON.stringify(product),
        }
    );
}
export async function getProducts() {
    return await fetchResponse<{ products: ProductDetails[] }>('/api/product/all', {
        method: 'GET',
    });
}
export async function getProductById(productId: number) {
    return await fetchResponse<ProductDetails>(`/api/product/${productId}`, {
        method: 'GET',
    });
}