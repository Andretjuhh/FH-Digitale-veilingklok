const apiBase =
	process.env.REACT_APP_API_URL ??
	(typeof window !== 'undefined' ? `${window.location.origin}/` : 'http://localhost:5219/');

const config = {
	VERSION: '1.0.0',
	API: apiBase,

	X_ACCESS_TOKEN: 'X-Access-Token',
	AUTH_OBJECT_KEY: 'auth_object',
	AUTH_ACCESS_TOKEN_KEY: 'auth_access_token',
} as const;

export default config;
