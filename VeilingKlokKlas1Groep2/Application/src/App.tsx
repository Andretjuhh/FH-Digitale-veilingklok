// External imports
import React from 'react';
import { Routes, Route, useLocation } from 'react-router-dom';

// Internal imports
import RootContext from './contexts/RootContext';
import Home from './pages/general/Home';
import Login from './pages/general/Login';
import Register from './pages/general/Register';
import './styles/app.css';

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<Routes location={location} key={location.pathname}>
				<Route path="/" element={<Home />} />
				<Route path="/login" element={<Login />} />
				<Route path="/register" element={<Register />} />
			</Routes>
		</RootContext>
	);
}

export default App;
