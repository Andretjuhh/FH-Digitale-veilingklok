// External imports
import React from 'react';

// Internal imports
import Button from '../../buttons/Button';
import {useRootContext} from '../../contexts/RootContext';

type SettingsSidebarProps = {
	onChange: (section: string) => void;
	active: string;
};

function SettingsSidebar({ onChange, active }: SettingsSidebarProps) {
	const {t} = useRootContext();
	return (
		<div className="settings-sidebar">
			<Button
				label={t('settings_account')}
				className={active === 'account' ? 'sidebar-account-button active' : 'sidebar-account-button'}
				onClick={() => onChange('account')}
			/>
			<Button
				label={t('settings_privacy')}
				className={active === 'privacy' ? 'sidebar-account-button active' : 'sidebar-account-button'}
				onClick={() => onChange('privacy')}
			/>
			<Button
				label={t('settings_notifications')}
				className={active === 'notifications' ? 'sidebar-account-button active' : 'sidebar-account-button'}
				onClick={() => onChange('notifications')}
			/>
			<Button
				label={t('settings_preferences')}
				className={active === 'preferences' ? 'sidebar-account-button active' : 'sidebar-account-button'}
				onClick={() => onChange('preferences')}
			/>
		</div>
	);
}

export default SettingsSidebar;

