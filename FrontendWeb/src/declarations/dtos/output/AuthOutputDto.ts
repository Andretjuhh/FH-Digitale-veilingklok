import { AccountType } from '../../enums/AccountTypes';

// AuthOutputDto.ts
export interface AuthOutputDto {
	accessToken: string;
	accessTokenExpiresAt: string;
	accountType: AccountType;
}
