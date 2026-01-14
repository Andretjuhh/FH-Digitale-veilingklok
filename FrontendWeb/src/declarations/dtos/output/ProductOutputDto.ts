// ProductOutputDto.ts
export interface ProductOutputDto {
	id: string;
	name: string;
	description: string;
	imageUrl: string;
	auctionedPrice: number | null;
	auctionedAt: string | null;
	dimension: string;
	stock: number;
	companyName: string;
	kwekerId: string;
}
