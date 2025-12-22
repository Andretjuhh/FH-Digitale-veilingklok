import { AddressInputDTO } from './AddressInputDTO';

// UpdateKoperDTO.ts
export interface UpdateKoperDTO {
	email: string;
	password: string;
	firstName: string;
	lastName: string;
	telephone: string;
	address: AddressInputDTO;
}
