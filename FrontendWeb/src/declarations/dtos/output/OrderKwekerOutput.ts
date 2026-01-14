// OrderKwekerOutput.ts
import {OrderStatus} from '../../enums/OrderStatus';
import {ProductOutputDto} from './ProductOutputDto';
import {KoperInfoOutputDto} from './KoperInfoOutputDto';

export interface OrderKwekerOutput {
	id: string;
	createdAt: string;
	status: OrderStatus;
	closedAt: string | null;
	quantity: number;
	totalPrice: number;
	product: ProductOutputDto;
	koperInfo: KoperInfoOutputDto;
}
