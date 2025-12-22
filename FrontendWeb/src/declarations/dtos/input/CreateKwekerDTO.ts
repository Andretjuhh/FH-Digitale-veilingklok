import { AddressInputDTO } from './AddressInputDTO';

// CreateKwekerDTO.ts
export interface CreateKwekerDTO {
	companyName: string;
	firstName: string;
	lastName: string;
	email: string;
	password: string;
	telephone: string;
	address: AddressInputDTO;
	kvkNumber: string;
}
