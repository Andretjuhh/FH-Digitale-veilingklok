// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import SettingsContent from '../../components/sections/settingspage/SettingsContent';
import SettingsSidebar from '../../components/sections/settingspage/SettingsSidebar';

function Settings() {
	const [activeSection, setActiveSection] = React.useState('account');

	return (
		<Page enableHeader={false}>
			<div className="settings-layout">
				<SettingsSidebar active={activeSection} onChange={setActiveSection} />
				<SettingsContent active={activeSection} />
			</div>
		</Page>
	);
}
export default Settings;
