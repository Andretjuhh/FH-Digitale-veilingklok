import React, { useEffect, useMemo, useRef, useState } from 'react';
import { OnFetchHandlerParams, Pagination } from './Table';
import Spinner from '../elements/Spinner';
import clsx from 'clsx';
import { joinClsx } from '../../utils/classPrefixer';

export interface SortConfig {
	key: string | null;
	direction: 'asc' | 'desc';
}

export interface DataGridProps<T> extends React.HTMLAttributes<HTMLDivElement> {
	data: T[];
	renderItem: (item: T, index: number) => React.JSX.Element;
	renderFilterButtons?: () => React.JSX.Element;

	icon?: React.JSX.Element;

	title: string;
	emptyText: string;
	itemsPerPage?: number;
	totalItems?: number;

	isLazy?: boolean;
	loading?: boolean;
	onFetchData?: (params: OnFetchHandlerParams) => void | Promise<void>;
}

function GridTable<T>(props: DataGridProps<T>): React.JSX.Element {
	const {
		icon,
		data,
		className,
		title,
		itemsPerPage = 10,
		totalItems: externalTotalItems,
		loading = false,
		isLazy = false,
		emptyText,
		onFetchData,
		renderItem,
		renderFilterButtons,
		...rest
	} = props;

	const [currentPage, setCurrentPage] = useState<number>(1);
	const [searchTerm, setSearchTerm] = useState<string>('');
	const [debouncedSearchTerm, setDebouncedSearchTerm] = useState<string>('');
	const [sortConfig, setSortConfig] = useState<SortConfig>({ key: null, direction: 'asc' });
	const lastFetchedParams = useRef<string>('');

	const totalItems = isLazy ? externalTotalItems || 0 : data.length;
	const totalPages = Math.ceil(totalItems / itemsPerPage);

	// Debounce search term
	useEffect(() => {
		const timer = setTimeout(() => {
			setDebouncedSearchTerm(searchTerm);
		}, 500);
		return () => clearTimeout(timer);
	}, [searchTerm]);

	// Reset to page 1 when search term changes
	useEffect(() => {
		setCurrentPage(1);
	}, [debouncedSearchTerm]);

	useEffect(() => {
		if (isLazy && onFetchData) {
			const params = { page: currentPage, pageSize: itemsPerPage, searchTerm: debouncedSearchTerm, sortConfig };
			const paramsString = JSON.stringify(params);

			// Only fetch if params have actually changed to avoid redundant calls
			if (paramsString !== lastFetchedParams.current) {
				onFetchData(params);
				lastFetchedParams.current = paramsString;
			}
		}
	}, [isLazy, currentPage, debouncedSearchTerm, sortConfig, onFetchData]);

	const paginatedData = useMemo(() => {
		if (isLazy) return data;
		const startIndex = (currentPage - 1) * itemsPerPage;
		return data.slice(startIndex, startIndex + itemsPerPage);
	}, [data, data, currentPage, itemsPerPage, isLazy]);

	const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
		setSearchTerm(e.target.value);
	};

	return (
		<section className={clsx('app-grid-table', className)} {...rest}>
			<div className={'app-grid-lines'} />
			{loading && (
				<div className="app-table-loading-overlay">
					<Spinner className="app-table-loading-spinner" />
				</div>
			)}
			<div className={clsx('app-grid-table-header', joinClsx(className, 'header'))}>
				<div className={'app-table-actions-row'}>
					<div className="app-table-title-row">
						{icon}
						<h2 className="app-table-title">{title}</h2>
					</div>
					<div className="app-table-search-wrapper">
						<i className="bi bi-search app-table-search-icon"></i>
						<input type="text" placeholder="Search..." value={searchTerm} onChange={handleSearch} className="app-table-search-input" />
					</div>
					<div className="app-table-filter-group">{renderFilterButtons?.()}</div>
				</div>
			</div>

			<div className={clsx('app-table-wrapper', joinClsx(className, 'wrapper'))}>
				<div className={clsx('app-grid-table-content', joinClsx(className, 'content'))}>
					{paginatedData.map((item, index) => (
						<div key={index} className="app-grid-table-item">
							{renderItem(item, index)}
						</div>
					))}

					{paginatedData.length === 0 && !loading && (
						<div className="app-table-empty-state">
							<p>{emptyText}</p>
						</div>
					)}
				</div>
			</div>

			<Pagination
				currentPage={currentPage}
				totalPages={totalPages}
				onPageChange={setCurrentPage}
				itemsPerPage={itemsPerPage}
				totalItems={totalItems}
			/>
		</section>
	);
}

export default GridTable;
