// OrderProductOutputDto.ts
export interface OrderProductOutputDto {
	productId: string;
	productName: string;
	productDescription: string;
	productImageUrl: string;
	companyName: string;
	quantity: number;
	priceAtPurchase: number;
	minimalPrice?: number;
	orderedAt: string;
}
