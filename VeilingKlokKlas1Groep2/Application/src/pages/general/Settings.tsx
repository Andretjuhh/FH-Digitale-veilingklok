// Internal imports
import React, { useState } from 'react';
import Page from '../../components/nav/Page';
import SettingsContent from '../../components/sections/settingspage/SettingsContent';
import SettingsSidebar from '../../components/sections/settingspage/SettingsSidebar';

function Settings() {
	const [section, setSection] = useState('account');
	return (
		<div className="flex h-screen bg-gray-50">
			<SettingsSidebar onChange={setSection} active={section} />
			<SettingsContent active={section} />
		</div>
	);
}
export default Settings;
