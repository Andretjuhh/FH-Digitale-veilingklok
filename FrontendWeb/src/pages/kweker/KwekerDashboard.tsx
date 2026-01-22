// External imports
import React from 'react';

// Internal import
import { useRootContext } from '../../components/contexts/RootContext';

// Components
import { KwekerDashboardStats } from '../../components/sections/kweker/KwekerStats';
import { RevenueChart } from '../../components/sections/kweker/RevenueChart';
import Page from '../../components/nav/Page';

function KwekerDashboard() {
	const { t, account } = useRootContext();

	return (
		<Page enableHeader className="kweker-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="kweker-products-page-ctn">
				<section className="page-title-section" aria-labelledby="kweker-dashboard-title kweker-dashboard-subtitle">
					<h1 id="kweker-dashboard-title">
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<p id="kweker-dashboard-subtitle" className="page-subtitle">
						{t('kweker_desc')}
					</p>
				</section>

				<KwekerDashboardStats aria-label={t('aria_kweker_dashboard_stats')} />

				<section aria-label={t('aria_kweker_revenue_chart')}>
					<RevenueChart />
				</section>
			</main>
		</Page>
	);
}

export default KwekerDashboard;
