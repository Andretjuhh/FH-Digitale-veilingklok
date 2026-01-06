// ProductDetailsOutputDto.ts
export interface ProductDetailsOutputDto {
	id: string;
	createdAt: string;
	name: string;
	description: string;
	auctionPrice: number | null;
	minimumPrice: number;
	stock: number;
	imageBase64: string;
	dimension: string;
	auctioned: boolean;
	auctionedCount: number;
	auctionedAt: string | null;
	kwekerId: string;
	companyName: string;
}
