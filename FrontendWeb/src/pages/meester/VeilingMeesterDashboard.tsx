import React, { useEffect, useState } from 'react';
import Page from '../../components/nav/Page';
import AuctionClock from '../../components/elements/AuctionClock';
import Button from '../../components/buttons/Button';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { formatEur } from '../../utils/standards';

import {
	createVeilingKlok,
	getProducts,
	updateVeilingKlokStatus,
	startVeilingProduct,
} from '../../controllers/server/veilingmeester';

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
		imageUrl: '',
		auctionedPrice: null,
		auctionedAt: null,
		dimension: '60 cm',
		stock: 150,
		companyName: 'Kwekerij Bloemenhof',
	},
	{
		id: '00000000-0000-0000-0000-000000000002',
		name: 'Witte Lelies',
		description: 'Verse witte lelies, grote knoppen',
		imageUrl: '',
		auctionedPrice: null,
		auctionedAt: null,
		dimension: '70 cm',
		stock: 80,
		companyName: 'Lelie Centrum BV',
	},
];

export default function VeilingmeesterDashboard() {
	const [tab, setTab] = useState<'veiling' | 'history'>('veiling');

	/* ---- Queue & veiling state ---- */
	const [queue, setQueue] = useState<ProductOutputDto[]>([]);
	const [isLoadingQueue, setIsLoadingQueue] = useState(false);
	const [queueError, setQueueError] = useState<string | null>(null);
	const [useDevQueue, setUseDevQueue] = useState(false);
	const [activeProduct, setActiveProduct] = useState<ProductOutputDto | null>(null);
	const [veilingState, setVeilingState] = useState<VeilingState>('none');

	/* ---- Clock state ---- */
	const [startPrice, setStartPrice] = useState(0);
	const [resetToken, setResetToken] = useState(0);

	/* ---- Persisted veiling ---- */
	const [currentVeiling, setCurrentVeiling] = useState<VeilingHistory | null>(null);
	const [history, setHistory] = useState<VeilingHistory[]>([]);

	useEffect(() => {
		let isActive = true;
		const loadQueue = async () => {
			setIsLoadingQueue(true);
			setQueueError(null);
			try {
				const response = await getProducts();
				if (!isActive) return;
				const data = response.data.data;
				if (data.length === 0 && process.env.NODE_ENV === 'development') {
					setQueue(DUMMY_QUEUE);
					setUseDevQueue(true);
				} else {
					setQueue(data);
					setUseDevQueue(false);
				}
			} catch (err) {
				console.error('Load products failed:', err);
				if (!isActive) return;
				setQueueError('Producten ophalen mislukt.');
				if (process.env.NODE_ENV === 'development') {
					setQueue(DUMMY_QUEUE);
					setUseDevQueue(true);
				}
			} finally {
				if (isActive) setIsLoadingQueue(false);
			}
		};
		loadQueue();
		return () => {
			isActive = false;
		};
	}, []);

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
				alert('Er zijn geen producten om te veilen.');
				return;
			}

			const productsMap: Record<string, number> = {};
			queue.forEach((p) => {
				productsMap[p.id] = p.auctionedPrice ?? 0;
			});

			const payload = {
				scheduledAt: new Date(Date.now() + 60_000).toISOString(),
				veilingDurationMinutes: 60,
				products: productsMap,
			};

			console.log('CreateVeiling payload:', payload);

			if (useDevQueue) {
				setCurrentVeiling({
					id: `dev-${Date.now()}`,
					startedAt: payload.scheduledAt,
					endedAt: null,
					products: [],
				});
				setActiveProduct(queue[0]);
				setVeilingState('open');
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
		} catch (err) {
			console.error('Open veiling failed:', err);
			alert('Openen van veiling mislukt (zie console)');
		}
	};

	/**
	 * START VEILING
	 * → Requires valid start price
	 */
	const startVeiling = async () => {
		if (!currentVeiling || !activeProduct) return;

		if (startPrice <= 0) {
			alert('Vul eerst een geldige minimumprijs in.');
			return;
		}

		if (useDevQueue) {
			setVeilingState('running');
			setResetToken((t) => t + 1);
			return;
		}

		await updateVeilingKlokStatus(currentVeiling.id, 'Started');
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
					buyerName: 'Demo koper',
					amount: activeProduct.stock,
					price: startPrice,
				},
			],
		};

		setCurrentVeiling((v) =>
			v ? { ...v, products: [...v.products, sold] } : v
		);

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
			if (!useDevQueue) {
				await updateVeilingKlokStatus(currentVeiling.id, 'Ended');
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
	};

	/* =====================================================
	   RENDER
	   ===================================================== */

	const displayProduct = activeProduct ?? queue[0] ?? null;
	const remainingProducts = displayProduct ? queue.filter((p) => p.id !== displayProduct.id) : queue;

	return (
		<Page enableHeader enableFooter>
			<main className="vm-container">
				<h1 className="vm-title">Veilingmeester</h1>

				<div className="vm-tabs">
					<button className={tab === 'veiling' ? 'active' : ''} onClick={() => setTab('veiling')}>
						Veiling
					</button>
					<button className={tab === 'history' ? 'active' : ''} onClick={() => setTab('history')}>
						History
					</button>
				</div>

				{/* ================= VEILING TAB ================= */}
				{tab === 'veiling' && (
					<section className="vm-board">
						<div className="vm-boardHeader">
							<div>
								<h2 className="vm-sectionTitle">Veiling</h2>
								<p className="vm-sectionSub">Beheer de veiling en volg de producten.</p>
							</div>
							{veilingState === 'none' && (
								<Button label="Open veiling" onClick={openVeiling} disabled={isLoadingQueue || queue.length == 0} />
							)}
							{veilingState !== 'none' && (
								<Button label="Eindig veiling" onClick={endVeiling} />
							)}
						</div>

						<div className="vm-currentRow">
							{displayProduct ? (
								<div className="vm-currentCard">
									<div className="vm-currentMedia">
										{displayProduct.imageUrl ? (
											<img
												className="vm-currentImg"
												src={displayProduct.imageUrl}
												alt={displayProduct.name}
											/>
										) : (
											<div className="vm-currentImg vm-currentImgPlaceholder" />
										)}
									</div>
									<div className="vm-currentBody">
										<div className="vm-currentHead">
											<div>
												<h3 className="vm-productName">{displayProduct.name}</h3>
												<p className="vm-productDesc">{displayProduct.description}</p>
											</div>
											<div className="vm-currentTags">
												<span className="vm-metaPill">{displayProduct.companyName}</span>
												{displayProduct.dimension && (
													<span className="vm-metaPill">{displayProduct.dimension}</span>
												)}
											</div>
										</div>
										<div className="vm-currentGrid">
											<div className="vm-kv">
												<span className="vm-kvLabel">Voorraad</span>
												<span className="vm-kvValue">{displayProduct.stock}</span>
											</div>
											<div className="vm-kv">
												<span className="vm-kvLabel">Maximumprijs (kweker)</span>
												<span className="vm-kvValue">
													{displayProduct.auctionedPrice != null
														? formatEur(displayProduct.auctionedPrice || 0)
														: 'Nog niet gezet'}
												</span>
											</div>
										</div>
									</div>
								</div>
							) : (
								<div className="vm-empty">Geen producten beschikbaar.</div>
							)}

							<div className="vm-clockCard">
								<div className={`vm-clockCircle ${veilingState === 'running' ? 'running' : 'idle'}`}>
									{veilingState === 'running' ? (
										<AuctionClock
											start
											maxPrice={displayProduct?.auctionedPrice ?? startPrice}
											minPrice={startPrice}
											resetToken={resetToken}
											onComplete={onAuctionComplete}
										/>
									) : (
										<div className="vm-clockPlaceholder">Klok</div>
									)}
								</div>
								{veilingState !== 'none' && (
									<div className="vm-clockControls">
										<label className="vm-label">Minimumprijs (veilingmeester)</label>
										<input
											className="vm-input"
											type="number"
											value={startPrice}
											onChange={(e) => setStartPrice(Number(e.target.value))}
										/>
										{veilingState === 'open' && (
											<Button label="Start veiling" onClick={startVeiling} />
										)}
									</div>
								)}
							</div>
						</div>

						<div className="vm-list">
							<h3 className="vm-sectionTitle">Overige producten</h3>
							<div className="vm-queueList">
								{isLoadingQueue && <div>Producten laden...</div>}
								{queueError && <div>{queueError}</div>}
								{!isLoadingQueue && !queueError && queue.length == 0 && (
									<div>Geen producten beschikbaar.</div>
								)}
								{!isLoadingQueue && !queueError && remainingProducts.map((p) => (
									<div key={p.id} className="vm-queueRow">
										<div>
											<div className="vm-queueName">{p.name}</div>
											<div className="vm-queueSub">{p.description}</div>
										</div>
										<div className="vm-queueMid">
											<div>{p.companyName}</div>
											{p.dimension && <div>{p.dimension}</div>}
											<div>Voorraad: {p.stock}</div>
										</div>
										<div className="vm-queueRight">
											{p.auctionedPrice != null ? formatEur(p.auctionedPrice) : 'Nog niet gezet'}
										</div>
									</div>
								))}
							</div>
						</div>
					</section>
				)}

				{/* ================= HISTORY TAB ================= */}

				{tab === 'history' && (
					<section className="vm-history">
						{history.map((v) => (
							<details key={v.id}>
								<summary>
									Veiling –{' '}
									{v.startedAt
										? new Date(v.startedAt).toLocaleString()
										: 'Onbekende datum'}
								</summary>

								{v.products.map((p) => (
									<div key={p.id}>
										<strong>{p.product.name}</strong> – {formatEur(p.finalPrice)}
									</div>
								))}
							</details>
						))}
					</section>
				)}
			</main>
		</Page>
	);
}
