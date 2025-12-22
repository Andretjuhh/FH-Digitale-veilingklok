// External imports
import React from "react";

type SnapshotItem = {
	label: string;
	value: string;
	note?: string;
};

type DashboardSnapshotProps = {
	items: SnapshotItem[];
};

function DashboardSnapshot(props: DashboardSnapshotProps) {
	const {items} = props;

	return (
		<section className="dashboard-section dashboard-snapshot">
			<header className="dashboard-section-header">
				<h2 className="dashboard-section-title">Actuele stand</h2>
			</header>

			<div className="dashboard-card-grid">
				{items.map((item) => (
					<div className="dashboard-card" key={item.label}>
						<span className="dashboard-card-label">{item.label}</span>
						<strong className="dashboard-card-value">{item.value}</strong>
						{item.note && (
							<span className="dashboard-card-note">{item.note}</span>
						)}
					</div>
				))}
			</div>
		</section>
	);
}

export default DashboardSnapshot;
