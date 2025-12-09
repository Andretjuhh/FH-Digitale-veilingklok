// External imports
import React, { useState, useEffect } from 'react';

// Internal imports
import Page from '../../components/nav/Page';

function VeilingMeesterDashboard() {
	// Dummy data — vervang later door backend data
	const [items, setItems] = useState([
		{ id: 1, name: "Appelmand", amount: 12, price: 3.5, image: "", duration: 30 },
		{ id: 2, name: "Perenkrat", amount: 8, price: 4.2, image: "", duration: 30 },
		{ id: 3, name: "Bananendoos", amount: 20, price: 2.8, image: "", duration: 30 },
	]);

	const [currentIndex, setCurrentIndex] = useState(0);
	const [timeLeft, setTimeLeft] = useState(items[0].duration);
	const [auctionRunning, setAuctionRunning] = useState(false);

	// Timer logic
	useEffect(() => {
		if (!auctionRunning) return;
		if (timeLeft <= 0) {
			// Move to next product
			setCurrentIndex((prev) => Math.min(prev + 1, items.length - 1));
			setTimeLeft(items[currentIndex + 1]?.duration || 0);
			setAuctionRunning(false);
			return;
		}
		const timer = setTimeout(() => setTimeLeft(timeLeft - 1), 1000);
		return () => clearTimeout(timer);
	}, [auctionRunning, timeLeft, currentIndex, items]);

	const currentItem = items[currentIndex];

	return (
		<Page enableHeader enableFooter className="veilingmeester-page">
			<main className="veilingmeester-container">

				{/* Titel */}
				<section className="veilingmeester-hallo">
					<h1>Hallo, veilingmeester!</h1>
					<p className="veilingmeester-desc">
						Welkom op de dashboard pagina! Hier kan je de veiling starten en bekijken.
					</p>
				</section>

				{/* ===================== ACTIEF VEILING PRODUCT ===================== */}
				<section className="bg-gray-100 p-6 rounded-xl shadow mb-12">
					<h2 className="text-xl font-semibold mb-4">Huidige veiling</h2>

					<div className="flex items-center gap-4 border rounded-lg bg-white p-4">
						<div className="w-24 h-24 bg-gray-300 rounded-xl" />

						<div className="flex-1">
							<h3 className="font-semibold text-lg">{currentItem.name}</h3>
							<p className="text-gray-700">Aantal: {currentItem.amount}</p>
							<p className="text-gray-700">Prijs: €{currentItem.price.toFixed(2)}</p>
							<p className="mt-2 font-semibold">
								Tijd resterend: {timeLeft}s
							</p>
						</div>

						<button
							onClick={() => setAuctionRunning(!auctionRunning)}
							className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
						>
							{auctionRunning ? "Stop" : "Start"}
						</button>
					</div>
				</section>

				{/* ===================== KOMENDE PRODUCTEN ===================== */}
				<section className="bg-gray-100 p-6 rounded-xl shadow">
					<h2 className="text-xl font-semibold mb-4">Komende producten</h2>

					<div className="space-y-3">
						{items.slice(currentIndex + 1).map((item) => (
							<div
								key={item.id}
								className="flex items-center gap-4 border rounded-lg bg-white p-4"
							>
								<div className="w-20 h-20 bg-gray-300 rounded-xl" />

								<div className="flex-1">
									<h3 className="font-semibold">{item.name}</h3>
									<p className="text-gray-700">Aantal: {item.amount}</p>
									<p className="text-gray-700">Prijs: €{item.price.toFixed(2)}</p>
								</div>

								<button
									className="px-4 py-2 bg-gray-200 rounded-lg cursor-default"
									disabled
								>
									Wachten
								</button>
							</div>
						))}
					</div>
				</section>
			</main>
		</Page>
	);
}

export default VeilingMeesterDashboard;
