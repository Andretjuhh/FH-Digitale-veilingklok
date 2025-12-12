// External imports
import React, { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';

// Internal imports
import Page from '../../components/nav/Page';
import AuctionClock from '../../components/elements/AuctionClock';

type Auction = {
	id: number;
	title: string;
	date: string;
	lots: number;
	products: {
		id: number;
		name: string;
		quantity: number;
		startingPrice: number;
		minAmount?: number;
	}[];
};

function VeilingMeesterDashboard() {
	const navigate = useNavigate();

	const pastAuctions: Auction[] = useMemo(
		() => [
			{ id: 9001, title: 'Najaar veiling - 10 dec', date: '10 dec 2025', lots: 14, products: [] },
			{ id: 9002, title: 'Winter veiling - 2 dec', date: '2 dec 2025', lots: 11, products: [] },
		],
		[]
	);

	const activeAuctions: Auction[] = useMemo(
		() => [
			{
				id: 2001,
				title: 'Voorjaarsbloemen',
				date: 'Vandaag, 14:00',
				lots: 6,
				products: [
					{ id: 1, name: 'Tulpen mix', quantity: 120, startingPrice: 3.8, minAmount: 1 },
					{ id: 2, name: 'Rozen rood', quantity: 80, startingPrice: 4.5, minAmount: 1 },
					{ id: 3, name: 'Zonnebloemen', quantity: 60, startingPrice: 2.9, minAmount: 1 },
				],
			},
			{
				id: 2002,
				title: 'Planten specials',
				date: 'Vandaag, 16:00',
				lots: 4,
				products: [
					{ id: 4, name: 'Kamerplant XL', quantity: 25, startingPrice: 9.9, minAmount: 1 },
					{ id: 5, name: 'Vetplant set', quantity: 70, startingPrice: 2.1, minAmount: 1 },
				],
			},
		],
		[]
	);

	const [currentAuction, setCurrentAuction] = useState<Auction | null>(null);
	const [clockToken, setClockToken] = useState(0);

	const handleStartAuction = (auction: Auction) => {
		setCurrentAuction(auction);
		setClockToken((token) => token + 1);
		navigate(`/veilingmeester/auction/${auction.id}`, { state: { auction } });
	};

	return (
		<Page enableHeader enableFooter className="veilingmeester-page">
			<main className="veilingmeester-container space-y-10">
				{/* Titel */}
				<section className="veilingmeester-hallo">
					<h1>Hallo, veilingmeester!</h1>
					<p className="veilingmeester-desc">
						Selecteer een veiling om te starten. Producten worden pas zichtbaar op de klokpagina.
					</p>
				</section>

				{/* Huidige veiling */}
				<section className="bg-gray-100 p-6 rounded-xl shadow">
					<div className="flex items-start justify-between gap-4 mb-4">
						<div>
							<h2 className="text-xl font-semibold">Huidige veiling</h2>
							<p className="text-gray-600">
								Wordt zichtbaar zodra je een actieve veiling start.
							</p>
						</div>
						{currentAuction && (
							<button
								className="text-blue-700 underline"
								onClick={() =>
									navigate(`/veilingmeester/auction/${currentAuction.id}`, {
										state: { auction: currentAuction },
									})
								}
							>
								Open veiling
							</button>
						)}
					</div>

					{currentAuction ? (
						<div className="grid gap-6 lg:grid-cols-[1fr,320px]">
							<div className="border rounded-lg bg-white p-4">
								<h3 className="font-semibold text-lg">{currentAuction.title}</h3>
								<p className="text-gray-600">{currentAuction.date}</p>
								<p className="text-gray-700 mt-2">Lots: {currentAuction.lots}</p>
								<p className="text-sm text-gray-500 mt-1">
									Producten en startprijzen stel je in op de klokpagina.
								</p>
							</div>
							<div className="bg-white border rounded-lg p-4">
								<AuctionClock
									start
									resetToken={clockToken}
									price={currentAuction.products[0]?.startingPrice ?? 0}
									amountPerLot={currentAuction.products[0]?.quantity ?? 0}
									minAmount={currentAuction.products[0]?.minAmount ?? 1}
								/>
							</div>
						</div>
					) : (
						<div className="border border-dashed rounded-lg p-6 bg-white text-gray-600">
							Nog geen veiling gestart. Kies er één bij &quot;Actieve veilingen&quot; en druk op
							&quot;Start veiling&quot;.
						</div>
					)}
				</section>

				{/* Actieve veilingen */}
				<section className="bg-gray-100 p-6 rounded-xl shadow">
					<h2 className="text-xl font-semibold mb-4">Actieve veilingen</h2>
					<div className="space-y-3">
						{activeAuctions.map((auction) => (
							<div
								key={auction.id}
								className="flex flex-col md:flex-row md:items-center gap-4 border rounded-lg bg-white p-4"
							>
								<div className="flex-1">
									<h3 className="font-semibold text-lg">{auction.title}</h3>
									<p className="text-gray-700">{auction.date}</p>
									<p className="text-gray-600">Lots: {auction.lots}</p>
								</div>
								<div className="flex gap-3">
									<button
										onClick={() => navigate(`/veilingmeester/auction/${auction.id}`, { state: { auction } })}
										className="px-4 py-2 bg-gray-200 rounded-lg hover:bg-gray-300"
									>
										Bezoek
									</button>
									<button
										onClick={() => handleStartAuction(auction)}
										className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
									>
										Start veiling
									</button>
								</div>
							</div>
						))}
					</div>
				</section>

				{/* Eerdere veilingen */}
				<section className="bg-gray-100 p-6 rounded-xl shadow">
					<h2 className="text-xl font-semibold mb-4">Eerdere veilingen</h2>
					<div className="grid gap-3 md:grid-cols-2">
						{pastAuctions.map((auction) => (
							<div key={auction.id} className="border rounded-lg bg-white p-4">
								<h3 className="font-semibold">{auction.title}</h3>
								<p className="text-gray-600">{auction.date}</p>
								<p className="text-gray-600">Lots verkocht: {auction.lots}</p>
							</div>
						))}
					</div>
				</section>
			</main>
		</Page>
	);
}

export default VeilingMeesterDashboard;
