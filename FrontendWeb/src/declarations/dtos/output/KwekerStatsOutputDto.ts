// KwekerStatsOutputDto.ts
export interface KwekerStatsOutputDto {
	totalProducts: number;
	activeAuctions: number;
	totalRevenue: number;
	ordersReceived: number;
	monthlyRevenue: MonthlyRevenueDto[];
	dailyRevenue: DailyRevenueDto[];
}

export interface MonthlyRevenueDto {
	year: number;
	month: number;
	monthName: string;
	revenue: number;
}

export interface DailyRevenueDto {
	year: number;
	month: number;
	day: number;
	dateLabel: string;
	revenue: number;
}
