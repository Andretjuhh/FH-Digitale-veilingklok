// OrderItemOutputDto.ts
export interface OrderItemOutputDto {
	productId: string;
	productName: string;
	productDescription: string;
	productImageUrl: string;
	companyName: string;
	quantity: number;
	priceAtPurchase: number;
	orderedAt: string;
}
