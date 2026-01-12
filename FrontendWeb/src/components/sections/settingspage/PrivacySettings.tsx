// External imports
import React from 'react';

// Internal imports
import {useRootContext} from '../../contexts/RootContext';

function PrivacySettings() {
	const {t} = useRootContext();
	return (
		<section className="settings-panel">
			<header className="settings-panel-header">
				<h2>{t('privacy_title')}</h2>
				<p>{t('privacy_subtitle')}</p>
			</header>
			<div className="settings-panel-body">
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked aria-label={t('privacy_toggle_profile')} />
					<span className="settings-toggle-label">{t('privacy_toggle_profile')}</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" aria-label={t('privacy_toggle_search')} />
					<span className="settings-toggle-label">{t('privacy_toggle_search')}</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked aria-label={t('privacy_toggle_status')} />
					<span className="settings-toggle-label">{t('privacy_toggle_status')}</span>
				</label>
				<button className="settings-primary-btn" type="button" aria-label={t('privacy_save')}>
					{t('privacy_save')}
				</button>
			</div>
		</section>
	);
}

export default PrivacySettings;
