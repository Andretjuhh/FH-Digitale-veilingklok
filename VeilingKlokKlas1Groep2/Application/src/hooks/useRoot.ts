// External imports
import {useCallback, useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";

// Internal imports
import {useTranslation} from "../controllers/localization";
import {SupportedLanguages} from "../controllers/localization";
import LocalStorage from "../controllers/localStorage";
import initializeApp from "../controllers/application";

function useRoot() {
	//  Router navigation
	const navigate = useNavigate();

	// Application initialization status
	const [initialized, setInitialized] = useState(false);

	// Initialize localization
	const {t, i18n} = useTranslation();

	// Check if the application is logged in
	const [loggedIn, setLoggedIn] = useState<boolean>(false);

	// Initialize Application & Global application functions
	useEffect(() => {
		// Initialize Application
		initializeApp({t, navigate, changeLanguage}).then(() => {
			setInitialized(true);
			setLoggedIn(false);
		});

		// Initialize User Agent Primary Language
		i18n.changeLanguage(window.application.languageCode).then(null);

		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [i18n]);

	// Function to switch application language
	const changeLanguage = useCallback((code: SupportedLanguages) => {
		i18n.changeLanguage(code);
		window.application.languageCode = code;
		LocalStorage.setItem('language', code, 'persistent');
	}, [i18n]);

	return {
		initialized,
		loggedIn,
		setLoggedIn,
	}
}

export default useRoot;