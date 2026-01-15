// External imports
import React from 'react';
import { Route, Routes, useLocation } from 'react-router-dom';
import { AnimatePresence } from 'framer-motion';

// Internal imports
import RootContext from './components/contexts/RootContext';
import ProtectedRoute from './components/nav/ProtectedRoute';
import { AccountType } from './declarations/enums/AccountTypes';

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
import KoperVeilingKlok from './pages/koper/KoperVeilingKlok';
import KoperProducts from './pages/koper/KoperProducts';
import KoperOrders from './pages/koper/KoperOrders';

function App() {
	const location = useLocation();

	return (
		<RootContext>
			<AnimatePresence initial={false}>
				<Routes location={location} key={location.pathname}>
					{/*Root Routes*/}
					<Route path="/" element={<Home />} />

					{/*Authentication Routes*/}
					<Route path="/login" element={<Login />} />
					<Route path="/register" element={<Register />} />
					<Route
						path="/settings"
						element={
							<ProtectedRoute>
								<Settings />
							</ProtectedRoute>
						}
					/>

					{/*Veilingmeester Routes*/}
					<Route
						path="/veilingmeester/veilingen-beheren"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Veilingmeester]}>
								<VeilingmeesterManageHome />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/veilingmeester/veilingen-beheren/:klokId"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Veilingmeester]}>
								<VeilingmeesterKlokManage />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/veilingmeester/veilingen"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Veilingmeester]}>
								<VeilingmeesterVeilingen />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/veilingmeester/veilingen/:klokId"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Veilingmeester]}>
								<VeilingmeesterKlokDetails />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/veilingmeester/region-flowers"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Veilingmeester]}>
								<VeilingmeesterProducts />
							</ProtectedRoute>
						}
					/>

					{/*Koper Routes*/}
					<Route
						path="/koper/veilingen"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Koper]}>
								<KoperVeilingKlokken />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/koper/veilingen/:klokId"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Koper]}>
								<KoperVeilingKlok />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/koper/zoeken"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Koper]}>
								<KoperProducts />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/koper/orders"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Koper]}>
								<KoperOrders />
							</ProtectedRoute>
						}
					/>

					{/*Kweker Routes*/}
					<Route
						path="/kweker/dashboard"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Kweker]}>
								<KwekerDashboard />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/kweker/products"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Kweker]}>
								<KwekerProducts />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/kweker/orders"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Kweker]}>
								<KwekerOrders />
							</ProtectedRoute>
						}
					/>
					<Route
						path="/kweker/product/:id"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Kweker]}>
								<ProductDetails />
							</ProtectedRoute>
						}
					/>

					{/*Admin Routes*/}
					<Route path="/admin" element={<AdminLogin />} />
					<Route
						path="/admin/dashboard"
						element={
							<ProtectedRoute allowedAccountTypes={[AccountType.Admin]}>
								<AdminDashboard />
							</ProtectedRoute>
						}
					/>
				</Routes>
			</AnimatePresence>
		</RootContext>
	);
}

export default App;
