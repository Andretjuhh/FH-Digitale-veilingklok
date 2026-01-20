// External imports
import React from 'react';

// Internal imports
import {useRootContext} from '../../contexts/RootContext';

function NotificationsSettings() {
	const {t} = useRootContext();
	return (
		<section className="settings-panel">
			<header className="settings-panel-header">
				<h2>{t('notifications_title')}</h2>
				<p>{t('notifications_subtitle')}</p>
			</header>
			<div className="settings-panel-body">
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked aria-label={t('notifications_email_updates')} />
					<span className="settings-toggle-label">{t('notifications_email_updates')}</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" aria-label={t('notifications_sms_bids')} />
					<span className="settings-toggle-label">{t('notifications_sms_bids')}</span>
				</label>
				<label className="settings-toggle">
					<input type="checkbox" defaultChecked aria-label={t('notifications_product_announcements')} />
					<span className="settings-toggle-label">{t('notifications_product_announcements')}</span>
				</label>
				<button className="settings-primary-btn" type="button" aria-label={t('notifications_save')}>
					{t('notifications_save')}
				</button>
			</div>
		</section>
	);
}

export default NotificationsSettings;
