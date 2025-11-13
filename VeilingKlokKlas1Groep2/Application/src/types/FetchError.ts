export interface FetchError<T> {
	name?: string;
	statusCode: number;
	data?: string | T;
}
