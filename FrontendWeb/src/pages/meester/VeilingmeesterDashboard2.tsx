import React, { useEffect, useState } from 'react';
import Page from '../../components/nav/Page';
import AuctionClock from '../../components/elements/AuctionClock2';
import Button from '../../components/buttons/Button';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { formatEur } from '../../utils/standards';

import { createDevVeilingKlok, createVeilingKlok, getProducts, startVeilingProduct, updateVeilingKlokStatus } from '../../controllers/server/veilingmeester';

/* =========================================================
   TYPES
   ========================================================= */

type HistoryBuyerLine = {
	buyerName: string;
	amount: number;
	price: number;
};

type HistoryProduct = {
	id: string;
	product: ProductOutputDto;
	startPrice: number;
	finalPrice: number;
	auctionedAt: string;
	lines: HistoryBuyerLine[];
};

type VeilingHistory = {
	id: string;
	startedAt: string | null;
	endedAt: string | null;
	products: HistoryProduct[];
};

type VeilingState = 'none' | 'open' | 'running';

/* =========================================================
   COMPONENT
   ========================================================= */

const DUMMY_QUEUE: ProductOutputDto[] = [
	{
		id: '00000000-0000-0000-0000-000000000001',
		name: 'Rode Rozen Premium',
		description: 'Dieprode rozen van topkwaliteit',
		imageUrl: 'https://images.unsplash.com/photo-1509042239860-f550ce710b93',
		auctionedPrice: 125,
		minimumPrice: null,
		auctionedAt: null,
		dimension: '60 cm',
		stock: 150,
		companyName: 'Kwekerij Bloemenhof',
		auctionPlanned: true,
	},
	{
		id: '00000000-0000-0000-0000-000000000002',
		name: 'Witte Lelies',
		description: 'Verse witte lelies, grote knoppen',
		imageUrl: 'https://images.unsplash.com/photo-1501004318641-b39e6451bec6',
		minimumPrice: null,
		auctionedPrice: 90,
		auctionedAt: null,
		dimension: '70 cm',
		stock: 80,
		companyName: 'Lelie Centrum BV',
		auctionPlanned: true,
	},
	{
		id: '00000000-0000-0000-0000-000000000003',
		name: 'Zonnebloem XL',
		description: 'Grote zonnebloemen met stevige stelen',
		imageUrl: 'https://images.unsplash.com/photo-1498654896293-37aacf113fd9',
		auctionedPrice: 65,
		minimumPrice: null,
		auctionedAt: null,
		dimension: '90 cm',
		stock: 120,
		companyName: 'Zon & Co',
		auctionPlanned: true,
	},
	{
		id: '00000000-0000-0000-0000-000000000004',
		name: 'Tulpen Mix',
		minimumPrice: null,
		description: 'Mix van voorjaarskleuren, premium selectie',
		imageUrl: 'https://images.unsplash.com/photo-1508747703725-719777637510',
		auctionedPrice: 55,
		auctionedAt: null,
		dimension: '40 cm',
		stock: 200,
		companyName: 'Tulipa Holland',
		auctionPlanned: true,
	},
	{
		id: '00000000-0000-0000-0000-000000000005',
		name: 'Orchidee Phalaenopsis',
		description: 'Bloeiend en rijk vertakt',
		imageUrl: 'https://images.unsplash.com/photo-1501004318641-b39e6451bec6',
		minimumPrice: null,
		auctionedPrice: 160,
		auctionedAt: null,
		dimension: '55 cm',
		stock: 60,
		companyName: 'Orchid World',
		auctionPlanned: true,
	},
];
const DEV_DEFAULT_MIN_RATIO = 0.6;

export default function VeilingmeesterDashboard() {
	const { t } = useTranslation();

	const [tab, setTab] = useState<'veiling' | 'history'>('veiling');

	/* ---- Queue & veiling state ---- */
	const [queue, setQueue] = useState<ProductOutputDto[]>([]);
	const [isLoadingQueue, setIsLoadingQueue] = useState(false);
	const [queueError, setQueueError] = useState<string | null>(null);
	const [activeProduct, setActiveProduct] = useState<ProductOutputDto | null>(null);
	const [veilingState, setVeilingState] = useState<VeilingState>('none');
	const [hasVeilingStarted, setHasVeilingStarted] = useState(false);
	const [durationSeconds, setDurationSeconds] = useState(60);

	/* ---- Clock state ---- */
	const [startPrice, setStartPrice] = useState(0);
	const [resetToken, setResetToken] = useState(0);

	/* ---- Persisted veiling ---- */
	const [currentVeiling, setCurrentVeiling] = useState<VeilingHistory | null>(null);
	const [history, setHistory] = useState<VeilingHistory[]>([]);
	const [isLoadingHistory, setIsLoadingHistory] = useState(false);
	const [historyError, setHistoryError] = useState<string | null>(null);

	useEffect(() => {
		let isActive = true;
		const loadQueue = async () => {
			setIsLoadingQueue(true);
			setQueueError(null);
			try {
				const response = await getProducts();
				if (!isActive) return;
				const data = response.data.data;
				setQueue(data);
			} catch (err) {
				console.error('Load products failed:', err);
				if (!isActive) return;
				setQueueError(t('vm_queue_load_error'));
			} finally {
				if (isActive) setIsLoadingQueue(false);
			}
		};
		loadQueue();
		return () => {
			isActive = false;
		};
	}, [t]);

	const mapVeilingKlokToHistory = (klok: VeilingKlokOutputDto): VeilingHistory => {
		const products: HistoryProduct[] = klok.products.map((p) => {
			const price = p.auctionedPrice ?? 0;
			return {
				id: `hp-${klok.id}-${p.id}`,
				product: p,
				startPrice: price,
				finalPrice: price,
				auctionedAt: p.auctionedAt ?? klok.endedAt ?? klok.startedAt ?? klok.scheduledAt ?? klok.createdAt,
				lines: [
					{
						buyerName: p.companyName,
						amount: p.stock,
						price,
					},
				],
			};
		});

		return {
			id: klok.id,
			startedAt: klok.startedAt ?? klok.scheduledAt ?? klok.createdAt ?? null,
			endedAt: klok.endedAt ?? null,
			products,
		};
	};

	const loadHistory = useCallback(async () => {
		setIsLoadingHistory(true);
		setHistoryError(null);
		try {
			const response = await getVeilingKlokken();
			const items: VeilingHistory[] = response.data.map(mapVeilingKlokToHistory);
			items.sort((a: VeilingHistory, b: VeilingHistory) => {
				const aTime = a.startedAt ? Date.parse(a.startedAt) : a.endedAt ? Date.parse(a.endedAt) : 0;
				const bTime = b.startedAt ? Date.parse(b.startedAt) : b.endedAt ? Date.parse(b.endedAt) : 0;
				return bTime - aTime;
			});
			setHistory(items);
		} catch (err) {
			console.error('Load veiling history failed:', err);
			setHistoryError(t('vm_history_load_error'));
		} finally {
			setIsLoadingHistory(false);
		}
	}, [t]);

	useEffect(() => {
		if (tab !== 'history') return;
		void loadHistory();
	}, [tab, loadHistory]);

	/* =====================================================
	   ACTIONS
	   ===================================================== */

	/**
	 * OPEN VEILING
	 * → Creates VeilingKlok in DB
	 * → NO start price required here
	 */
	const openVeiling = async () => {
		try {
			if (queue.length === 0) {
				alert(t('vm_alert_no_products'));
				return;
			}

			const productsMap: Record<string, number> = {};
			queue.forEach((p) => {
				productsMap[p.id] = p.auctionedPrice ?? 0;
			});

			const payload = {
				scheduledAt: new Date(Date.now() + 5 * 60_000).toISOString(),
				veilingDurationSeconds: Math.max(1, Math.ceil(durationSeconds / 60)),
				products: productsMap,
			};

			console.log('CreateVeiling payload:', payload);

			if (useDevQueue) {
				try {
					const response = await createDevVeilingKlok({
						scheduledAt: payload.scheduledAt,
						veilingDurationSeconds: payload.veilingDurationSeconds,
						products: queue.map((p) => ({
							id: p.id,
							name: p.name,
							description: p.description,
							imageUrl: p.imageUrl,
							dimension: p.dimension ?? null,
							stock: p.stock,
							companyName: p.companyName,
							maxPrice: p.auctionedPrice ?? 0,
						})),
					});

					const klok = response.data;
					setCurrentVeiling({
						id: klok.id,
						startedAt: klok.scheduledAt,
						endedAt: null,
						products: [],
					});
					setPersistedDevVeiling(true);
				} catch (err) {
					console.error('Persist dev veiling failed:', err);
					setCurrentVeiling({
						id: `dev-${Date.now()}`,
						startedAt: payload.scheduledAt,
						endedAt: null,
						products: [],
					});
					setPersistedDevVeiling(false);
				}

				setActiveProduct(queue[0]);
				setVeilingState('open');
				setHasVeilingStarted(false);
				return;
			}

			const response = await createVeilingKlok(payload);
			const klok = response.data;

			setCurrentVeiling({
				id: klok.id,
				startedAt: klok.scheduledAt,
				endedAt: null,
				products: [],
			});

			setActiveProduct(queue[0]);
			setVeilingState('open');
			setHasVeilingStarted(false);
		} catch (err) {
			console.error('Open veiling failed:', err);
			alert(t('vm_alert_open_failed'));
		}
	};

	/**
	 * START VEILING
	 * → Requires valid start price
	 */
	const startVeiling = async () => {
		if (!currentVeiling || !activeProduct) return;

		if (startPrice <= 0) {
			alert(t('vm_alert_min_price_invalid'));
			return;
		}

		if (!hasVeilingStarted) {
			await updateVeilingKlokStatus(currentVeiling.id, 'Started' as any);
			setHasVeilingStarted(true);
		}
		await startVeilingProduct(currentVeiling.id, activeProduct.id);

		setVeilingState('running');
		setResetToken((t) => t + 1);
	};

	/**
	 * PRODUCT FINISHED
	 */
	const onAuctionComplete = () => {
		if (!currentVeiling || !activeProduct) return;

		const sold: HistoryProduct = {
			id: `hp-${activeProduct.id}-${Date.now()}`,
			product: {
				...activeProduct,
				auctionedPrice: startPrice,
				auctionedAt: new Date().toISOString(),
			},
			startPrice,
			finalPrice: startPrice,
			auctionedAt: new Date().toISOString(),
			lines: [
				{
					buyerName: t('vm_demo_buyer'),
					amount: activeProduct.stock,
					price: startPrice,
				},
			],
		};

		setCurrentVeiling((v) => (v ? { ...v, products: [...v.products, sold] } : v));

		const remaining = queue.filter((p) => p.id !== activeProduct.id);
		setQueue(remaining);

		if (remaining.length > 0) {
			setActiveProduct(remaining[0]);
			setVeilingState('open');
			setStartPrice(0);
		} else {
			endVeiling();
		}
	};

	/**
	 * END VEILING
	 */
	const endVeiling = async () => {
		if (currentVeiling) {
			if (!useDevQueue || persistedDevVeiling) {
				await updateVeilingKlokStatus(currentVeiling.id, 'Ended' as any);
			}

			setHistory((h) => [
				{
					...currentVeiling,
					endedAt: new Date().toISOString(),
				},
				...h,
			]);
		}

		setCurrentVeiling(null);
		setActiveProduct(null);
		setVeilingState('none');
		setStartPrice(0);
		setHasVeilingStarted(false);
	};

	/* =====================================================
	   RENDER
	   ===================================================== */

	const displayProduct = activeProduct ?? queue[0] ?? null;
	const remainingProducts = displayProduct ? queue.filter((p) => p.id !== displayProduct.id) : queue;

	return (
		<Page enableHeader enableFooter>
			<main className="vm-page-ctn">
				<h1 className="vm-title">Veilingmeester</h1>

				<div className="vm-tabs">
					<button className={`vm-tab ${tab === 'veiling' ? 'active' : ''}`} aria-label={t('vm_tab_auction_aria')} onClick={() => setTab('veiling')}>
						{t('vm_tab_auction')}
					</button>
					<button
						className={`vm-tab ${tab === 'history' ? 'active' : ''}`}
						aria-label={t('vm_tab_history_aria')}
						onClick={() => {
							setTab('history');
							void loadHistory();
						}}
					>
						{t('vm_tab_history')}
					</button>
				</div>

				{/* ================= VEILING TAB ================= */}
				{tab === 'veiling' && (
					<section className="vm-board">
						<div className="vm-boardHeader">
							<div>
								<h2 className="vm-sectionTitle">{t('vm_tab_auction')}</h2>
								<p className="vm-sectionSub">{t('vm_section_subtitle')}</p>
							</div>
							{veilingState === 'none' && <Button label="Open veiling" onClick={openVeiling} disabled={isLoadingQueue || queue.length == 0} />}
							{veilingState !== 'none' && <Button label="Eindig veiling" onClick={endVeiling} />}
						</div>

						<div className="vm-currentRow">
							{displayProduct ? (
								<div className="vm-currentCard">
									<div className="vm-currentMedia">{displayProduct.imageUrl ? <img className="vm-currentImg" src={displayProduct.imageUrl} alt={displayProduct.name} /> : <div className="vm-currentImg vm-currentImgPlaceholder" />}</div>
									<div className="vm-currentBody">
										<div className="vm-currentHead">
											<div>
												<h3 className="vm-productName">{displayProduct.name}</h3>
												<p className="vm-productDesc">{displayProduct.description}</p>
											</div>
											<div className="vm-currentTags">
												<span className="vm-metaPill">{displayProduct.companyName}</span>
												{displayProduct.dimension && <span className="vm-metaPill">{displayProduct.dimension}</span>}
											</div>
										</div>
										<div className="vm-currentGrid">
											<div className="vm-kv">
												<span className="vm-kvLabel">{t('vm_stock_label')}</span>
												<span className="vm-kvValue">{displayProduct.stock}</span>
											</div>
											<div className="vm-kv">
												<span className="vm-kvLabel">{t('vm_max_price_label')}</span>
												<span className="vm-kvValue">{displayProduct.auctionedPrice != null ? formatEur(displayProduct.auctionedPrice || 0) : t('vm_price_not_set')}</span>
											</div>
										</div>
									</div>
								</div>
							) : (
								<div className="vm-empty">{t('vm_empty_no_products')}</div>
							)}

							<div className="vm-clockCard">
								<div className={`vm-clockCircle ${veilingState === 'running' ? 'running' : 'idle'}`}>
									{veilingState === 'running' ? <AuctionClock start totalSeconds={durationSeconds} maxPrice={displayProduct?.auctionedPrice ?? startPrice} minPrice={startPrice} resetToken={resetToken} onComplete={onAuctionComplete} /> : <div className="vm-clockPlaceholder">{t('vm_clock_placeholder')}</div>}
								</div>
								{veilingState !== 'none' && (
									<div className="vm-clockControls">
										<label className="vm-label" htmlFor="vm-duration-seconds">
											{t('vm_label_duration_seconds')}
										</label>
										<input className="vm-input" id="vm-duration-seconds" type="number" min={1} value={durationSeconds} aria-label={t('vm_input_duration_aria')} onChange={(e) => setDurationSeconds(Number(e.target.value))} />
										<label className="vm-label" htmlFor="vm-min-price">
											{t('vm_label_min_price')}
										</label>
										<input className="vm-input" id="vm-min-price" type="number" value={startPrice} aria-label={t('vm_input_min_price_aria')} onChange={(e) => setStartPrice(Number(e.target.value))} />
										{veilingState === 'open' && <Button label="Start veiling" onClick={startVeiling} />}
									</div>
								)}
							</div>
						</div>

						<div className="vm-list">
							<h3 className="vm-sectionTitle">{t('vm_section_other_products')}</h3>
							<div className="vm-queueList">
								{isLoadingQueue && <div>{t('vm_loading_products')}</div>}
								{queueError && <div>{queueError}</div>}
								{!isLoadingQueue && !queueError && queue.length == 0 && <div>{t('vm_empty_no_products')}</div>}
								{!isLoadingQueue &&
									!queueError &&
									remainingProducts.map((p) => (
										<div key={p.id} className="vm-queueRow">
											<div>
												<div className="vm-queueName">{p.name}</div>
												<div className="vm-queueSub">{p.description}</div>
											</div>
											<div className="vm-queueMid">
												<div>{p.companyName}</div>
												{p.dimension && <div>{p.dimension}</div>}
												<div>{t('vm_stock_value', { count: p.stock })}</div>
											</div>
											<div className="vm-queueRight">{p.auctionedPrice != null ? formatEur(p.auctionedPrice) : t('vm_price_not_set')}</div>
										</div>
									))}
							</div>
						</div>
					</section>
				)}

				{/* ================= HISTORY TAB ================= */}

				{tab === 'history' && (
					<section className="vm-history">
						<div className="vm-historyList">
							{isLoadingHistory && <div className="vm-empty">{t('vm_history_loading')}</div>}
							{historyError && <div className="vm-empty">{historyError}</div>}
							{!isLoadingHistory && !historyError && history.length === 0 && <div className="vm-empty">{t('vm_history_empty')}</div>}
							{history.map((v) => {
								const startedLabel = v.startedAt ? new Date(v.startedAt).toLocaleString() : t('vm_history_unknown_date');
								const endedLabel = v.endedAt ? new Date(v.endedAt).toLocaleString() : '-';
								const totalRevenue = v.products.reduce((sum, p) => sum + p.finalPrice, 0);

								return (
									<details key={v.id} className="vm-historyItem">
										<summary className="vm-historySummary" aria-label={t('vm_history_summary_aria', { date: startedLabel })}>
											<div>
												<div className="vm-historyName">{t('vm_history_auction_title', { date: startedLabel })}</div>
												<div className="vm-historySub">{t('vm_history_products_count', { count: v.products.length })}</div>
											</div>
											<div>
												<div className="vm-historyPriceLabel">{t('vm_history_total_revenue_label')}</div>
												<div className="vm-historyPrice">{formatEur(totalRevenue)}</div>
											</div>
										</summary>

										<div className="vm-historyDetails">
											<div className="vm-historyGrid">
												<div className="vm-historyBox">
													<div className="vm-historyBoxLabel">{t('vm_history_start_label')}</div>
													<div className="vm-historyBoxValue">{startedLabel}</div>
												</div>
												<div className="vm-historyBox">
													<div className="vm-historyBoxLabel">{t('vm_history_end_label')}</div>
													<div className="vm-historyBoxValue">{endedLabel}</div>
												</div>
												<div className="vm-historyBox">
													<div className="vm-historyBoxLabel">{t('vm_history_products_label')}</div>
													<div className="vm-historyBoxValue">{v.products.length}</div>
												</div>
												<div className="vm-historyBox">
													<div className="vm-historyBoxLabel">{t('vm_history_revenue_label')}</div>
													<div className="vm-historyBoxValue">{formatEur(totalRevenue)}</div>
												</div>
											</div>

											<div className="vm-linesTitle">{t('vm_history_sold_products_title')}</div>
											<div className="vm-lines">
												{v.products.length === 0 && <div className="vm-empty">{t('vm_empty_no_products')}</div>}
												{v.products.map((p) => {
													const buyer = p.lines[0]?.buyerName ?? t('vm_history_unknown_buyer');
													const amount = p.lines[0]?.amount ?? 0;
													return (
														<div key={p.id} className="vm-lineRow">
															<div>
																<div className="vm-lineBuyer">{p.product.name}</div>
																<div className="vm-lineMeta">
																	{p.product.description} • {t('vm_history_piece_count', { count: amount })}
																</div>
																<div className="vm-lineMeta">
																	{buyer} • {t('vm_stock_value', { count: p.product.stock })}
																</div>
															</div>
															<div className="vm-lineMeta">{p.product.companyName}</div>
															<div className="vm-lineTotal">{formatEur(p.finalPrice)}</div>
														</div>
													);
												})}
											</div>
										</div>
									</details>
								);
							})}
						</div>
					</section>
				)}
			</main>
		</Page>
	);
}
