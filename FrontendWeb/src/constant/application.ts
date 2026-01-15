const apiBase =
	process.env.REACT_APP_API_URL ??
	'webappklas1groep2-g2gtftgfgdg3fzcc.germanywestcentral-01.azurewebsites.net';

const config = {
	VERSION: '1.0.0',
	API: apiBase,
	KLOK_HUB_URL: 'http://localhost:5219/hubs/veiling-klok',

	X_ACCESS_TOKEN: 'X-Access-Token',
	AUTH_OBJECT_KEY: 'auth_object',
	AUTH_ACCESS_TOKEN_KEY: 'auth_access_token',
} as const;

export default config;
