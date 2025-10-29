// External imports
import React from 'react';

// Internal imports
import Page from "../../components/screens/Page";
import AppHome from "../../components/soft/AppHome";
import AppBloemSoort from "../../components/soft/AppBloemSoort";
import AppWhatIsFlori from "../../components/soft/AppWhatIsFlori";

function Home() {
	return (
		<Page enableHeader>
			<AppHome/>
			<AppBloemSoort/>
			<AppWhatIsFlori/>
		</Page>
	);
}

export default Home;
