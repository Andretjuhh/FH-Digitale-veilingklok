// UpdateProductDTO.ts
export interface UpdateProductDTO {
	name?: string;
	description?: string;
	minimumPrice?: number;
	stock?: number;
	imageBase64?: string;
	dimension?: string;
}
