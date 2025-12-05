import { useState } from 'react';
import Button from '../../buttons/Button';

function SettingsSidebar({ onChange, active }: { onChange: (section: string) => void; active: string }) {
	return (
		<div className="flex flex-col w-60 h-screen p-4 border-r bg-white shadow-md">
			<Button onClick={() => onChange('account')} className={active === 'account' ? 'font-bold' : ''}>
				Account
			</Button>
			<Button onClick={() => onChange('privacy')} className={active === 'privacy' ? 'font-bold' : ''}>
				Privacy
			</Button>
			<Button onClick={() => onChange('notifications')} className={active === 'notifications' ? 'font-bold' : ''}>
				Notifications
			</Button>
		</div>
	);
}

export default SettingsSidebar;
