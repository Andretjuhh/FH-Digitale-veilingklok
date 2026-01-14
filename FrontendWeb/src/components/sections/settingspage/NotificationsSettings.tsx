// External imports
import React from 'react';

function NotificationsSettings() {
	return (
		<section className="settings-panel">
			<header className="settings-panel-header">
				<h2>Notifications</h2>
				<p>Choose how you want to hear from us.</p>
			</header>
			<div className="settings-panel-body">
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked />
					<span className="settings-toggle-label">Email me about auction updates</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" />
					<span className="settings-toggle-label">Send SMS alerts for bids</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked />
					<span className="settings-toggle-label">Product announcements</span>
				</label>
				<button className="settings-primary-btn" type="button">
					Save notifications
				</button>
			</div>
		</section>
	);
}

export default NotificationsSettings;
