// PaginatedOutputDto.ts
export interface PaginatedOutputDto<T> {
	page: number;
	limit: number;
	totalCount: number;
	data: T[];
}
