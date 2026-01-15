import { VeilingKlokStatus } from '../enums/VeilingKlokStatus';

// VeilingNotifications.ts

export interface RegionVeilingStartedNotification {
	clockId: string; // Guid
	country: string;
	region: string;
	startTime: string; // ISO string
}

export interface VeilingKlokStateNotification {
	status: VeilingKlokStatus;
	clockId: string;
	currentProductId: string;
	currentPrice: number;
	startingPrice: number;
	lowestPrice: number;
	remainingQuantity: number;
	liveViewerCount: number;
	endTime: string; // ISO string
	totalRounds: number;
}

export interface VeilingBodNotification {
	productId: string;
	quantity: number;
	price: number;
	remainingQuantity: number;
}

export interface VeilingProductChangedNotification {
	productId: string;
	startingPrice: number;
	quantity: number;
}

export interface VeilingPriceTickNotification {
	clockId: string;
	productId: string;
	currentPrice: number;
	tickTime: string; // ISO string
}

export interface VeilingProductWaitingNotification {
	clockId: string;
	completedProductId: string;
}
