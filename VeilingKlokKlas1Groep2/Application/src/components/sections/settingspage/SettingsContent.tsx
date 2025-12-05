import AccountSettings from '../settingspage/AccountSettings';
import PrivacySettings from '../settingspage/PrivacySettings';
import NotificationsSettings from '../settingspage/NotificationsSettings';

function SettingsContent({ active }: { active: string }) {
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
