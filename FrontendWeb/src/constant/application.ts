const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

if (!API_BASE_URL) {
	throw new Error('REACT_APP_API_BASE_URL is not defined');
}

const config = {
	VERSION: '1.0.0',

	API: API_BASE_URL,
	KLOK_HUB_URL: `${API_BASE_URL}/hubs/veiling-klok`,

	X_ACCESS_TOKEN: 'X-Access-Token',
	AUTH_OBJECT_KEY: 'auth_object',
	AUTH_ACCESS_TOKEN_KEY: 'auth_access_token',
} as const;

export default config;
