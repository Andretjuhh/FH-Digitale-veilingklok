// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import SettingsContent from '../../components/sections/settingspage/SettingsContent';

function Settings() {
	return (
		<Page enableHeader={false}>
			<SettingsContent />
		</Page>
	);
}

export default Settings;
