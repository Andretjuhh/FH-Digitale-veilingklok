// External imports
import React from 'react';
import {Routes, Route, useLocation} from 'react-router-dom';

// Internal imports
import RootContext from './contexts/RootContext';
import Home from './pages/Home';

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<Routes location={location} key={location.pathname}>
				<Route path="/" element={<Home/>}/>
			</Routes>
		</RootContext>
	);
}

export default App;
