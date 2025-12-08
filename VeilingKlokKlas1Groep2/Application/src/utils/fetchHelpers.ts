// Internal exports
import config from '../constant/application';
import { HttpError } from '../types/HttpError';
import { ProcessError } from '../types/ProcessError';

/** Make an application fetch request */
export async function appFetch(request: RequestInfo | URL, options: RequestInit = {}) {
	// Is app fetch
	let isAppFetch = false;

	// Check if the request url start with / if yes add the API_URL
	if (typeof request === 'string' && request.startsWith('/')) {
		request = new URL(request, config.API);
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
		throw new ProcessError({
			code: 'INVALID_RESPONSE',
			message: 'Invalid response',
			details: {
				response: requestResponse,
			},
			expose: true,
			status: 400,
		});
	}

	// Get the content type from the requestResponse headers
	const contentType = requestResponse.headers.get('content-type');

	// Check for the x-new-token header in the requestResponse
	if (requestResponse.headers.has(config.X_ACCESS_TOKEN)) {
		const newAccessToken = requestResponse.headers.get(config.X_ACCESS_TOKEN);
		localStorage.setItem(config.AUTH_ACCESS_TOKEN_KEY, newAccessToken ?? '');
	}

	// Check if the requestResponse status indicates success
	if (requestResponse.ok) {
		// If the requestResponse is OK, parse based on content type
		if (contentType?.includes('application/json')) {
			try {
				// Return the parsed JSON requestResponse
				return (await requestResponse.json()) as Promise<GeneticResponse>;
			} catch (error: any) {
				throw new HttpError({
					code: 'FETCH_JSON_PARSE_ERROR',
					message: error instanceof Error ? `Error parsing JSON: ${error.message}` : 'Error parsing JSON',
					statusCode: 500,
					expose: false,
				});
			}
		} else {
			// Handle non-JSON requestResponse types
			return (await requestResponse.text()) as unknown as Promise<GeneticResponse>;
		}
	}

	// If the requestResponse indicates an error, create an ProcessError
	let fetchError: GeneticError | string;

	// Attempt to parse the error requestResponse as JSON
	try {
		fetchError = (await requestResponse.clone().json()) as GeneticError;
	} catch {
		fetchError = await requestResponse.clone().text();
	}

	// Throw an ProcessError with details from the failed requestResponse
	throw new HttpError({
		code: 'FETCH_REQUEST_ERROR',
		statusCode: requestResponse.status,
		message: typeof fetchError === 'object' && fetchError !== null && 'message' in fetchError ? (fetchError.message as string) : `Fetch request failed with status ${requestResponse.status}: ${requestResponse.statusText}`,
		details: typeof fetchError === 'object' && fetchError !== null && 'details' in fetchError ? (fetchError.details as GeneticError) : fetchError,
		expose: false,
	});
}

/** Fetch and handle HTTPS request requestResponse */
export async function fetchResponse<GeneticResponse = any, GeneticError = any>(request: RequestInfo | URL, options: RequestInit = {}) {
	// Make the API fetch request
	const requestResponse = await appFetch(request, options);

	// Handle the requestResponse
	return handleResponse<GeneticResponse, GeneticError>(requestResponse);
}
