// External imports
import React from "react";

type PlanningItem = {
	id: string;
	title: string;
	seller: string;
	info: string;
	status: "concept" | "klaar" | "afgerond";
};

type DashboardPlanningProps = {
	upcoming: PlanningItem[];
	checklist: string[];
	completed: PlanningItem[];
};

function DashboardPlanning(props: DashboardPlanningProps) {
	const {upcoming, checklist, completed} = props;

	const statusLabels: Record<PlanningItem["status"], string> = {
		concept: "Conceptfase",
		klaar: "Klaar voor start",
		afgerond: "Afgerond",
	};

	return (
		<section className="dashboard-section dashboard-planning">
			<header className="dashboard-section-header">
				<h2 className="dashboard-section-title">Planning en taken</h2>
			</header>

			<div className="dashboard-columns">
				<div className="dashboard-column">
					<h3 className="dashboard-column-title">Aankomende veilingen</h3>
					<ul className="dashboard-list">
						{upcoming.map((item) => (
							<li className="dashboard-list-item" key={item.id}>
								{/* Titel en verkoper */}
								<div className="dashboard-list-item-main">
									<strong>{item.title}</strong>
									<span>{item.seller}</span>
								</div>
								<span className={`dashboard-badge status-${item.status}`}>
                  {statusLabels[item.status]}
                </span>
								<span className="dashboard-list-item-info">{item.info}</span>
							</li>
						))}
					</ul>
				</div>

				<div className="dashboard-column">
					<h3 className="dashboard-column-title">Taken voor vandaag</h3>
					<ul className="dashboard-checklist">
						{checklist.map((todo, index) => (
							<li className="dashboard-checklist-item" key={index}>
								<i className="bi bi-circle dashboard-checklist-icon"/>
								<span>{todo}</span>
							</li>
						))}
					</ul>
				</div>

				<div className="dashboard-column">
					<h3 className="dashboard-column-title">Recent afgerond</h3>
					<ul className="dashboard-list">
						{completed.map((item) => (
							<li className="dashboard-list-item" key={item.id}>
								<div className="dashboard-list-item-main">
									<strong>{item.title}</strong>
									<span>{item.seller}</span>
								</div>
								<span className={`dashboard-badge status-${item.status}`}>
                  {statusLabels[item.status]}
                </span>
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
