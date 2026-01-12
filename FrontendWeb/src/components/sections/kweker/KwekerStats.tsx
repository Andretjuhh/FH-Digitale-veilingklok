import React from 'react';
import {useRootContext} from "../../contexts/RootContext";

type Props = {
	totalProducts?: number;
	activeAuctions?: number;
	totalRevenue?: number;
	stemsSold?: number;
}

function StatsCard({title, icon, data, trendLabel, periodLabel}: { title: string, icon: React.ReactNode, data: string, trendLabel: string, periodLabel: string }) {
	return (
		<div className="kweker-stat-card">
			<div className="kweker-stat-title">
		        <span>
			        {icon}
 				</span>
				<p className="kweker-stat-txt">
					{title}
				</p>
			</div>
			<div className="kweker-stat-data">
				<p>
					{data}
				</p>
			</div>

			<div className="kweker-stat-row">
				<span className="kweker-stat-label">
					<p>
						{trendLabel}
					</p>
				</span>
				<span className="kweker-stat-row-txt">{periodLabel}</span>
			</div>
		</div>
	);
}

function KwekerStats(props: Props) {
	const {t} = useRootContext();

	return (
		<section className="kweker-stats">
			<StatsCard
				title={t('kweker_stats_offered')}
				icon={<i className="bi bi-bag-fill"></i>}
				data={t('kweker_stats_offered_value')}
				trendLabel={t('kweker_stats_change')}
				periodLabel={t('kweker_stats_this_month')}
			/>

			<StatsCard
				title={t('kweker_stats_sold')}
				icon={<i className="bi bi-box-seam-fill"></i>}
				data={t('kweker_stats_sold_value')}
				trendLabel={t('kweker_stats_change')}
				periodLabel={t('kweker_stats_this_month')}
			/>

			<StatsCard
				title={t('kweker_stats_total_revenue')}
				icon={<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1792 1792" height="20" fill="currentColor" width="20">
					<path
						d="M1362 1185q0 153-99.5 263.5t-258.5 136.5v175q0 14-9 23t-23 9h-135q-13 0-22.5-9.5t-9.5-22.5v-175q-66-9-127.5-31t-101.5-44.5-74-48-46.5-37.5-17.5-18q-17-21-2-41l103-135q7-10 23-12 15-2 24 9l2 2q113 99 243 125 37 8 74 8 81 0 142.5-43t61.5-122q0-28-15-53t-33.5-42-58.5-37.5-66-32-80-32.5q-39-16-61.5-25t-61.5-26.5-62.5-31-56.5-35.5-53.5-42.5-43.5-49-35.5-58-21-66.5-8.5-78q0-138 98-242t255-134v-180q0-13 9.5-22.5t22.5-9.5h135q14 0 23 9t9 23v176q57 6 110.5 23t87 33.5 63.5 37.5 39 29 15 14q17 18 5 38l-81 146q-8 15-23 16-14 3-27-7-3-3-14.5-12t-39-26.5-58.5-32-74.5-26-85.5-11.5q-95 0-155 43t-60 111q0 26 8.5 48t29.5 41.5 39.5 33 56 31 60.5 27 70 27.5q53 20 81 31.5t76 35 75.5 42.5 62 50 53 63.5 31.5 76.5 13 94z"
					></path>
				</svg>}
				data={t('kweker_stats_total_revenue_value')}
				trendLabel={t('kweker_stats_change')}
				periodLabel={t('kweker_stats_this_month')}
			/>

			<StatsCard
				title={t('kweker_stats_stems_sold')}
				icon={<i className="bi bi-flower3"></i>}
				data={t('kweker_stats_stems_sold_value')}
				trendLabel={t('kweker_stats_change')}
				periodLabel={t('kweker_stats_this_month')}
			/>
		</section>
	);
}

export default KwekerStats;


