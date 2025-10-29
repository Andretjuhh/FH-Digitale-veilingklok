// External imports
import {TFunction} from 'i18next';
import {NavigateFunction} from "react-router-dom";

// Internal imports
import {SupportedLanguages, LocalizationResources} from "./src/controllers/localization";

declare global {
	interface Application {
		initialized: boolean; // Indicates if the application has been initialized
		t: TFunction; // Assuming t is a function that translates a key to a string
		navigate: NavigateFunction; // Function to navigate to a different page
		pathname: string;
		languageCode: SupportedLanguages;
		changeLanguage: (code: SupportedLanguages) => Promise<void> | void;
	}

	interface Window {
		application: Application;
	}
}

// This is making sure that the i18next module recognizes our custom types in our translation files
declare module "i18next" {
	interface CustomTypeOptions {
		defaultNS: "translation";
		resources: LocalizationResources;
	}
}
