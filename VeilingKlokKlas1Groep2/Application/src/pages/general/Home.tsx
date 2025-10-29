// External imports
import React from 'react';

// Internal imports
import Page from "../../components/screens/Page";
import AppHome from "../../components/soft/AppHome";
import AppBloemSoort from "../../components/soft/AppBloemSoort";
import AppWhatIsFlori from "../../components/soft/AppWhatIsFlori";
import AppFooter from "../../components/soft/AppFooter";

function Home() {
	return (
		<Page enableHeader>
			<AppHome/>
			<AppBloemSoort/>
			<AppWhatIsFlori/>
			<AppFooter/>
		</Page>
	);
}

export default Home;
