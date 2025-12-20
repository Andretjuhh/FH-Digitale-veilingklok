// External imports
import {NavigateFunction} from 'react-router-dom';

// Internal imports

type InitializeAppProps = {
	navigate: NavigateFunction;
};

/** Initialize application global variable */
export default async function initializeApp({navigate}: InitializeAppProps) {
	console.log('Setting up application global variable...');

	window.application = {
		initialized: false,
		navigate,
		pathname: window.location.pathname,
		languageCode: 'nl',
	};
}
