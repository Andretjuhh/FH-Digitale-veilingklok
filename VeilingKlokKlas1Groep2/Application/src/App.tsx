// External imports
import React from 'react';
import { Routes, Route, useNavigate, useLocation } from 'react-router-dom';

// Internal imports
import Home from './pages/Home';

function App() {
	const location = useLocation();

	return (
		<Routes location={location} key={location.pathname}>
			<Route path="/" element={<Home />} />
		</Routes>
	);
}

export default App;
