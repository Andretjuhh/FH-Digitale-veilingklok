// External imports
import React from 'react';

// Internal imports
import AccountSettings from './AccountSettings';
import PrivacySettings from './PrivacySettings';
import NotificationsSettings from './NotificationsSettings';
import PreferencesSettings from './PreferencesSettings';

type SettingsContentProps = {
	active: string;
};

function SettingsContent({ active }: SettingsContentProps) {
	switch (active) {
		case 'privacy':
			return <PrivacySettings />;
		case 'notifications':
			return <NotificationsSettings />;
		case 'preferences':
			return <PreferencesSettings />;
		case 'account':
		default:
			return <AccountSettings />;
	}
}

export default SettingsContent;
