import { VeilingKlokStatus } from '../../enums/VeilingKlokStatus';
import { ProductDetailsOutputDto } from './ProductDetailsOutputDto';

// VeilingKlokDetailsOutputDto.ts
export interface VeilingKlokDetailsOutputDto {
	id: string;
	status: VeilingKlokStatus;
	peakedLiveViews: number;
	createdAt: string;
	regionOrState: string;
	country: string;
	currentBids: number;
	totalProducts: number;
	scheduledAt: string | null;
	startedAt: string | null;
	endedAt: string | null;
	highestBidAmount: number | null;
	lowestBidAmount: number | null;
	products: ProductDetailsOutputDto[];
}
