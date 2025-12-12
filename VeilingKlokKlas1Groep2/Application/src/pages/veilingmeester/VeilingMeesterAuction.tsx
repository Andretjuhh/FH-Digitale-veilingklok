// External imports
import React, { useMemo, useState } from 'react';
import { useLocation, useNavigate, useParams } from 'react-router-dom';

// Internal imports
import Page from '../../components/nav/Page';
import AuctionClock from '../../components/elements/AuctionClock';

type Product = {
	id: number;
	name: string;
	quantity: number;
	startingPrice: number;
	minAmount?: number;
};

type Auction = {
	id: number;
	title: string;
	date: string;
	products: Product[];
};

function VeilingMeesterAuction() {
	const navigate = useNavigate();
	const params = useParams();
	const location = useLocation();
	const navAuction = (location.state as { auction?: Auction } | undefined)?.auction;

	const fallbackAuction = useMemo<Auction>(
		() => ({
			id: Number(params.id) || 0,
			title: 'Actieve veiling',
			date: 'Vandaag',
			products: [
				{ id: 1, name: 'Tulpen mix', quantity: 120, startingPrice: 3.8, minAmount: 1 },
				{ id: 2, name: 'Rozen rood', quantity: 80, startingPrice: 4.5, minAmount: 1 },
				{ id: 3, name: 'Zonnebloemen', quantity: 60, startingPrice: 2.9, minAmount: 1 },
			],
		}),
		[params.id]
	);

	const auction = navAuction ?? fallbackAuction;

	const [products, setProducts] = useState<Product[]>(
		auction.products.map((p) => ({ ...p }))
	);
	const [activeProductId, setActiveProductId] = useState<number | null>(null);
	const [clockToken, setClockToken] = useState(0);
	const [clockPaused, setClockPaused] = useState(false);

	const activeProduct = products.find((p) => p.id === activeProductId) ?? null;

	const handlePriceChange = (productId: number, value: number) => {
		setProducts((prev) =>
			prev.map((product) =>
				product.id === productId ? { ...product, startingPrice: value } : product
			)
		);
	};

	const handleStartProduct = (productId: number) => {
		setActiveProductId(productId);
		setClockPaused(false);
		setClockToken((token) => token + 1);
	};

	const handlePauseResume = () => setClockPaused((state) => !state);

	const handleComplete = () => {
		const currentIndex = products.findIndex((p) => p.id === activeProductId);
		const nextProduct = products[currentIndex + 1];
		if (nextProduct) {
			setActiveProductId(nextProduct.id);
			setClockToken((token) => token + 1);
			setClockPaused(false);
		} else {
			setActiveProductId(null);
			setClockPaused(false);
		}
	};

	return (
		<Page enableHeader enableFooter className="veilingmeester-page">
			<main className="veilingmeester-container space-y-8">
				<div className="flex items-start justify-between gap-4">
					<div>
						<h1 className="text-2xl font-semibold">{auction.title}</h1>
						<p className="text-gray-600">{auction.date}</p>
					</div>
					<button
						onClick={() => navigate(-1)}
						className="px-4 py-2 bg-gray-200 rounded-lg hover:bg-gray-300"
					>
						Terug
					</button>
				</div>

				<section className="grid gap-6 lg:grid-cols-[minmax(320px,380px),1fr]">
					<div className="bg-white border rounded-xl p-4 shadow-sm">
						<h2 className="text-lg font-semibold mb-3">Veilingklok</h2>
						{activeProduct ? (
							<div className="space-y-3">
								<div>
									<p className="text-sm text-gray-600">Actief product</p>
									<p className="font-semibold">{activeProduct.name}</p>
									<p className="text-gray-700">
										Startprijs: ƒ {activeProduct.startingPrice.toFixed(2)}
									</p>
									<p className="text-gray-700">
										Aantal: {activeProduct.quantity} · Min. aant.: {activeProduct.minAmount ?? 1}
									</p>
								</div>
								<AuctionClock
									start={Boolean(activeProduct)}
									paused={clockPaused}
									resetToken={clockToken}
									price={activeProduct.startingPrice}
									amountPerLot={activeProduct.quantity}
									minAmount={activeProduct.minAmount ?? 1}
									onComplete={handleComplete}
								/>
								<div className="flex gap-3">
									<button
										onClick={handlePauseResume}
										className="px-4 py-2 bg-gray-200 rounded-lg hover:bg-gray-300 w-full"
									>
										{clockPaused ? 'Hervat klok' : 'Pauzeer klok'}
									</button>
								</div>
							</div>
						) : (
							<div className="text-gray-600">
								Kies een product in de lijst om de klok te starten.
							</div>
						)}
					</div>

					<div className="bg-white border rounded-xl p-4 shadow-sm">
						<h2 className="text-lg font-semibold mb-3">Producten in deze veiling</h2>
						<div className="space-y-3">
							{products.map((product) => {
								const isActive = product.id === activeProductId;
								return (
									<div
										key={product.id}
										className={`border rounded-lg p-3 ${isActive ? 'border-blue-500 bg-blue-50' : 'border-gray-200'}`}
									>
										<div className="flex flex-col md:flex-row md:items-center gap-3">
											<div className="flex-1">
												<p className="font-semibold">{product.name}</p>
												<p className="text-gray-600">
													Aantal: {product.quantity} · Min. aant.: {product.minAmount ?? 1}
												</p>
											</div>
											<div className="flex items-center gap-2">
												<label className="text-sm text-gray-600" htmlFor={`start-${product.id}`}>
													Startprijs
												</label>
												<input
													id={`start-${product.id}`}
													type="number"
													step="0.1"
													min={0}
													className="w-24 border rounded px-2 py-1"
													value={product.startingPrice}
													onChange={(e) => handlePriceChange(product.id, Number(e.target.value))}
												/>
											</div>
											<button
												onClick={() => handleStartProduct(product.id)}
												className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
											>
												Start klok
											</button>
										</div>
									</div>
								);
							})}
						</div>
					</div>
				</section>
			</main>
		</Page>
	);
}

export default VeilingMeesterAuction;
