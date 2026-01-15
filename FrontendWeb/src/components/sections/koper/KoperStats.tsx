import React, {useEffect, useState} from 'react';
import {useRootContext} from '../../contexts/RootContext';
import {getKoperStats, getKoperVeilingStats} from '../../../controllers/server/koper';

type StatsCardProps = {
	title: string;
	icon: React.ReactNode;
	data: string;
	percentage?: number;
	enablePercentage?: boolean;
}

function StatsCard(props: StatsCardProps) {
	const {t} = useRootContext();
	const {title, icon, data, enablePercentage, percentage = 0.00} = props;
	return (
		<div className="kweker-stat-card">
			<div className="kweker-stat-title">
		        <span>
			        {icon}
 				</span>
				<p className="products-page-action-card-txt">
					{title}
				</p>
			</div>
			<div className="kweker-stat-data">
				<p>
					{data}
				</p>
			</div>

			{
				enablePercentage && (<div className="kweker-stat-row">
				<span className="kweker-stat-label">
					<p>
						+{percentage}%
					</p>
				</span>
					<span className="kweker-stat-row-txt"> {t('this_month')}</span>
				</div>)
			}
		</div>
	);
}

export function KoperStats() {
	const {t} = useRootContext();
	const [stats, setStats] = useState({
		totalOrders: 0,
		pendingOrders: 0,
		completedOrders: 0,
		canceledOrders: 0
	});

	useEffect(() => {
		const fetchStats = async () => {
			try {
				const response = await getKoperStats();
				if (response.data) {
					setStats(response.data);
				}
			} catch (error) {
				console.error('Failed to fetch koper stats:', error);
			}
		};

		fetchStats();
	}, []);

	return (
		<section className="kweker-stats meester-stats">
			<StatsCard
				title={t('total_orders')}
				icon={<i className="bi bi-cart-fill"></i>}
				data={stats.totalOrders.toString()}
			/>

			<StatsCard
				title={t('pending_orders')}
				icon={<i className="bi bi-clock-fill"></i>}
				data={stats.pendingOrders.toString()}
			/>

			<StatsCard
				title={t('completed_orders')}
				icon={<i className="bi bi-check-circle-fill"></i>}
				data={stats.completedOrders.toString()}
			/>

			<StatsCard
				title={t('canceled_orders')}
				icon={<i className="bi bi-x-circle-fill"></i>}
				data={stats.canceledOrders.toString()}
			/>
		</section>
	);
}

export function KoperVeilingStats() {
	const {t} = useRootContext();
	const [stats, setStats] = useState({
		activeAuctions: 0,
		scheduledAuctions: 0,
		availableProducts: 0,
		yourPurchases: 0
	});

	useEffect(() => {
		const fetchStats = async () => {
			try {
				const response = await getKoperVeilingStats();
				if (response.data) {
					setStats(response.data);
				}
			} catch (error) {
				console.error('Failed to fetch veiling stats:', error);
			}
		};

		fetchStats();
	}, []);

	return (
		<section className="kweker-stats meester-stats">
			<StatsCard
				title={t('active_auctions')}
				icon={<i className="bi bi-clock-fill"></i>}
				data={stats.activeAuctions.toString()}
			/>

			<StatsCard
				title={t('scheduled_auctions')}
				icon={<i className="bi bi-calendar-fill"></i>}
				data={stats.scheduledAuctions.toString()}
			/>

			<StatsCard
				title={t('available_products')}
				icon={<i className="bi bi-bag-fill"></i>}
				data={stats.availableProducts.toString()}
			/>

			<StatsCard
				title={t('your_purchases')}
				icon={<i className="bi bi-cart-check-fill"></i>}
				data={stats.yourPurchases.toString()}
			/>
		</section>
	);
}
