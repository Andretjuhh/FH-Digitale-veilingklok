// CreateProductDTO.ts
export interface CreateProductDTO {
	name: string;
	description: string;
	minimumPrice: number;
	stock: number;
	imageBase64: string;
	dimension?: string;
}
