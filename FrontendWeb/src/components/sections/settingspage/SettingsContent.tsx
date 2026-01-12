// External imports
import React from 'react';

// Internal imports
import AccountSettings from './AccountSettings';
import PrivacySettings from './PrivacySettings';
import NotificationsSettings from './NotificationsSettings';

type SettingsContentProps = {
	active: string;
};

function SettingsContent({ active }: SettingsContentProps) {
	switch (active) {
		case 'privacy':
			return <PrivacySettings />;
		case 'notifications':
			return <NotificationsSettings />;
		case 'account':
		default:
			return <AccountSettings />;
	}
}

export default SettingsContent;
