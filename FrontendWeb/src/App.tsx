// External imports
import React from 'react';
import {Route, Routes, useLocation} from 'react-router-dom';
import {AnimatePresence} from 'framer-motion';

// Internal imports
import RootContext from './components/contexts/RootContext';

import Home from './pages/general/Home';
import Login from './pages/general/Login';
import Register from './pages/general/Register';
import Settings from './pages/general/Settings';
import KwekerDashboard from './pages/kweker/KwekerDashboard';
import ProductDetails from './pages/kweker/ProductDetails';
import VeilingMeesterDashboard from './pages/meester/VeilingMeesterDashboard';
import KoperDashboard from './pages/koper/KoperDashboard';

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<AnimatePresence initial={false}>
				<Routes location={location} key={location.pathname}>
					<Route path="/" element={<Home/>}/>
					<Route path="/login" element={<Login/>}/>
					<Route path="/register" element={<Register/>}/>
					<Route path="/settings" element={<Settings/>}/>
					<Route path="/veilingmeester/dashboard" element={<VeilingMeesterDashboard/>}/>
					<Route path="/koper/dashboard" element={<KoperDashboard/>}/>
					<Route path="/kweker/dashboard" element={<KwekerDashboard/>}/>
					<Route path="/kweker/product/:id" element={<ProductDetails/>}/>
				</Routes>
			</AnimatePresence>
		</RootContext>
	);
}

export default App;
