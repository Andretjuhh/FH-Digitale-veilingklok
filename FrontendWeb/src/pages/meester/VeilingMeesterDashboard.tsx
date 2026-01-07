import React, { useState } from 'react';
import Page from '../../components/nav/Page';
import AuctionClock from '../../components/elements/AuctionClock';
import Button from '../../components/buttons/Button';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { formatEur } from '../../utils/standards';

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
	id: string;                 // VeilingKlok.Id
	startedAt: string;          // ScheduledAt / StartedAt
	endedAt: string | null;
	products: HistoryProduct[];
};

type VeilingState = 'none' | 'open' | 'running';

/* =========================================================
   DUMMY QUEUE (until backend product fetch is wired)
   ========================================================= */

const DUMMY_QUEUE: ProductOutputDto[] = [
	{
		id: '1',
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
		id: '2',
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

/* =========================================================
   API HELPERS (replace URLs if needed)
   ========================================================= */

async function apiCreateVeiling(
	products: ProductOutputDto[],
	startPrice: number
) {
	return fetch('/api/veilingklok', {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({
			scheduledAt: new Date().toISOString(),
			veilingDurationMinutes: 60,
			products: Object.fromEntries(
				products.map((p) => [p.id, startPrice])
			),
		}),
	}).then((r) => r.json());
}

async function apiUpdateVeilingStatus(
	veilingId: string,
	status: 'Started' | 'Ended'
) {
	return fetch(`/api/veilingklok/${veilingId}/status`, {
		method: 'PUT',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ status }),
	});
}

async function apiStartVeilingProduct(
	veilingId: string,
	productId: string
) {
	return fetch(`/api/veilingklok/${veilingId}/product/start`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ productId }),
	});
}

/* =========================================================
   COMPONENT
   ========================================================= */

export default function VeilingmeesterDashboard() {
	const [tab, setTab] = useState<'veiling' | 'history'>('veiling');

	/* ---- Veiling state ---- */
	const [queue, setQueue] = useState<ProductOutputDto[]>(DUMMY_QUEUE);
	const [activeProduct, setActiveProduct] = useState<ProductOutputDto | null>(null);
	const [veilingState, setVeilingState] = useState<VeilingState>('none');

	const [startPrice, setStartPrice] = useState(0);
	const [resetToken, setResetToken] = useState(0);

	/* ---- Persisted veiling ---- */
	const [currentVeiling, setCurrentVeiling] = useState<VeilingHistory | null>(null);
	const [history, setHistory] = useState<VeilingHistory[]>([]);

	/* =====================================================
	   ACTIONS
	   ===================================================== */

	/**
	 * OPEN VEILING
	 * Creates a VeilingKlok row in the database
	 */
	const openVeiling = async () => {
		if (queue.length === 0) return;

		// --- BACKEND CALL ---
		const klok = await apiCreateVeiling(queue, startPrice);

		// --- STORE VEILING LOCALLY ---
		setCurrentVeiling({
			id: klok.id,
			startedAt: klok.scheduledAt,
			endedAt: null,
			products: [],
		});

		setActiveProduct(klok.products[0] ?? queue[0]);
		setVeilingState('open');
	};

	/**
	 * START VEILING (clock starts running)
	 */
	const startVeiling = async () => {
		if (!currentVeiling || !activeProduct) return;

		await apiUpdateVeilingStatus(currentVeiling.id, 'Started');
		await apiStartVeilingProduct(currentVeiling.id, activeProduct.id);

		setVeilingState('running');
		setResetToken((t) => t + 1);
	};

	/**
	 * PRODUCT FINISHED
	 * Adds result to current veiling
	 */
	const onAuctionComplete = () => {
		if (!activeProduct || !currentVeiling) return;

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
	 * Marks VeilingKlok as ended and moves it to history
	 */
	const endVeiling = async () => {
		if (currentVeiling) {
			await apiUpdateVeilingStatus(currentVeiling.id, 'Ended');

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
					<>
						{veilingState === 'none' && (
							<section className="vm-noVeiling">
								<h2>Producten</h2>
								{queue.map((p) => (
									<div key={p.id}>{p.name}</div>
								))}
								<Button label="Open veiling" onClick={openVeiling} />
							</section>
						)}

						{veilingState !== 'none' && activeProduct && (
							<>
								<section className="vm-top">
									<div className="vm-productCard">
										<h2>{activeProduct.name}</h2>
										<p>{activeProduct.description}</p>
									</div>

									<div className="vm-controlCard">
										<label>Startprijs (€)</label>
										<input
											type="number"
											value={startPrice}
											onChange={(e) => setStartPrice(Number(e.target.value))}
										/>

										{veilingState === 'open' && (
											<Button label="Start veiling" onClick={startVeiling} />
										)}

										{veilingState === 'running' && (
											<AuctionClock
												start
												price={startPrice}
												resetToken={resetToken}
												onComplete={onAuctionComplete}
											/>
										)}

										<Button label="Eindig veiling" onClick={endVeiling} />
									</div>
								</section>
							</>
						)}
					</>
				)}

				{/* ================= HISTORY TAB ================= */}
				{tab === 'history' && (
					<section className="vm-history">
						{history.map((v) => (
							<details key={v.id}>
								<summary>
									Veiling – {new Date(v.startedAt).toLocaleString()}
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
