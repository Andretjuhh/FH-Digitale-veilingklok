// External imports
import {useCallback, useLayoutEffect, useState} from 'react';
import {useNavigate} from 'react-router-dom';

// Internal imports
import {SupportedLanguages, useTranslation} from '../controllers/services/localization';
import initializeApp from '../controllers/services/application';
import {getAccountInfo, logoutAccount} from '../controllers/server/account';
import {LocalStorageService} from "../controllers/services/localStorage";
import {AccountOutputDto} from "../declarations/dtos/output/AccountOutputDto";
import {AuthOutputDto} from "../declarations/dtos/output/AuthOutputDto";

function useRoot() {
	//  Router navigation
	const navigate = useNavigate();

	// Initialize localization
	const {t, i18n} = useTranslation();

	// Application initialization status
	const [initialized, setInitialized] = useState(false);

	// Check if the application is logged in
	const [account, setAccount] = useState<AccountOutputDto | undefined>();
	const [loggedIn, setLoggedIn] = useState<boolean>(false);

	// Current Application language
	const [languageCode, setLanguageCode] = useState<SupportedLanguages>('nl');

	// Initialize Application & Global application functions
	useLayoutEffect(() => {
		// Initialize Application
		initializeRoot().then(null);
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, []);

	// Function to switch application language
	const changeLanguage = useCallback(
		(code: SupportedLanguages) => {
			i18n.changeLanguage(code).then(null);
			window.application.languageCode = code;
			LocalStorageService.setItem('language', code);
			// This will re-render the components that depend on the languageCode
			setLanguageCode(code);
			// Update document language attribute
			document.documentElement.setAttribute('lang', code);
		},
		[i18n]
	);

	// Initialize Application function
	const initializeRoot = useCallback(async () => {
		console.log('Initializing application...');

		// Initialize application global variable
		await initializeApp({navigate});

		// Initialize authentication
		const response = await getAccountInfo().catch(() => undefined);

		// Update state based on authentication result
		window.application.initialized = true;
		setInitialized(true);
		setAccount(response?.data);
		setLoggedIn(!!response?.data);
	}, [t, navigate, changeLanguage]);

	const refreshAccount = useCallback(async () => {
		const response = await getAccountInfo().catch(() => undefined);
		setAccount(response?.data);
		setLoggedIn(!!response?.data);
	}, []);

	// Authenticate account function (saves token and refreshes account info)
	const authenticateAccount = useCallback(async (authData: AuthOutputDto) => {
		console.log('Authenticating account in context...', authData);
		// Save the authentication token to localStorage
		LocalStorageService.setItem('accessToken', authData);
		// Fetch and set the full account info
		const response = await getAccountInfo().catch(() => undefined);
		setAccount(response?.data);
		setLoggedIn(!!response?.data);
	}, []);

	/** Remove authentication function */
	const removeAuthentication = useCallback(() => {
		logoutAccount().then(null).catch(() => undefined);
		setLoggedIn(false);
		setAccount(undefined);
	}, []);

	return {
		initialized,
		loggedIn,
		account,
		authenticateAccount,
		refreshAccount,
		removeAuthentication,
		t,
		languageCode,
		changeLanguage,
		navigate,
	};
}

export default useRoot;
