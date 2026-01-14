// External imports
import React from 'react';

function PrivacySettings() {
	return (
		<section className="settings-panel">
			<header className="settings-panel-header">
				<h2>Privacy</h2>
				<p>Control how your profile and activity are shared.</p>
			</header>
			<div className="settings-panel-body">
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked />
					<span className="settings-toggle-label">Show my profile to other users</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" />
					<span className="settings-toggle-label">Allow search engines to index my profile</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked />
					<span className="settings-toggle-label">Share activity status</span>
				</label>
				<button className="settings-primary-btn" type="button">
					Update privacy
				</button>
			</div>
		</section>
	);
}

export default PrivacySettings;
