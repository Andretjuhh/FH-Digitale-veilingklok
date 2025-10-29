// External imports
import React from 'react';
import {Routes, Route, useLocation} from 'react-router-dom';

// Internal imports
import RootContext from './contexts/RootContext';
import Home from './pages/general/Home';
import UserDashboard from './pages/user/UserDashboard';

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<Routes location={location} key={location.pathname}>
				<Route path="/" element={<Home/>}/>
				<Route path="/user-dashboard" element={<UserDashboard/>}/>
			</Routes>
		</RootContext>
	);
}

export default App;
