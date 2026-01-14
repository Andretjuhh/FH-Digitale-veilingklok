// External imports
import React from 'react';

// Internal imports
import Button from '../../buttons/Button';

type SettingsSidebarProps = {
	onChange: (section: string) => void;
	active: string;
};

function SettingsSidebar({ onChange, active }: SettingsSidebarProps) {
	return (
		<div className="settings-sidebar">
			<Button
				label="Account"
				className={active === 'account' ? 'sidebar-account-button active' : 'sidebar-account-button'}
				onClick={() => onChange('account')}
			/>
			<Button
				label="Privacy"
				className={active === 'privacy' ? 'sidebar-account-button active' : 'sidebar-account-button'}
				onClick={() => onChange('privacy')}
			/>
			<Button
				label="Notifications"
				className={active === 'notifications' ? 'sidebar-account-button active' : 'sidebar-account-button'}
				onClick={() => onChange('notifications')}
			/>
		</div>
	);
}

export default SettingsSidebar;
