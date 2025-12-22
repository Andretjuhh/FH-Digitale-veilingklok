// External imports
import React from "react";

// Internal imports
import Button from "../../buttons/Button";

type DashboardIntroProps = {
	onCreateAuction: () => void;
};

function DashboardIntro(props: DashboardIntroProps) {
	const {onCreateAuction} = props;

	return (
		<section className="dashboard-section dashboard-intro">
			<div className="dashboard-intro-content">
				<div className="dashboard-intro-text">
					<span className="dashboard-tag">Jouw dashboard</span>
					<h1 className="dashboard-title">Welkom in het veilingdashboard</h1>
					<p className="dashboard-description">
						Dit is je startpunt om nieuwe kavels klaar te zetten, lopende rondes
						in de gaten te houden en te wennen aan de flow. Alles is nog dummy
						data, zodat we rustig kunnen opbouwen.
					</p>

					<div className="dashboard-cta">
						<Button
							className="dashboard-primary-btn"
							label="Nieuwe veiling aanmaken"
							icon="bi-plus-lg"
							onClick={onCreateAuction}
						/>
						<Button
							className="dashboard-secondary-btn"
							label="Bekijk kavels"
							icon="bi-box"
						/>
					</div>
				</div>
				<div className="dashboard-intro-illustration">
					<div className="dashboard-intro-card">
						<span className="dashboard-intro-card-tag">Vandaag</span>
						<p className="dashboard-intro-card-title">Rozen Avalanche+</p>
						<span className="dashboard-intro-card-meta">
              Startprijs €6,50 • 120 bossen
            </span>
						<div className="dashboard-intro-progress">
							<div
								className="dashboard-intro-progress-bar"
								style={{width: "40%"}}
							/>
						</div>
						<span className="dashboard-intro-card-note">
              Live rondes starten om 09:30
            </span>
					</div>
				</div>
			</div>
		</section>
	);
}

export default DashboardIntro;
