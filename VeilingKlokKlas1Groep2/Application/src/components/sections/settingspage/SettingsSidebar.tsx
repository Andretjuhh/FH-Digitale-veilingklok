import { useState } from 'react';
import Button from '../../buttons/Button';

function SettingsSidebar({ onChange, active }: { onChange: (section: string) => void; active: string }) {
	return (
		<div className="settings-sidebar">
			<Button label="Account" className="sidebar-account-button" onClick={() => onChange('account')}></Button>
			<Button label="Privacy" className="sidebar-account-button" onClick={() => onChange('privacy')}></Button>
			<Button label="Notifications" className="sidebar-account-button" onClick={() => onChange('notifications')}></Button>
		</div>
	);
}

export default SettingsSidebar;
