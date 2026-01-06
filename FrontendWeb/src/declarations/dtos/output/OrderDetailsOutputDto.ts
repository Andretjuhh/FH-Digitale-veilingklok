import { OrderStatus } from '../../enums/OrderStatus';
import { OrderItemOutputDto } from './OrderItemOutputDto';

// OrderDetailsOutputDto.ts
export interface OrderDetailsOutputDto {
	id: string;
	createdAt: string;
	status: OrderStatus;
	closedAt?: string;
	totalAmount: number;
	totalItems: number;
	products: OrderItemOutputDto[];
}
