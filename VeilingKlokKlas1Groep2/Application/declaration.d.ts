// External imports
import {NavigateFunction} from 'react-router-dom';

// Internal imports
import {LocalizationResources, SupportedLanguages} from './src/controllers/localization';
import {AccountInfo} from "./src/declarations/AccountInfo";

declare global {
	/** Interface representing the global application state
	 *
	 * @property {boolean} initialized - Indicates if the application has been initialized
	 * @property {string}
	 * pathname - The current pathname of the application
	 * @property {SupportedLanguages} languageCode - The current language code of the application
	 * @property {NavigateFunction} navigate - Function to programmatically navigate within the application
	 * @property {Pick<AccountInfo, 'email' | 'accountType'>} [account] - Account authentication state
	 * */
	interface Application {
		initialized: boolean; // Indicates if the application has been initialized
		pathname: string;
		languageCode: SupportedLanguages;

		/** Function to programmatically navigate within the application */
		navigate: NavigateFunction;

		/** Account authentication state */
		account?: AccountInfo;
	}

	interface Window {
		application: Application;
	}
}

// This is making sure that the i18next module recognizes our custom types in our translation files
declare module 'i18next' {
	interface CustomTypeOptions {
		defaultNS: 'translation';
		resources: LocalizationResources;
	}
}
