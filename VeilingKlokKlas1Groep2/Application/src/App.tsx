// External imports
import React from 'react';
import { Routes, Route, useLocation } from 'react-router-dom';

// Internal imports
import RootContext from './contexts/RootContext';
import Home from "./pages/Home";
import Login from "./pages/Login";
import KlantDashboard from "./pages/KlantDashboard";
import './styles/app.css';
import './styles/components.header.css';

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<Routes location={location} key={location.pathname}>
				<Route path="/" element={<Home />} />
				<Route path="/login" element={<Login />} />
				<Route path="/KlantDashboard" element={<KlantDashboard />} />
			</Routes>
		</RootContext>
	);
}

export default App;
