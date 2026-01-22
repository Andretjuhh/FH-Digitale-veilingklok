import React, { useEffect, useState } from 'react';
import { useRootContext } from '../../contexts/RootContext';
import { getMeesterStats } from '../../../controllers/server/veilingmeester';

type StatsCardProps = {
	title: string;
	icon: React.ReactNode;
	data: string;
	percentage?: number;
	enablePercentage?: boolean;
};

function StatsCard(props: StatsCardProps) {
	const { t } = useRootContext();
	const { title, icon, data, enablePercentage, percentage = 0.0 } = props;
	return (
		<div className="kweker-stat-card">
			<div className="kweker-stat-title">
				<span>{icon}</span>
				<p className="products-page-action-card-txt">{title}</p>
			</div>
			<div className="kweker-stat-data">
				<p>{data}</p>
			</div>

			{enablePercentage && (
				<div className="kweker-stat-row">
					<span className="kweker-stat-label">
						<p>+{percentage}%</p>
					</span>
					<span className="kweker-stat-row-txt"> {t('this_month')}</span>
				</div>
			)}
		</div>
	);
}

export function MeesterStats(props: React.HTMLAttributes<HTMLElement>) {
	const { t } = useRootContext();
	const [stats, setStats] = useState({
		totalVeilingKlokken: 0,
		activeVeilingKlokken: 0,
		totalProducts: 0,
		totalOrders: 0,
	});

	useEffect(() => {
		const fetchStats = async () => {
			try {
				const response = await getMeesterStats();
				if (response.data) {
					setStats(response.data);
				}
			} catch (error) {
				console.error('Failed to fetch meester stats:', error);
			}
		};

		fetchStats();
	}, []);

	return (
		<section className="kweker-stats meester-stats" {...props}>
			<StatsCard
				title={t('total_veilingklokken')}
				icon={<i className="bi bi-clock-fill"></i>}
				data={stats.totalVeilingKlokken.toString()}
			/>

			<StatsCard
				title={t('active_veilingklokken')}
				icon={<i className="bi bi-clock-history"></i>}
				data={stats.activeVeilingKlokken.toString()}
			/>

			<StatsCard title={t('total_products')} icon={<i className="bi bi-bag-fill"></i>} data={stats.totalProducts.toString()} />

			<StatsCard title={t('total_orders')} icon={<i className="bi bi-cart-fill"></i>} data={stats.totalOrders.toString()} />
		</section>
	);
}

export function MeesterProductStats(props: React.HTMLAttributes<HTMLElement>) {
	const { t } = useRootContext();
	const [stats, setStats] = useState({
		totalVeilingKlokken: 0,
		activeVeilingKlokken: 0,
		totalProducts: 0,
		totalOrders: 0,
	});

	useEffect(() => {
		const fetchStats = async () => {
			try {
				const response = await getMeesterStats();
				if (response.data) {
					setStats(response.data);
				}
			} catch (error) {
				console.error('Failed to fetch meester stats:', error);
			}
		};

		fetchStats();
	}, []);

	return (
		<section className="kweker-stats kweker-product-stats" {...props}>
			<StatsCard title={t('total_products')} icon={<i className="bi bi-bag-fill"></i>} data={stats.totalProducts.toString()} />

			<StatsCard title={t('total_orders')} icon={<i className="bi bi-cart-fill"></i>} data={stats.totalOrders.toString()} />

			<StatsCard
				title={t('active_veilingklokken')}
				icon={<i className="bi bi-clock-fill"></i>}
				data={stats.activeVeilingKlokken.toString()}
			/>
		</section>
	);
}
