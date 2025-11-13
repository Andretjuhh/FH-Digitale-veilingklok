// Internal exports
import CONFIG from '../constant/application';
import { FetchError } from '../types/FetchError';

/** Make an application fetch request */
export async function appFetch(request: RequestInfo | URL, options: RequestInit = {}) {
	// Is app fetch
	let isAppFetch = false;

	// Check if the request url start with / if yes add the API_URL
	if (typeof request === 'string' && request.startsWith('/')) {
		request = new URL(request, CONFIG.API);
		isAppFetch = true;
	}

	// Set default options for proper cookie handling
	const defaultOptions: RequestInit = isAppFetch
		? {
				method: 'GET',
				credentials: 'include', // Include cookies in the request
				headers: {
					'Content-Type': 'application/json',
					Accept: 'application/json',
					Authorization: `Bearer ${localStorage.getItem('accessToken') || ''}`,
				},
		  }
		: {};

	// Merge with user options
	const mergedOptions: RequestInit = {
		...defaultOptions,
		...options,
		headers: {
			...defaultOptions.headers,
			...(options.headers || {}),
		},
	};

	// Handle API request method from REACT-CROSS-FETCH
	return fetch(request, mergedOptions);
}

/** Handle HTTPS request requestResponse */
export async function handleResponse<GeneticResponse = any, GeneticError = any>(requestResponse: Response) {
	// Check if the input is a valid Response object
	if (!(requestResponse instanceof Response)) {
		throw new Error('Invalid argument: requestResponse must be an instance of Response');
	}

	// // Check for the x-new-token header in the requestResponse
	// if (requestResponse.headers.has('x-authentication')) {
	// 	const newAccessToken = requestResponse.headers.get('auth-token');
	// 	LocalStorage.setItem('auth_access_token', newAccessToken || '', 'persistent');
	// }

	// Get the content type from the requestResponse headers
	const contentType = requestResponse.headers.get('content-type');

	// Check if the requestResponse status indicates success
	if (requestResponse.ok) {
		// If the requestResponse is OK, parse based on content type
		if (contentType?.includes('application/json')) {
			try {
				// Return the parsed JSON requestResponse
				return (await requestResponse.json()) as Promise<GeneticResponse>;
			} catch (error: any) {
				throw new Error(`Error parsing JSON: ${error.message}`);
			}
		} else {
			// Handle non-JSON requestResponse types
			return (await requestResponse.text()) as unknown as Promise<GeneticResponse>;
		}
	} else {
		// Read the error message from the requestResponse body
		let fetchError: FetchError<GeneticError> = {
			name: 'FetchError',
			statusCode: requestResponse.status,
		};

		// Attempt to parse the error requestResponse as JSON
		try {
			fetchError.data = (await requestResponse.clone().json()) as GeneticError;
		} catch {
			fetchError.data = await requestResponse.clone().text();
		}

		throw fetchError;
	}
}

/** Fetch and handle HTTPS request requestResponse */
export async function fetchResponse<GeneticResponse = any, GeneticError = any>(request: RequestInfo | URL, options: RequestInit = {}) {
	// Make the API fetch request
	const requestResponse = await appFetch(request, options);

	// Handle the requestResponse
	return handleResponse<GeneticResponse, GeneticError>(requestResponse);
}
