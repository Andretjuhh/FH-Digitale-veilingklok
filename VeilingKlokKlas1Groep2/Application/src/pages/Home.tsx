// External imports
import React from 'react';

// Internal imports
import Page from "../components/screens/Page";
import AppHome from "../components/soft/AppHome";
import AppBloemSoort from "../components/soft/AppBloemSoort";

function Home() {
	return (
		<Page enableHeader>
			<AppHome/>
			<AppBloemSoort/>
		</Page>
	);
}

export default Home;
