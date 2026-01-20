// External imports
import React from 'react';
import {TFunction} from 'i18next';
import {NavigateFunction} from 'react-router-dom';

// Internal imports
import useRoot from '../../hooks/useRoot';
import {SupportedLanguages} from '../../controllers/services/localization';
import {AccountOutputDto} from "../../declarations/dtos/output/AccountOutputDto";
import {AuthOutputDto} from "../../declarations/dtos/output/AuthOutputDto";

type RootContextProps = {
	initialized: boolean;
	loggedIn: boolean;
	account: AccountOutputDto | undefined;

	/** Current application language code */
	languageCode: SupportedLanguages;
	t: TFunction<'translation', undefined>;

	/** Authenticate and store account information */
	authenticateAccount: (account: AuthOutputDto) => void;
	refreshAccount: () => Promise<void> | void;
	removeAuthentication: () => void;

	changeLanguage: (code: SupportedLanguages) => Promise<void> | void;
	navigate: NavigateFunction;
};

const RootContext = React.createContext<RootContextProps>({
	initialized: false,
	loggedIn: false,
	languageCode: 'nl',
	t: (() => '') as TFunction<'translation', undefined>,
	account: undefined,

	authenticateAccount: () => undefined,
	refreshAccount: () => Promise.resolve(),
	removeAuthentication: () => undefined,
	changeLanguage: () => Promise.resolve(),
	navigate: () => undefined,
});

function RootContextProvider({children}: { children: React.ReactNode }) {
	const rootData = useRoot();
	return <RootContext.Provider value={rootData}>{rootData.initialized && children}</RootContext.Provider>;
}

function useRootContext() {
	const context = React.useContext(RootContext);
	if (!context) {
		throw new Error('useRootContext must be used within a RootContextProvider');
	}
	return context;
}

export {useRootContext};
export default RootContextProvider;
