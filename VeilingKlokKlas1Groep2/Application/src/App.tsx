// External imports
import React from 'react';
import {Route, Routes, useLocation} from 'react-router-dom';
import {AnimatePresence} from 'framer-motion';

// Internal imports
import RootContext from './contexts/RootContext';

import Home from './pages/general/Home';
import Login from './pages/general/Login';
import KlantDashboard from './pages/KlantDashboard';
import Register from './pages/general/Register';
import KwekerDashboard from './pages/Kweker/KwekerDashboard';
import ProductDetails from './pages/Kweker/ProductDetails';
import Dashboard from './pages/general/Dashboard';
import UserDashboard from './pages/user/UserDashboard';

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<AnimatePresence initial={false}>
				<Routes location={location} key={location.pathname}>
					<Route path="/" element={<Home/>}/>
					<Route path="/login" element={<Login/>}/>
					<Route path="/register" element={<Register/>}/>
					<Route path="/dashboard" element={<Dashboard/>}/>
					<Route path="/KlantDashboard" element={<KlantDashboard/>}/>
					<Route path="/user-dashboard" element={<UserDashboard/>}/>
					<Route path="/kweker" element={<KwekerDashboard/>}/>
					<Route path="/kweker/product/:id" element={<ProductDetails/>}/>
				</Routes>
			</AnimatePresence>
		</RootContext>
	);
}

export default App;
