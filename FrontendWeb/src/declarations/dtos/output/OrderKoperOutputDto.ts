// OrderKwekerOutputDto.ts
import {OrderStatus} from '../../enums/OrderStatus';
import {KoperInfoOutputDto} from './KoperInfoOutputDto';
import {KwekerInfoOutputDto} from './KwekerInfoOutputDto';
import {OrderProductOutputDto} from './OrderProductOutputDto';

export interface OrderKoperOutputDto {
	id: string;
	createdAt: string;
	status: OrderStatus;
	closedAt: string | null;
	quantity: number;
	totalPrice: number;
	products: OrderProductOutputDto[];
	koperInfo: KoperInfoOutputDto;
	kwekerInfo: KwekerInfoOutputDto;
}
