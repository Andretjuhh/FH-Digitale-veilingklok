import { OrderStatus } from '../../enums/OrderStatus';

// OrderOutputDto.ts
export interface OrderOutputDto {
	id: string;
	createdAt: string;
	status: OrderStatus;
	closedAt?: string;
	totalAmount: number;
	totalItems: number;
	productId: string;
	productName: string;
	productDescription: string;
	productImageUrl: string;
	companyName: string;
}
