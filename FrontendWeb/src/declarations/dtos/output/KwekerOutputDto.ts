import { AccountType } from '../../enums/AccountTypes';
import { AddressOutputDto } from './AddressOutputDto';

// KwekerOutputDto.ts
export interface KwekerOutputDto {
	accountType: AccountType;
	email: string;
	kvkNumber: string;
	companyName: string;
	firstName: string;
	lastName: string;
	telephone: string;
	address: AddressOutputDto;
}
