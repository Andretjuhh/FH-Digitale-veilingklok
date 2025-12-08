// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import DashboardIntro from '../../components/sections/veiling-dashboard/DashboardIntro';
import DashboardSnapshot from '../../components/sections/veiling-dashboard/DashboardSnapshot';
import DashboardPlanning from '../../components/sections/veiling-dashboard/DashboardPlanning';

const SNAPSHOT_ITEMS = [
	{ label: 'Lopende ronde', value: '1 kavel', note: 'Rozen Avalanche+ (dummy data)' },
	{ label: 'Komende kavels', value: '3 voorbereidingen', note: 'Alles nog in testfase' },
	{ label: 'Deelnemers', value: '12 ingelogd', note: 'Deze aantallen zijn fictief' },
];

const PLANNING_UPCOMING = [
	{
		id: 'UP-01',
		title: 'Tulpen voorjaar mix',
		seller: 'Van der Meer',
		info: 'Startprijs €5,75 • 40 kratten',
		status: 'klaar' as const,
	},
	{
		id: 'UP-02',
		title: 'Pioenrozen Coral Sunset',
		seller: 'Pioenhof Zeeland',
		info: 'Controle om 11:00',
		status: 'concept' as const,
	},
];

const PLANNING_CHECKLIST = ['Controleer kwaliteitsrapport Avalanche+', 'Nieuwe kavel voor morgen klaarzetten', 'Bespreek verzendplanning met logistiek'];

const PLANNING_COMPLETED = [
	{
		id: 'CP-01',
		title: 'Hydrangea selectie',
		seller: 'Hydrangea House',
		info: 'Afgehandeld om 08:40',
		status: 'afgerond' as const,
	},
	{
		id: 'CP-02',
		title: 'Lelies Stargazer',
		seller: 'Lilium Co.',
		info: 'Terugblik gepland',
		status: 'afgerond' as const,
	},
];

function Dashboard() {
	return (
		<Page enableHeader enableFooter className="dashboard-page">
			<DashboardIntro onCreateAuction={() => window.alert('Nieuwe dummy veiling aangemaakt')} />
			<DashboardSnapshot items={SNAPSHOT_ITEMS} />
			<DashboardPlanning upcoming={PLANNING_UPCOMING} checklist={PLANNING_CHECKLIST} completed={PLANNING_COMPLETED} />
		</Page>
	);
}

export default Dashboard;
