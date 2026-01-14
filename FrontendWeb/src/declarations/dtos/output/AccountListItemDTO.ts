import { AccountType } from '../../enums/AccountTypes';

export interface AccountListItemDTO {
	id: string;
	email: string;
	accountType: AccountType;
	createdAt: string;
	deletedAt: string | null;
	isDeleted: boolean;
}
