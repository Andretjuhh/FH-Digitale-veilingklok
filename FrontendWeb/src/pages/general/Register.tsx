// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import RegisterContent from '../../components/sections/auth/RegisterContent';

function Register() {
	return (
		<Page enableHeader={false} className="auth-page">
			<RegisterContent/>
		</Page>
	);
}

export default Register;
