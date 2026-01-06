import { AddressInputDTO } from './AddressInputDTO';

// CreateKoperDTO.ts
export interface CreateKoperDTO {
	email: string;
	password: string;
	firstName: string;
	lastName: string;
	telephone: string;
	address: AddressInputDTO;
}
