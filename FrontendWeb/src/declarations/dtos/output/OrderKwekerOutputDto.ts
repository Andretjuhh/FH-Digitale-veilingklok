// OrderKwekerOutputDto.ts
import {OrderStatus} from '../../enums/OrderStatus';
import {KoperInfoOutputDto} from './KoperInfoOutputDto';
import {OrderProductOutputDto} from './OrderProductOutputDto';

export interface OrderKwekerOutputDto {
	id: string;
	createdAt: string;
	status: OrderStatus;
	closedAt: string | null;
	quantity: number;
	totalPrice: number;
	products: OrderProductOutputDto[];
	koperInfo: KoperInfoOutputDto;
}
