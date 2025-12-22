// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import LoginContent from '../../components/sections/auth/LoginContent';

function Login() {
	return (
		<Page enableHeader={false} className="auth-page">
			<LoginContent/>
		</Page>
	);
}

export default Login;
