// External imports
import { useCallback, useLayoutEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

// Internal imports
import { SupportedLanguages, useTranslation } from '../controllers/localization';
import LocalStorage from '../controllers/localStorage';
import initializeApp from '../controllers/application';
import { initializeAuthentication, loginAccount, logoutAccount, saveAuthenticationResponse } from '../controllers/authentication';
import { AccountInfo } from '../declarations/AccountInfo';
import { LoginRequest } from '../declarations/LoginRequest';
import { AuthResponse } from '../declarations/AuthenticationResponse';

function useRoot() {
	//  Router navigation
	const navigate = useNavigate();

	// Initialize localization
	const { t, i18n } = useTranslation();

	// Application initialization status
	const [initialized, setInitialized] = useState(false);

	// Check if the application is logged in
	const [account, setAccount] = useState<AccountInfo | undefined>();
	const [loggedIn, setLoggedIn] = useState<boolean>(false);

	// Current Application language
	const [languageCode, setLanguageCode] = useState<SupportedLanguages>('nl');

	// Initialize Application & Global application functions
	useLayoutEffect(() => {
		// Initialize Application
		initializeRoot();

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

	// Initialize Application function
	const initializeRoot = useCallback(async () => {
		console.log('Initializing application...');

		// Initialize application global variable
		await initializeApp({ navigate });

		// Initialize authentication
		const loggedUser = await initializeAuthentication().catch(() => undefined);

		// Update state based on authentication result
		window.application.initialized = true;
		setInitialized(true);
		setAccount(loggedUser);
		setLoggedIn(!!loggedUser);
	}, [t, navigate, changeLanguage]);

	// Authenticate account function (stub for future use)
	const authenticateAccount = useCallback(async (account: AuthResponse) => {
		console.log('Authenticating account in context...', account);
		setLoggedIn(true);
		setAccount({ email: account.email, accountType: account.accountType });
		await saveAuthenticationResponse(account);
	}, []);

	// Authenticate user function
	const authenticate = useCallback(async (account: LoginRequest) => {
		console.log('Authenticating account in context...', account);
		// Perform login
		const authResponse = await loginAccount(account);
		// If no error is thrown, the login was successful
		setLoggedIn(true);
		setAccount({ email: authResponse.email, accountType: authResponse.accountType });
		await saveAuthenticationResponse(authResponse);
	}, []);

	/** Remove authentication function */
	const removeAuthentication = useCallback(() => {
		setLoggedIn(false);
		setAccount(undefined);
		logoutAccount();
	}, []);

	return {
		initialized,

		loggedIn,
		account,
		authenticate,
		authenticateAccount,
		removeAuthentication,

		t,
		languageCode,
		changeLanguage,
		navigate,
	};
}

export default useRoot;
