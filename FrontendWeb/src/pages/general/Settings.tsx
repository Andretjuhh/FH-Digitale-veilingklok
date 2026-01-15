// External imports
import React, { useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

// Internal imports
import Page from '../../components/nav/Page';
import Button from '../../components/buttons/Button';
import { useRootContext } from '../../components/contexts/RootContext';
import SettingsContent from '../../components/sections/settingspage/SettingsContent';
import SettingsSidebar from '../../components/sections/settingspage/SettingsSidebar';
import { AccountType } from '../../declarations/enums/AccountTypes';

function Settings() {
	const { account, navigate, t } = useRootContext();
	const [searchParams] = useSearchParams();
	const sectionParam = searchParams.get('section') || 'account';
	const [section, setSection] = useState(sectionParam);

	useEffect(() => {
		setSection(sectionParam);
	}, [sectionParam]);

	const dashboardPath = useMemo(() => {
		switch (account?.accountType) {
			case AccountType.Koper:
				return '/koper/veilingen';
			case AccountType.Kweker:
				return '/kweker/dashboard';
			case AccountType.Veilingmeester:
				return '/veilingmeester/dashboard';
			default:
				return '/';
		}
	}, [account?.accountType]);

	return (
		<Page enableHeader className="settings-page">
			<div className="settings-layout">
				<SettingsSidebar onChange={setSection} active={section} />
				<div className="settings-main">
					<div className="settings-main-header">
						<Button label={t('settings_back_to_dashboard')} aria-label={t('settings_back_to_dashboard_aria')} className="settings-back-btn" onClick={() => navigate(dashboardPath)} />
					</div>
					<SettingsContent active={section} />
				</div>
			</div>
		</Page>
	);
}

export default Settings;
