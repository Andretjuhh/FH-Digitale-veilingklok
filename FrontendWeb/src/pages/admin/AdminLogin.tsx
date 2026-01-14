// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import AdminLoginContent from '../../components/sections/admin/AdminLoginContent';

function AdminLogin() {
	return (
		<Page enableHeader={false} className="auth-page">
			<AdminLoginContent/>
		</Page>
	);
}

export default AdminLogin;
