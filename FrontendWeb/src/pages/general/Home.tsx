// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import AppHome from '../../components/sections/home/AppHome';
import AppBloemSoort from '../../components/sections/home/AppBloemSoort';
import AppWhatIsFlori from '../../components/sections/home/AppWhatIsFlori';

function Home() {
	return (
		<Page enableHeader enableFooter>
			<AppHome/>
			<AppBloemSoort/>
			<AppWhatIsFlori/>
		</Page>
	);
}

export default Home;
