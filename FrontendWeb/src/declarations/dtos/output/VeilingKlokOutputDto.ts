import {VeilingKlokStatus} from '../../enums/VeilingKlokStatus';
import {ProductOutputDto} from './ProductOutputDto';

// VeilingKlokOutputDto.ts
export interface VeilingKlokOutputDto {
	id: string;
	status: VeilingKlokStatus;
	peakedLiveViews: number;
	createdAt: string;
	regionOrState: string;
	country: string;
	currentBids: number;
	totalProducts: number;
	scheduledAt: string;
	startedAt: string | null;
	endedAt: string | null;
	veilingDurationSeconds: number;

	veilingRounds: number | null;
	currentProductIndex: number | null;
	highestBidAmount: number | null;
	lowestBidAmount: number | null;
	products: ProductOutputDto[];
}
