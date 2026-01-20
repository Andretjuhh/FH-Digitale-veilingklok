import { AddressInputDTO } from './AddressInputDTO';

// UpdateKwekerDTO.ts
export interface UpdateKwekerDTO {
	companyName?: string;
	firstName?: string;
	lastName?: string;
	email?: string;
	password?: string;
	telephone?: string;
	kvkNumber?: string;
	address?: AddressInputDTO;
}
