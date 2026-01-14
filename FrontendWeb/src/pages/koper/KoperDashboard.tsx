// External imports
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

// Internal imports
import Page from '../../components/nav/Page';
import Button from '../../components/buttons/Button';
import AuctionClock from '../../components/elements/AuctionClock';
import { useRootContext } from '../../components/contexts/RootContext';
import { getAuthentication } from '../../controllers/server/account';
import {
	createOrder,
	getProducts,
	orderProduct,
	getVeilingKlok,
	getKwekerAveragePrice,
	getKwekerPriceHistory,
	getLatestPrices,
} from '../../controllers/server/koper';
import config from '../../constant/application';
import { RegionVeilingStartedNotification, VeilingPriceTickNotification, VeilingProductChangedNotification } from '../../declarations/models/VeilingNotifications';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { PriceHistoryItemOutputDto } from '../../declarations/dtos/output/PriceHistoryItemOutputDto';
import { KwekerAveragePriceOutputDto } from '../../declarations/dtos/output/KwekerAveragePriceOutputDto';
import { formatEur } from '../../utils/standards';

function UserDashboard() {
	const { t, account } = useRootContext();
	const CLOCK_SECONDS = 4;
	const DEFAULT_COUNTRY = 'NL';
	const DEFAULT_REGION = 'Noord-Holland';
	const [price, setPrice] = useState<number>(0.65);
	const [products, setProducts] = useState<ProductOutputDto[]>([]);
	const [loading, setLoading] = useState(true);
	const [productIndex, setProductIndex] = useState<number>(0);
	const [isClockRunning, setIsClockRunning] = useState(false);
	const [clockId, setClockId] = useState<string | null>(null);
	const [orderId, setOrderId] = useState<string | null>(null);
	const [isHydratingClock, setIsHydratingClock] = useState<boolean>(false);
	const [kwekerHistory, setKwekerHistory] = useState<PriceHistoryItemOutputDto[]>([]);
	const [kwekerAverage, setKwekerAverage] = useState<KwekerAveragePriceOutputDto | null>(null);
	const [allHistory, setAllHistory] = useState<PriceHistoryItemOutputDto[]>([]);
	const [historyLoading, setHistoryLoading] = useState<boolean>(false);
	const connectionRef = useRef<HubConnection | null>(null);

	const initializeProducts = useCallback(async () => {
		setLoading(true);
		try {
			const response = await getProducts();
			if (response.data) {
				setProducts(response.data.data);
			}
		} catch (error) {
			console.error('Failed to fetch products:', error);
		} finally {
			setLoading(false);
		}
	}, []);

	const current = products[productIndex];
	const currentKwekerId = current?.kwekerId ?? null;

	useEffect(() => {
		initializeProducts();
	}, [initializeProducts]);

	useEffect(() => {
		if (!currentKwekerId) return;
		let isActive = true;
		setHistoryLoading(true);
		Promise.all([
			getKwekerPriceHistory(currentKwekerId, 10),
			getKwekerAveragePrice(currentKwekerId),
			getLatestPrices(10),
		])
			.then(([kwekerHistoryResp, kwekerAvgResp, allHistoryResp]) => {
				if (!isActive) return;
				setKwekerHistory(kwekerHistoryResp.data ?? []);
				setKwekerAverage(kwekerAvgResp.data ?? null);
				setAllHistory(allHistoryResp.data ?? []);
			})
			.catch((err) => {
				if (!isActive) return;
				console.error('Failed to fetch price history:', err);
			})
			.finally(() => {
				if (isActive) setHistoryLoading(false);
			});

		return () => {
			isActive = false;
		};
	}, [currentKwekerId]);

	// afbeelding bron per product
	const [imgSrc, setImgSrc] = useState<string>('');
	useEffect(() => {
		if (current) {
			setImgSrc(current.imageUrl || '/pictures/kweker.png');
		}
	}, [current]);

	useEffect(() => {
		if (!current) return;
		if (!isClockRunning) {
			setPrice(current.auctionedPrice ?? 0.65);
		}
	}, [current, isClockRunning]);

	const upcoming = useMemo(() => {
		if (products.length === 0) return [];
		const after = products.slice(productIndex + 1);
		const before = products.slice(0, productIndex);
		return [...after, ...before];
	}, [products, productIndex]);

	const [paused, setPaused] = useState<boolean>(false);
	const [resetToken, setResetToken] = useState<number>(0);

	useEffect(() => {
		const country = account?.countryCode ?? account?.address?.country ?? DEFAULT_COUNTRY;
		const region = account?.region ?? account?.address?.regionOrState ?? DEFAULT_REGION;
		let isActive = true;

		const startSignalR = async () => {
			const connection = new HubConnectionBuilder()
				.withUrl(`${config.API}hubs/veiling-klok`, {
					accessTokenFactory: async () => {
						const auth = await getAuthentication();
						return auth?.accessToken ?? '';
					},
				})
				.withAutomaticReconnect()
				.configureLogging(LogLevel.Warning)
				.build();

			connection.on('RegionVeilingStarted', (notification: RegionVeilingStartedNotification) => {
				if (!isActive) return;
				setClockId(notification.clockId);
				setOrderId(null);
				setIsClockRunning(true);
				setPaused(false);
				setResetToken((v) => v + 1);
				setIsHydratingClock(true);
				getVeilingKlok(notification.clockId)
					.then((resp) => {
						const list = resp?.data?.products ?? [];
						setProducts(list);
						setProductIndex(0);
						if (list[0]) {
							setPrice(list[0].auctionedPrice ?? 0.65);
						}
					})
					.catch((err) => console.error('Failed to hydrate veilingklok:', err))
					.finally(() => setIsHydratingClock(false));
				connection.invoke('JoinClock', notification.clockId).catch((err) => {
					console.error('JoinClock failed:', err);
				});
			});

			connection.on('VeilingPriceTick', (notification: VeilingPriceTickNotification) => {
				if (!isActive) return;
				setPrice(notification.currentPrice);
			});

			connection.on('VeilingProductChanged', (notification: VeilingProductChangedNotification) => {
				if (!isActive) return;
				setPrice(notification.startingPrice);
				setProducts((prev) => {
					const idx = prev.findIndex((p) => p.id === notification.productId);
					if (idx >= 0) setProductIndex(idx);
					return prev;
				});
			});

			connection.on('VeilingEnded', () => {
				if (!isActive) return;
				setIsClockRunning(false);
			});

			connection.on('RegionVeilingEnded', () => {
				if (!isActive) return;
				setIsClockRunning(false);
			});

			try {
				await connection.start();
				await connection.invoke('JoinRegion', country, region);
			} catch (err) {
				console.error('SignalR connection failed:', err);
			}

			connectionRef.current = connection;
		};

		startSignalR();

		return () => {
			isActive = false;
			const connection = connectionRef.current;
			connectionRef.current = null;
			if (connection) {
				connection.stop().catch(() => undefined);
			}
		};
	}, [account?.countryCode, account?.region, account?.address?.country, account?.address?.regionOrState]);

	const [qty, setQty] = useState<number>(5);
	const currentStock = current?.stock ?? 0;

	useEffect(() => {
		// clamp quantity when product or stock changes
		setQty((q) => Math.max(0, Math.min(currentStock, q)));
	}, [currentStock, productIndex]);

	if (loading || isHydratingClock) {
		return (
			<Page enableHeader className="user-dashboard">
				<div className="flex items-center justify-center h-64">
					<div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary-main"></div>
				</div>
			</Page>
		);
	}

	if (products.length === 0) {
		return (
			<Page enableHeader className="user-dashboard">
				<div className="flex flex-col items-center justify-center h-64">
					<p className="text-gray-500 mb-4">{t('no_products_available')}</p>
					<Button label={t('refresh')} onClick={initializeProducts} />
				</div>
			</Page>
		);
	}

	// prijs wordt gestuurd door de klok (onTick)

	return (
		<Page enableHeader className="user-dashboard">
			<section className="user-hero">
				<div className="user-hero-head">
					<div>
						<h1 className="user-hero-title">{t('koper_dashboard')}</h1>
						<p className="user-hero-sub">{t('koper_dashboard_sub')}</p>
					</div>
				</div>
			</section>

			<section className="user-card-wrap">
				<div className="user-card">
					{/* Left media block */}
					<div className="user-card-mediaBlock">
						<img
							className="user-card-media"
							src={imgSrc}
							onError={() => setImgSrc((prev) => (prev.endsWith('.svg') ? '/pictures/kweker.png' : '/pictures/roses.svg'))}
							alt={t('koper_product_image_alt')}
						/>
						<div className="product-info">
							<div className="prod-row">
								<span className="prod-label">{t('koper_supplier')}</span>
								<span className="prod-val">{current.companyName}</span>
								{/*<span className="prod-label">{t('koper_id')}</span>*/}
								<span className="prod-val">{current.id.substring(0, 8)}</span>
							</div>
							<div className="prod-row">
								<span className="prod-label">{t('koper_product')}</span>
								<span className="prod-val prod-val--wide">{current.name}</span>
								{/*<span className="prod-label">{t('koper_dimension')}</span>*/}
								<span className="prod-val">{current.dimension}</span>
							</div>
							<div className="prod-row">
								{/*<span className="prod-label">{t('koper_stock_label')}</span>*/}
								<span className="prod-val prod-val--wide">{current.stock}</span>
							</div>
						</div>
					</div>

					{/* Center profile area */}
					<div className="user-card-center">
						<AuctionClock
							totalSeconds={CLOCK_SECONDS}
							start={isClockRunning}
							paused={paused || !isClockRunning}
							resetToken={resetToken}
							round={1}
							coin={1}
							amountPerLot={1}
							minAmount={1}
							price={price}
						/>

						<div className="stock-text">{t('koper_stock', { count: currentStock })}</div>

						<div className="user-actions">
							<div className="buy-controls">
								<Button
									className="user-action-btn !bg-primary-main buy-full"
									label={`${t('koper_buy')} (${qty})`}
									onClick={async () => {
										if (qty <= 0 || !current) return;
										setPaused(true);
										try {
											let currentOrderId = orderId;
											if (!currentOrderId && clockId) {
												const orderResp = await createOrder({ veilingKlokId: clockId });
												currentOrderId = (orderResp as any)?.data?.id ?? (orderResp as any)?.data?.data?.id ?? null;
												if (currentOrderId) setOrderId(currentOrderId);
											}

											if (!currentOrderId) throw new Error('Geen orderId beschikbaar om aankoop te plaatsen.');

											await orderProduct(currentOrderId, current.id, qty);

											const nextStock = Math.max(0, currentStock - qty);
											setProducts((prev) => prev.map((p, i) => (i === productIndex ? { ...p, stock: nextStock } : p)));
											setResetToken((v) => v + 1);
											setPrice(current.auctionedPrice ?? 0.65);
											if (nextStock <= 0) setProductIndex((i) => (i + 1) % products.length);
										} catch (error) {
											console.error('Order failed:', error);
											setPaused(false);
										}
									}}
								/>
								<div className="buy-inline">
									<input
										type="number"
										min={0}
										max={currentStock}
										className="buy-inline-input"
										value={Number.isFinite(qty) ? qty : ''}
										onChange={(e) => {
											const val = parseInt(e.target.value, 10);
											if (Number.isNaN(val)) {
												setQty(0);
												return;
											}
											setQty(Math.max(0, Math.min(currentStock, val)));
										}}
										aria-label={t('koper_qty_input_aria')}
									/>
									<Button className="qty-max-btn btn-outline" label={t('koper_max_stock')} aria-label={t('koper_max_stock')} onClick={() => setQty(currentStock)} disabled={currentStock === 0} />
								</div>
							</div>
						</div>
					</div>

					{/* Right side: compacte wachtrij */}
					<aside className="upcoming-side">
						<h4 className="upcoming-side-title">{t('koper_upcoming')}</h4>
						<ul className="upcoming-side-list">
							{upcoming.map((p, i) => (
								<li className="upcoming-side-item" key={p.id}>
									<img
										className="upcoming-side-thumb"
										src={p.imageUrl || '/pictures/kweker.png'}
										alt={p.name}
										onError={(e) => {
											(e.currentTarget as HTMLImageElement).src = '/pictures/kweker.png';
										}}
									/>
									<div className="upcoming-side-info">
										<div className="upcoming-side-name">{p.name}</div>
										<div className="upcoming-side-meta">
											{t('koper_upcoming_meta', {company: p.companyName, dimension: p.dimension ?? ''})}
										</div>
									</div>
									<span className="upcoming-side-badge">{t('koper_upcoming_badge')}</span>
								</li>
							))}
						</ul>
						<div className="price-history-block">
							<h4 className="upcoming-side-title">Laatste 10 prijzen (kweker)</h4>
							{historyLoading ? (
								<div className="text-gray-500">Laden...</div>
							) : kwekerHistory.length === 0 ? (
								<div className="text-gray-500">Geen data</div>
							) : (
								<ul className="upcoming-side-list">
									{kwekerHistory.map((item) => (
										<li className="upcoming-side-item" key={`${item.productId}-${item.purchasedAt}`}>
											<div className="upcoming-side-info">
												<div className="upcoming-side-name">{item.productName}</div>
												<div className="upcoming-side-meta">{new Date(item.purchasedAt).toLocaleDateString()}</div>
											</div>
											<span className="upcoming-side-badge">{formatEur(item.price)}</span>
										</li>
									))}
								</ul>
							)}
							<div className="text-gray-500">
								Gemiddelde prijs:{' '}
								{kwekerAverage ? formatEur(kwekerAverage.averagePrice) : '-'}
							</div>
						</div>
						<div className="price-history-block">
							<h4 className="upcoming-side-title">Laatste 10 prijzen (alle kwekers)</h4>
							{historyLoading ? (
								<div className="text-gray-500">Laden...</div>
							) : allHistory.length === 0 ? (
								<div className="text-gray-500">Geen data</div>
							) : (
								<ul className="upcoming-side-list">
									{allHistory.map((item) => (
										<li className="upcoming-side-item" key={`${item.productId}-${item.purchasedAt}-all`}>
											<div className="upcoming-side-info">
												<div className="upcoming-side-name">{item.kwekerName}</div>
												<div className="upcoming-side-meta">{formatEur(item.price)}</div>
											</div>
											<span className="upcoming-side-badge">{new Date(item.purchasedAt).toLocaleDateString()}</span>
										</li>
									))}
								</ul>
							)}
						</div>
					</aside>
				</div>
			</section>

			<footer className="app-footer">
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_about_title')}</h4>
					<p className="user-footer-line">{t('koper_footer_about_line1')}</p>
					<p className="user-footer-line">{t('koper_footer_about_line2')}</p>
				</div>
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_product_title')}</h4>
					<ul className="user-footer-list">
						<li>
							<a href="#">{t('koper_footer_live')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_history')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_favorites')}</a>
						</li>
					</ul>
				</div>
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_resources_title')}</h4>
					<ul className="user-footer-list">
						<li>
							<a href="#">{t('koper_footer_docs')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_faq')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_status')}</a>
						</li>
					</ul>
				</div>
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_contact_title')}</h4>
					<ul className="user-footer-list">
						<li>
							<a href="#">{t('koper_footer_support')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_form')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_locations')}</a>
						</li>
					</ul>
				</div>
			</footer>
		</Page>
	);
}

export default UserDashboard;



