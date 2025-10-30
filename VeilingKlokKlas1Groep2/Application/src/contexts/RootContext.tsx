// External imports
import React from 'react';

// Internal imports
import useRoot from "../hooks/useRoot";

type RootContextProps = {
	initialized: boolean;
	loggedIn: boolean;
	setLoggedIn: (b: boolean) => void;
}

const RootContext = React.createContext<RootContextProps>({
	initialized: false,
	loggedIn: false,
	setLoggedIn: () => {
	}
});

function RootContextProvider({children}: { children: React.ReactNode }) {
	const rootData = useRoot();
	return (
		<RootContext.Provider value={rootData}>
			{rootData.initialized && children}
		</RootContext.Provider>
	);
}

function useRootContext() {
	const context = React.useContext(RootContext);
	if (!context) {
		throw new Error("useRootContext must be used within a RootContextProvider");
	}
	return context;
}

export {useRootContext};
export default RootContextProvider;