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
		<Page enableHeader className="kweker-page" enableHeaderAnimation={false}>
			<main className="kweker-container">
				<section className="kweker-hallo">
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
