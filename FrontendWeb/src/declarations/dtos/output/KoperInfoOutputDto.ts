// KoperInfoOutputDto.ts
import { AddressOutputDto } from './AddressOutputDto';

export interface KoperInfoOutputDto {
	email: string;
	firstName: string;
	lastName: string;
	telephone: string;
	address: AddressOutputDto;
}
