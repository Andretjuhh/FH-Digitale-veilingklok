// External imports
import React from 'react';

type PlanningItem = {
	id: string;
	title: string;
	seller: string;
	info: string;
	status: 'concept' | 'klaar' | 'afgerond';
};

type DashboardPlanningProps = {
	upcoming: PlanningItem[];
	checklist: string[];
	completed: PlanningItem[];
};

function DashboardPlanning(props: DashboardPlanningProps) {
	const { upcoming, checklist, completed } = props;

	const statusLabels: Record<PlanningItem['status'], string> = {
		concept: 'Concept',
		klaar: 'Klaar voor live',
		afgerond: 'Afgerond'
	};

	return (
		<section className="dashboard-section dashboard-planning">
			{/* Planning blok bestaat uit drie kolommen */}
			<header className="dashboard-section-header">
				<h2 className="dashboard-section-title">In de planning</h2>
				<span className="dashboard-section-description">Dummy lijstjes om een idee te geven van hoe het overzicht kan worden.</span>
			</header>

			<div className="dashboard-columns">
				<div className="dashboard-column">
					<h3 className="dashboard-column-title">Komende rondes</h3>
					<ul className="dashboard-list">
						{upcoming.map((item) => (
							<li className="dashboard-list-item" key={item.id}>
								{/* Titel en verkoper */}
								<div className="dashboard-list-item-main">
									<strong>{item.title}</strong>
									<span>{item.seller}</span>
								</div>
								<span className={`dashboard-badge status-${item.status}`}>{statusLabels[item.status]}</span>
								<span className="dashboard-list-item-info">{item.info}</span>
							</li>
						))}
					</ul>
				</div>

				<div className="dashboard-column">
					<h3 className="dashboard-column-title">To-do voor vandaag</h3>
					<ul className="dashboard-checklist">
						{checklist.map((todo, index) => (
							<li className="dashboard-checklist-item" key={index}>
								{/* Later kan je hier een echte checkbox van maken */}
								<i className="bi bi-circle dashboard-checklist-icon" />
								<span>{todo}</span>
							</li>
						))}
					</ul>
				</div>

				<div className="dashboard-column">
					<h3 className="dashboard-column-title">Net afgerond</h3>
					<ul className="dashboard-list">
						{completed.map((item) => (
							<li className="dashboard-list-item" key={item.id}>
								<div className="dashboard-list-item-main">
									<strong>{item.title}</strong>
									<span>{item.seller}</span>
								</div>
								<span className={`dashboard-badge status-${item.status}`}>{statusLabels[item.status]}</span>
								<span className="dashboard-list-item-info">{item.info}</span>
							</li>
						))}
					</ul>
				</div>
			</div>
		</section>
	);
}

export default DashboardPlanning;
