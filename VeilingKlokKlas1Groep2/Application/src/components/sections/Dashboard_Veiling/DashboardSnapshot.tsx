// External imports
import React from 'react';

type SnapshotItem = {
	label: string;
	value: string;
	note?: string;
};

type DashboardSnapshotProps = {
	items: SnapshotItem[];
};

function DashboardSnapshot(props: DashboardSnapshotProps) {
	const { items } = props;

	return (
		<section className="dashboard-section dashboard-snapshot">
			{/* Tweede blok met snelle cijfers */}
			<header className="dashboard-section-header">
				<h2 className="dashboard-section-title">Snelle blik</h2>
				<span className="dashboard-section-description">Een kleine greep uit wat er speelt in de veiling. Later kunnen we dit uitbreiden.</span>
			</header>

			<div className="dashboard-card-grid">
				{items.map((item) => (
					/* Elk kaartje is nu gewoon nepdata */
					<div className="dashboard-card" key={item.label}>
						<span className="dashboard-card-label">{item.label}</span>
						<strong className="dashboard-card-value">{item.value}</strong>
						{item.note && <span className="dashboard-card-note">{item.note}</span>}
					</div>
				))}
			</div>
		</section>
	);
}

export default DashboardSnapshot;
