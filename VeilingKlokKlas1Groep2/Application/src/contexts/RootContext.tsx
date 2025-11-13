// External imports
import React from 'react';
import { TFunction } from 'i18next';
import { NavigateFunction } from 'react-router-dom';

// Internal imports
import useRoot from '../hooks/useRoot';
import { SupportedLanguages } from '../controllers/localization';
import { LoginRequest } from '../declarations/LoginRequest';
import { AccountInfo } from '../declarations/AccountInfo';

type RootContextProps = {
	initialized: boolean;

	loggedIn: boolean;
	account: AccountInfo | undefined;

	languageCode: SupportedLanguages;
	t: TFunction<'translation', undefined>;

	authenticate: (account: LoginRequest) => Promise<void>;
	changeLanguage: (code: SupportedLanguages) => Promise<void> | void;
	navigate: NavigateFunction;
};

const RootContext = React.createContext<RootContextProps>({
	initialized: false,
	loggedIn: false,
	languageCode: 'nl',
	t: (() => '') as TFunction<'translation', undefined>,
	account: undefined,
	authenticate: () => Promise.resolve(),
	changeLanguage: () => Promise.resolve(),
	navigate: () => undefined,
});

function RootContextProvider({ children }: { children: React.ReactNode }) {
	const rootData = useRoot();
	console.log({ ...rootData });
	return <RootContext.Provider value={rootData}>{rootData.initialized && children}</RootContext.Provider>;
}

function useRootContext() {
	const context = React.useContext(RootContext);
	if (!context) {
		throw new Error('useRootContext must be used within a RootContextProvider');
	}
	return context;
}

export { useRootContext };
export default RootContextProvider;
