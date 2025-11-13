import { AccountType } from '../types/AccountTypes';

export interface AuthResponse {
	accessToken: string;
	refreshToken?: string; // Nullable - stored in HTTP-only cookie instead
	accessTokenExpiresAt: Date;
	refreshTokenExpiresAt: Date;
	accountId: number;
	email: string;
	accountType: AccountType;
}
