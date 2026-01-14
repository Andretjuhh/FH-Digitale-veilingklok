// ProductOutputDto.ts
export interface ProductOutputDto {
	id: string;
	name: string;
	description: string;
	imageUrl: string;
	auctionedPrice: number | null;
	minimumPrice: number | null;
	auctionedAt: string | null;
	region?: string;
	dimension: string;
	stock: number;
	companyName: string;
	auctionPlanned: boolean;
}
