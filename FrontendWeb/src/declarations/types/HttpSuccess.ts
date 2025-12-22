 
export interface HttpSuccess<TData = any> {
	success: boolean;
	message: string;
	data: TData;
}
