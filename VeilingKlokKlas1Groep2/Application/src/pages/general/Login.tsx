// External imports
import React from "react";

// Internal imports
import Page from "../../components/screens/Page";
import LoginContent from "../../components/sections/login/LoginContent";


function Login() {
	return (
		<Page enableHeader = {false}>
			<LoginContent />
		</Page>
	);
}

export default Login;
