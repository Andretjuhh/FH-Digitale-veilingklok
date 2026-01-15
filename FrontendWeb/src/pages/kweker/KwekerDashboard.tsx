// External imports
import React from 'react';

// Internal import
import {useRootContext} from '../../components/contexts/RootContext';

// Components
import {KwekerDashboardStats} from '../../components/sections/kweker/KwekerStats';
import Page from '../../components/nav/Page';


function KwekerDashboard() {
	const {t, account} = useRootContext();

	return (
		<Page enableHeader className="kweker-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="kweker-products-page-ctn">
				<section className="page-title-section">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
				</section>

				<KwekerDashboardStats/>
			</main>
		</Page>
	);
}

export default KwekerDashboard;
