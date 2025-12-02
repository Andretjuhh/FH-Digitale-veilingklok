import { isDevelopment, sanitizeMessage } from '../utils/standards';

export interface ProcessErrorPayload<TErrorDetails = unknown> {
	code: string;
	message: string;
	details?: TErrorDetails;
	expose?: boolean;
	status?: number;
}

/**
 * Custom Process Error class for structured error handling in internal processes
 * @template TErrorDetails - Type for additional error details (can be any type)
 * @extends Error - Built-in JavaScript Error class
 * @example
 * throw new ProcessError({
 *   code: 'VALIDATION_FAILED',
 *   message: 'User data validation failed',
 *   details: { field: 'email', reason: 'invalid format' },
 *   expose: false
 *   status: 400
 * });
 */
export class ProcessError<TErrorDetails = unknown> extends Error {
	/** Unique error code identifier (e.g., 'VALIDATION_ERROR', 'NOT_FOUND') */
	public readonly code: string;

	/** Additional error details with type safety via generics */
	public readonly details?: TErrorDetails;

	/** Whether to expose error details to the client (use false for sensitive errors) */
	public readonly expose: boolean;

	/** Optional HTTP status code associated with the error (e.g., 400, 500) */
	public readonly status?: number;

	/**
	 * Creates a new ProcessError instance
	 * @param payload - Error configuration object containing all error properties
	 */
	constructor(payload: ProcessErrorPayload<TErrorDetails>) {
		// Call parent Error constructor with the error message
		super(sanitizeMessage(payload.message));

		// Set the error name for better stack traces and debugging
		this.name = 'ProcessError';

		// Assign the unique error code
		this.code = payload.code;

		// Assign additional error details
		this.details = payload.details;

		// Set expose flag, defaulting to true to show error details to client
		this.expose = payload.expose ?? isDevelopment();

		// Assign optional HTTP status code
		this.status = payload.status;

		// Capture the stack trace for debugging (maintains proper stack trace in V8 engines)
		Error.captureStackTrace?.(this, ProcessError);
	}
}

/**
 * Type guard to check if an error is an instance of ProcessError
 * Useful for error handling logic to distinguish ProcessError from other error types
 * @param error - The error to check
 * @returns True if the error is a ProcessError instance, false otherwise
 * @example
 * try {
 *   // some code
 * } catch (error) {
 *   if (isProcessError(error)) {
 *     console.log(error.code, error.details);
 *   }
 * }
 */
export const isProcessError = (error: unknown): error is ProcessError => error instanceof ProcessError;
