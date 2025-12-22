// External imports
import React, { useCallback, useEffect, useState } from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import DashboardIntro from '../../components/sections/veiling-dashboard/DashboardIntro';
import DashboardSnapshot from '../../components/sections/veiling-dashboard/DashboardSnapshot';
import DashboardPlanning from '../../components/sections/veiling-dashboard/DashboardPlanning';
import { getProducts } from '../../controllers/server/veilingmeester';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';

function Dashboard() {
	const [products, setProducts] = useState<ProductOutputDto[]>([]);
	const [loading, setLoading] = useState(true);

	const initializeDashboard = useCallback(async () => {
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

	useEffect(() => {
		initializeDashboard();
	}, [initializeDashboard]);

	const upcoming = products
		.filter((p) => !p.auctionedPrice)
		.map((p) => ({
			id: p.id,
			title: p.name,
			seller: p.companyName,
			info: `Voorraad: ${p.stock} • ${p.dimension}`,
			status: 'klaar' as const,
		}));

	const completed = products
		.filter((p) => p.auctionedPrice)
		.map((p) => ({
			id: p.id,
			title: p.name,
			seller: p.companyName,
			info: `Verkocht voor €${p.auctionedPrice} • ${p.auctionedAt ? new Date(p.auctionedAt).toLocaleTimeString() : ''}`,
			status: 'afgerond' as const,
		}));

	const snapshotItems = [
		{ label: 'Totaal producten', value: `${products.length}`, note: 'In de database' },
		{ label: 'Wachtend', value: `${upcoming.length}`, note: 'Klaar voor veiling' },
		{ label: 'Afgerond', value: `${completed.length}`, note: 'Vandaag geveild' },
	];

	const checklist = ['Controleer nieuwe aanmeldingen van kwekers', 'Zet de veilingklok klaar voor de volgende ronde', 'Valideer de prijzen van de komende kavels'];

	if (loading) {
		return (
			<Page enableHeader enableFooter className="dashboard-page">
				<div className="flex items-center justify-center h-64">
					<div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary-main"></div>
				</div>
			</Page>
		);
	}

	return (
		<Page enableHeader enableFooter className="dashboard-page">
			<DashboardIntro onCreateAuction={() => window.alert('Nieuwe veiling functionaliteit komt binnenkort')} />
			<DashboardSnapshot items={snapshotItems} />
			<DashboardPlanning upcoming={upcoming} checklist={checklist} completed={completed} />
		</Page>
	);
}

export default Dashboard;
