import React from 'react';
import Page from "../../components/nav/Page";
import {useRootContext} from "../../components/contexts/RootContext";
import {KwekerOrderStats} from "../../components/sections/kweker/KwekerStats";

function KwekerOrders() {
	const {t, account} = useRootContext();

	return (
		<Page enableHeader className="kweker-page" enableHeaderAnimation={false}>
			<main className="kweker-container">
				<section className="kweker-hallo">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2>
						{t('kweker_orders_description')}
					</h2>
				</section>

				<KwekerOrderStats/>
			</main>
		</Page>
	);
}

export default KwekerOrders;