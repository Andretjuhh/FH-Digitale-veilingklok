/**
 * Log level types for categorizing log messages
 * - info: General informational messages
 * - warn: Warning messages for potential issues
 * - error: Error messages for failures
 * - debug: Detailed debugging information
 */
type LogLevel = 'info' | 'warn' | 'error' | 'debug';

/**
 * Logger class for handling application logging with environment-aware behavior
 * Automatically adjusts logging output based on ENV:
 * - Production: Only logs errors
 * - Development/Staging: Logs all levels (info, warn, error, debug)
 *
 * @example
 * logger.info('User logged in', { userId: 123 });
 * logger.error('Database connection failed', error);
 * logger.debug('Processing payment', { amount: 100, currency: 'USD' });
 */
class Logger {
	/**
	 * Flag indicating if the application is clockRunning in production mode
	 * Used to control which log levels are output to console
	 */
	private readonly isProduction: boolean;

	/**
	 * Initializes the Logger and determines the current environment
	 * Sets up the isProduction flag based on ENV
	 */
	constructor() {
		// Check if clockRunning in production environment
		this.isProduction = false;
	}

	/**
	 * Logs informational messages (only in non-production environments)
	 * Use for general application flow information
	 *
	 * @param message - The informational message to log
	 * @param optionalParams - Additional parameters to log (objects, values, etc.)
	 * @example
	 * logger.info('Server started', { port: 3000 });
	 */
	public info(message: string, ...optionalParams: unknown[]): void {
		// Only log info messages in development/staging environments
		if (!this.isProduction) {
			console.log(`[INFO] ${message}`, ...optionalParams);
		}
	}

	/**
	 * Logs warning messages (only in non-production environments)
	 * Use for potential issues that don't prevent execution
	 *
	 * @param message - The warning message to log
	 * @param optionalParams - Additional parameters to log (context, values, etc.)
	 * @example
	 * logger.warn('API rate limit approaching', { remaining: 10 });
	 */
	public warn(message: string, ...optionalParams: unknown[]): void {
		// Only log warnings in development/staging environments
		if (!this.isProduction) {
			console.warn(`[WARN] ${message}`, ...optionalParams);
		}
	}

	/**
	 * Logs error messages (ALWAYS logged, even in production)
	 * Use for errors, exceptions, and critical failures
	 *
	 * @param message - The error message to log
	 * @param optionalParams - Additional parameters to log (error objects, stack traces, context)
	 * @example
	 * logger.error('Failed to connect to database', error);
	 */
	public error(message: string, ...optionalParams: unknown[]): void {
		// Always log errors regardless of environment (critical information)
		console.error(`[ERROR] ${message}`, ...optionalParams);
	}

	/**
	 * Logs debug messages for detailed troubleshooting (only in non-production environments)
	 * Use for detailed technical information during development
	 *
	 * @param message - The debug message to log
	 * @param optionalParams - Additional parameters to log (detailed state, variables, etc.)
	 * @example
	 * logger.debug('Processing request', { headers, body, query });
	 */
	public debug(message: string, ...optionalParams: unknown[]): void {
		// Only log debug messages in development/staging environments
		if (!this.isProduction) {
			console.debug(`[DEBUG] ${message}`, ...optionalParams);
		}
	}

	/**
	 * Generic log method that delegates to specific log level methods
	 * Provides a unified interface for logging at different levels
	 *
	 * @param level - The log level (info, warn, error, or debug)
	 * @param message - The message to log
	 * @param optionalParams - Additional parameters to log
	 * @example
	 * logger.log('info', 'Application started');
	 * logger.log('error', 'Connection failed', error);
	 */
	public log(level: LogLevel, message: string, ...optionalParams: unknown[]): void {
		// Delegate to the appropriate log method based on level
		switch (level) {
			case 'info':
				// Log informational message
				this.info(message, ...optionalParams);
				break;
			case 'warn':
				// Log warning message
				this.warn(message, ...optionalParams);
				break;
			case 'error':
				// Log error message
				this.error(message, ...optionalParams);
				break;
			case 'debug':
				// Log debug message
				this.debug(message, ...optionalParams);
				break;
			default:
				// Default to info level for unknown log levels
				this.info(message, ...optionalParams);
		}
	}
}

/**
 * Singleton logger instance for use throughout the application
 * Import and use this instance instead of creating new Logger instances
 *
 * @example
 * import { logger } from './utils/logger';
 * logger.info('Application started');
 */
const logger = new Logger();

export default logger;
