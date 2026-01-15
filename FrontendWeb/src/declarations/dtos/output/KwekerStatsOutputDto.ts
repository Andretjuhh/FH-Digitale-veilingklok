// KwekerStatsOutputDto.ts
export interface KwekerStatsOutputDto {
	totalProducts: number;
	activeAuctions: number;
	totalRevenue: number;
	ordersReceived: number;
	monthlyRevenue: MonthlyRevenueDto[];
}

export interface MonthlyRevenueDto {
	year: number;
	month: number;
	monthName: string;
	revenue: number;
}
