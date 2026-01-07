// External imports
import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

// Internal imports
import Page from '../../components/nav/Page';
import SettingsContent from '../../components/sections/settingspage/SettingsContent';
import SettingsSidebar from '../../components/sections/settingspage/SettingsSidebar';

function Settings() {
	const [searchParams] = useSearchParams();
	const sectionParam = searchParams.get('section') || 'account';
	const [section, setSection] = useState(sectionParam);

	useEffect(() => {
		setSection(sectionParam);
	}, [sectionParam]);

	return (
		<Page enableHeader className="settings-page">
			<div className="settings-layout">
				<SettingsSidebar onChange={setSection} active={section} />
				<SettingsContent active={section} />
			</div>
		</Page>
	);
}

export default Settings;
