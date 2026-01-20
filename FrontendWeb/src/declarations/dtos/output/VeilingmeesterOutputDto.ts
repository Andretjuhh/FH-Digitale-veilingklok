import { AccountType } from '../../enums/AccountTypes';

// VeilingmeesterOutputDto.ts
export interface VeilingmeesterOutputDto {
	accountType: AccountType;
	email: string;
	countryCode: string;
	region: string;
}
