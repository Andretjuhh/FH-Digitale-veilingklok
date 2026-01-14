const config = {
	VERSION: '1.0.0',
	API: 'http://localhost:5219/',
	KLOK_HUB_URL: 'http://localhost:5219/hubs/veiling-klok',

	X_ACCESS_TOKEN: 'X-Access-Token',
	AUTH_OBJECT_KEY: 'auth_object',
	AUTH_ACCESS_TOKEN_KEY: 'auth_access_token',
} as const;

export default config;
