import React, {useCallback, useEffect, useRef, useState} from 'react';
import * as signalR from '@microsoft/signalr';
import {AuctionClockRef} from '../components/elements/AuctionClock';
import {getAuthentication} from '../controllers/server/account';
import {
	RegionVeilingStartedNotification,
	VeilingBodNotification,
	VeilingPriceTickNotification,
	VeilingProductChangedNotification,
	VeilingProductWaitingNotification
} from '../declarations/models/VeilingNotifications';
import config from '../constant/application';

interface UseVeilingKlokSignalRProps {
	hubUrl?: string;
	country?: string;
	region: string;
	clockRef: React.RefObject<AuctionClockRef | null>;
	onVeilingStarted?: (state: RegionVeilingStartedNotification) => void;
	onVeilingEnded?: () => void;
	onBidPlaced?: (bid: VeilingBodNotification) => void;
	onProductChanged?: (product: VeilingProductChangedNotification) => void;
	onAuctionEnded?: () => void;
	onViewerCountChanged?: (count: number) => void;
	onPriceTick?: (state: VeilingPriceTickNotification) => void;
	onProductWaitingForNext?: (notification: VeilingProductWaitingNotification) => void;
}

export function useVeilingKlokSignalR(props: UseVeilingKlokSignalRProps) {
	const {
		hubUrl = config.KLOK_HUB_URL,
		country = 'NL',
		region,
		clockRef,
		onVeilingStarted,
		onVeilingEnded,
		onBidPlaced,
		onProductChanged,
		onAuctionEnded,
		onViewerCountChanged,
		onPriceTick,
		onProductWaitingForNext
	} = props;

	const connectionRef = useRef<signalR.HubConnection | null>(null);
	const [klokConnectionStatus, setKlokConnectionStatus] = useState<signalR.HubConnectionState>(signalR.HubConnectionState.Disconnected);

	const connect = useCallback(async () => {
		if (connectionRef.current) {
			return;
		}

		const connection = new signalR.HubConnectionBuilder()
			.withUrl(hubUrl, {
				accessTokenFactory: async () => {
					const auth = await getAuthentication();
					return auth?.accessToken ?? '';
				},
			})
			.withAutomaticReconnect({
				nextRetryDelayInMilliseconds: (retryContext) => {
					// Exponential backoff: 0s, 2s, 10s, 30s
					if (retryContext.previousRetryCount === 0) return 0;
					if (retryContext.previousRetryCount === 1) return 2000;
					if (retryContext.previousRetryCount === 2) return 10000;
					return 30000;
				},
			})
			.configureLogging(signalR.LogLevel.Information)
			.build();

		// Register event handlers
		connection.on('RegionVeilingStarted', (state: RegionVeilingStartedNotification) => {
			console.log('Region veiling started:', state);
			// Note: clockRef.tick might expect a different type, adapt as needed
			onVeilingStarted?.(state);
		});

		connection.on('RegionVeilingEnded', () => {
			console.log('Region veiling ended');
			onVeilingEnded?.();
		});

		connection.on('VeilingBodPlaced', (bid: VeilingBodNotification) => {
			console.log('Bid placed:', bid);
			onBidPlaced?.(bid);
		});

		connection.on('VeilingProductChanged', (product: VeilingProductChangedNotification) => {
			console.log('Product changed:', product);
			onProductChanged?.(product);
		});

		connection.on('VeilingEnded', () => {
			console.log('Auction ended');
			clockRef.current?.reset();
			onAuctionEnded?.();
		});

		connection.on('ViewerCountChanged', (count: number) => {
			console.log('Viewer count:', count);
			onViewerCountChanged?.(count);
		});

		connection.on('VeilingPriceTick', (state: VeilingPriceTickNotification) => {
			console.log('Price tick:', state.currentPrice);
			// Update the clock with new price
			clockRef.current?.tick(state);
			// Call optional callback
			onPriceTick?.(state);
		});

		connection.on('VeilingProductWaiting', (klokId: string, completedProductId: string) => {
			const notification: VeilingProductWaitingNotification = {clockId: klokId, completedProductId};
			console.log('Product waiting for next:', notification);
			clockRef.current?.pause();
			onProductWaitingForNext?.(notification);
		});

		// Handle reconnection events
		connection.onreconnecting((error) => {
			console.warn('SignalR reconnecting...', error);
			setKlokConnectionStatus(signalR.HubConnectionState.Reconnecting);
		});

		connection.onreconnected((connectionId) => {
			console.log('SignalR reconnected:', connectionId);
			setKlokConnectionStatus(signalR.HubConnectionState.Connected);
			// Rejoin the group after reconnection
			connection.invoke('JoinRegion', country, region).catch((err) => {
				console.error('Failed to rejoin group:', err);
			});
		});

		connection.onclose((error) => {
			console.error('SignalR connection closed:', error);
			connectionRef.current = null;
			setKlokConnectionStatus(signalR.HubConnectionState.Disconnected);
		});

		try {
			setKlokConnectionStatus(signalR.HubConnectionState.Connecting);
			await connection.start();
			console.log('SignalR connected');
			setKlokConnectionStatus(signalR.HubConnectionState.Connected);

			// Join the region group
			await joinRegion(country, region);
			console.log(`Joined region group: ${country}, ${region}`);

			connectionRef.current = connection;
		} catch (error) {
			console.error('SignalR connection error:', error);
			connectionRef.current = null;
			setKlokConnectionStatus(signalR.HubConnectionState.Disconnected);
		}
	}, [hubUrl, region, clockRef, onVeilingStarted, onVeilingEnded, onBidPlaced, onProductChanged, onAuctionEnded, onViewerCountChanged, onPriceTick, onProductWaitingForNext]);
	const disconnect = useCallback(async () => {
		if (connectionRef.current) {
			try {
				await connectionRef.current.invoke('LeaveRegion', country, region);
				await connectionRef.current.stop();
				console.log('SignalR disconnected');
			} catch (error) {
				console.error('Error disconnecting SignalR:', error);
			}
			connectionRef.current = null;
		}
	}, [region]);

	const joinRegion = useCallback(async (country: string, region: string) => {
		if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
			try {
				await connectionRef.current.invoke('JoinRegion', country, region);
				console.log(`Joined region: ${country}, ${region}`);
			} catch (error) {
				console.error(`Error joining region: ${country}, ${region}`, error);
			}
		}
	}, []);

	const leaveRegion = useCallback(async (country: string, region: string) => {
		if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
			try {
				await connectionRef.current.invoke('LeaveRegion', country, region);
				console.log(`Left region: ${country}, ${region}`);
			} catch (error) {
				console.error(`Error leaving region: ${country}, ${region}`, error);
			}
		}
	}, []);

	const joinClock = useCallback(async (klokId: string) => {
		if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
			try {
				setKlokConnectionStatus(signalR.HubConnectionState.Connecting);
				await connectionRef.current.invoke('JoinClock', klokId);
				console.log(`Joined clock: ${klokId}`);
				setKlokConnectionStatus(signalR.HubConnectionState.Connected);
			} catch (error) {
				console.error(`Error joining clock: ${klokId}`, error);
				setKlokConnectionStatus(signalR.HubConnectionState.Disconnected);
			}
		}
	}, []);

	const leaveClock = useCallback(async (klokId: string) => {
		if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
			try {
				await connectionRef.current.invoke('LeaveClock', klokId);
				console.log(`Left clock: ${klokId}`);
				setKlokConnectionStatus(signalR.HubConnectionState.Disconnected);
			} catch (error) {
				console.error(`Error leaving clock: ${klokId}`, error);
			}
		}
	}, []);

	useEffect(() => {
		connect();

		return () => {
			disconnect();
		};
	}, [connect, disconnect]);

	return {
		connect,
		disconnect,
		joinRegion,
		leaveRegion,
		joinClock,
		leaveClock,
		connection: connectionRef.current,
		isConnected: connectionRef.current?.state === signalR.HubConnectionState.Connected,
		connectionStatus: connectionRef.current?.state, // EDITED: Added connectionStatus to match your hook
		klokConnectionStatus,
	};
}
