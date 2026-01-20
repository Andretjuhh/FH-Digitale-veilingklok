import { AccountType } from '../../enums/AccountTypes';
import { AddressOutputDto } from './AddressOutputDto';

// KoperOutputDto.ts
export interface KoperOutputDto {
	accountType: AccountType;
	email: string;
	firstName: string;
	lastName: string;
	telephone: string;
	primaryAddressId: number;
	addresses: AddressOutputDto[];
}
