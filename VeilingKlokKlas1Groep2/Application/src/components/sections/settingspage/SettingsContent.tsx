import { useState } from 'react';
import AccountSettings from './AccountSettings';
import PrivacySettings from './PrivacySettings';
import NotificationsSettings from './NotificationsSettings';

function SettingsContent({ active }: { active: string }) {
	switch (active) {
		case 'privacy':
			return <div>Privacy Settings Page</div>;
		case 'notifications':
			return <div>Notifications Settings Page</div>;
		case 'account':
		default:
			return <AccountSettings />;
	}
}
export default SettingsContent;
