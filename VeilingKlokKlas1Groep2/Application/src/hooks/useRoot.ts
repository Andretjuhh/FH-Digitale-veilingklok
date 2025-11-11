// External imports
import {useCallback, useLayoutEffect, useState} from 'react';
import {useNavigate} from 'react-router-dom';

// Internal imports
import {SupportedLanguages, useTranslation} from '../controllers/localization';
import LocalStorage from '../controllers/localStorage';
import initializeApp from '../controllers/application';

function useRoot() {
	//  Router navigation
	const navigate = useNavigate();

	// Application initialization status
	const [initialized, setInitialized] = useState(false);

	// Initialize localization
	const {t, i18n} = useTranslation();

	// Current Application language
	const [languageCode, setLanguageCode] = useState<SupportedLanguages>('nl');

	// Check if the application is logged in
	const [loggedIn, setLoggedIn] = useState<boolean>(false);

	// Initialize Application & Global application functions
	useLayoutEffect(() => {
		console.log('Initializing application...');

		// Initialize Application
		initializeApp({t, navigate, changeLanguage}).then(() => {
			setInitialized(true);
			setLoggedIn(false);
			changeLanguage(window.application.languageCode);
		});

		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, []);

	// Function to switch application language
	const changeLanguage = useCallback(
		(code: SupportedLanguages) => {
			i18n.changeLanguage(code);
			window.application.languageCode = code;
			LocalStorage.setItem('language', code, 'persistent');
			// This will re-render the components that depend on the languageCode
			setLanguageCode(code);
			// Update document language attribute
			document.documentElement.setAttribute('lang', code);
		},
		[i18n]
	);

	return {
		initialized,
		loggedIn,
		languageCode,
		t,
		setLoggedIn,
		changeLanguage,
		navigate,
	};
}

export default useRoot;
