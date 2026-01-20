import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useRootContext } from '../contexts/RootContext';
import { AccountType } from '../../declarations/enums/AccountTypes';

interface ProtectedRouteProps {
	children: React.JSX.Element;
	allowedAccountTypes?: AccountType[];
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, allowedAccountTypes }) => {
	const { loggedIn, account } = useRootContext();
	const location = useLocation();

	// If not logged in, redirect to login page
	if (!loggedIn || !account) {
		// allow /admin to redirect to /admin instead of /login if needed, but for now /login is standard
		// or maybe passing the return url
		return <Navigate to="/login" state={{ from: location }} replace />;
	}

	// If logged in but account type is restricted and doesn't match
	if (allowedAccountTypes && allowedAccountTypes.length > 0) {
		if (!allowedAccountTypes.includes(account.accountType as AccountType)) {
			// Redirect to home if unauthorized for this specific page
			return <Navigate to="/" replace />;
		}
	}

	return children;
};

export default ProtectedRoute;
