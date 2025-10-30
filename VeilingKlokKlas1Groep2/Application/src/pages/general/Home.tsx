// External imports
import React from "react";

// Internal imports
import Page from "../../components/screens/Page";
import AppHome from "../../components/sections/AppHome";
import AppBloemSoort from "../../components/sections/AppBloemSoort";
import AppWhatIsFlori from "../../components/sections/AppWhatIsFlori";
import AppFooter from "../../components/sections/AppFooter";
import KwekerDashboard from "./Kweker_dashboard";

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
