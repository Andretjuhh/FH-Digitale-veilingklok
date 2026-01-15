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

// Veilingmeester Pages
import VeilingmeesterKlokManage from './pages/meester/VeilingmeesterKlokManage';

import VeilingmeesterKlokDetails from './pages/meester/VeilingmeesterKlokDetails';
import VeilingmeesterVeilingen from './pages/meester/VeilingmeesterKlokken';
import VeilingmeesterProducts from './pages/meester/VeilingmeesterProducts';

// Koper Pages
import KoperVeilingKlokken from './pages/koper/KoperVeilingKlokken';
import AdminLogin from './pages/admin/AdminLogin';
import AdminDashboard from './pages/admin/AdminDashboard';

// Kweker Pages
import KwekerDashboard from './pages/kweker/KwekerDashboard';
import KwekerOrders from './pages/kweker/KwekerOrders';
import KwekerProducts from './pages/kweker/KwekerProducts';
import ProductDetails from './pages/kweker/ProductDetails';
import VeilingmeesterManageHome from './pages/meester/VeilingmeesterHome';
import KoperVeilingKlok from "./pages/koper/KoperVeilingKlok";
import KoperProducts from "./pages/koper/KoperProducts";
import KoperOrders from "./pages/koper/KoperOrders";

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<AnimatePresence initial={false}>
				<Routes location={location} key={location.pathname}>
					{/*Root Routes*/}
					<Route path="/" element={<Home/>}/>

					{/*Authentication Routes*/}
					<Route path="/login" element={<Login/>}/>
					<Route path="/register" element={<Register/>}/>
					<Route path="/settings" element={<Settings/>}/>

					{/*Veilingmeester Routes*/}
					<Route path="/veilingmeester/veilingen-beheren" element={<VeilingmeesterManageHome/>}/>
					<Route path="/veilingmeester/veilingen-beheren/:klokId" element={<VeilingmeesterKlokManage/>}/>
					<Route path="/veilingmeester/veilingen" element={<VeilingmeesterVeilingen/>}/>
					<Route path="/veilingmeester/veilingen/:klokId" element={<VeilingmeesterKlokDetails/>}/>
					<Route path="/veilingmeester/region-flowers" element={<VeilingmeesterProducts/>}/>

					{/*Koper Routes*/}
					<Route path="/koper/veilingen" element={<KoperVeilingKlokken/>}/>
					<Route path="/koper/veilingen/:id" element={<KoperVeilingKlok/>}/>
					<Route path="/koper/zoeken" element={<KoperProducts/>}/>
					<Route path="/koper/orders" element={<KoperOrders/>}/>

					{/*Kweker Routes*/}
					<Route path="/kweker/dashboard" element={<KwekerDashboard/>}/>
					<Route path="/kweker/products" element={<KwekerProducts/>}/>
					<Route path="/kweker/orders" element={<KwekerOrders/>}/>
					<Route path="/kweker/product/:id" element={<ProductDetails/>}/>

					{/*Admin Routes*/}
					<Route path="/admin" element={<AdminLogin/>}/>
					<Route path="/admin/dashboard" element={<AdminDashboard/>}/>
				</Routes>
			</AnimatePresence>
		</RootContext>
	);
}

export default App;
