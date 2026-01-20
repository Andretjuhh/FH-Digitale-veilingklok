import { AccountType } from '../../enums/AccountTypes';
import { AddressOutputDto } from './AddressOutputDto';

// AccountOutputDto.ts
export interface AccountOutputDto {
	accountType: AccountType;
	email: string;
	firstName?: string;
	lastName?: string;
	telephone?: string;
	companyName?: string;
	kvkNumber?: string;
	countryCode?: string;
	region?: string;
	address?: AddressOutputDto;
	primaryAddressId?: number;
	addresses?: AddressOutputDto[];
}
