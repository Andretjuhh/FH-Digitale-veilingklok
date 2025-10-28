// External imports
import {TFunction} from "i18next";
import {NavigateFunction} from "react-router-dom";

// Internal imports

type InitializeAppProps = {
	t: TFunction;
	navigate: NavigateFunction;
	changeLanguage: (code: any) => Promise<void> | void;
};

/** Initialize application global variable */
export default async function initializeApp({t, navigate, changeLanguage}: InitializeAppProps) {
	window.application = {
		initialized: true,
		t,
		navigate,
		pathname: window.location.pathname,
		languageCode: 'nl',
		changeLanguage
	}
}