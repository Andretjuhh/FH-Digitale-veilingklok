import React, {useEffect, useMemo, useRef, useState} from 'react';
import Spinner from '../ui/Spinner';
import Button from "../buttons/Button";

export interface Column<T> {
	key: keyof T | string;
	label: string;
	sortable?: boolean;
	render?: (item: T, onAction?: (item: T) => void) => React.ReactNode;
}

export interface TableHeaderProps<T> {
	columns: Column<T>[];
	onSort: (key: string) => void;
	sortConfig: SortConfig;
}

export interface TableRowProps<T> {
	item: T;
	columns: Column<T>[];
	onAction?: (item: T) => void;
}

export interface PaginationProps {
	currentPage: number;
	totalPages: number;
	onPageChange: (page: number) => void;
	itemsPerPage: number;
	totalItems: number;
}

export interface SortConfig {
	key: string | null;
	direction: 'asc' | 'desc';
}

export interface DataTableProps<T> {
	icon?: React.JSX.Element;
	data: T[];
	title: string;
	emptyText: string;
	columns: Column<T>[];
	itemsPerPage?: number;
	onAction?: (item: T) => void;
	// Lazy loading props
	isLazy?: boolean;
	totalItems?: number;
	loading?: boolean;
	onFetchData?: (params: { page: number; searchTerm: string; sortConfig: SortConfig }) => void | Promise<void>;
}

// Table Header Component
function TableHeader<T>({columns, onSort, sortConfig}: TableHeaderProps<T>): React.JSX.Element {
	return (
		<thead className="app-table-thead">
		<tr>
			{columns.map((column) => (
				<th key={String(column.key)} className="app-table-th" onClick={() => column.sortable && onSort(String(column.key))}>
					<div className="app-table-th-content">
						{column.label}
						{column.sortable &&
							<i className={`bi bi-chevron-${sortConfig.key === column.key && sortConfig.direction === 'desc' ? 'up' : 'down'} app-table-sort-icon`}></i>}
					</div>
				</th>
			))}
		</tr>
		</thead>
	);
}

// Table Row Component
function TableRow<T extends Record<string, any>>({item, columns, onAction}: TableRowProps<T>): React.JSX.Element {
	return (
		<tr className="app-table-tbody-row">
			{columns.map((column) => (
				<td key={String(column.key)} className="app-table-td">
					{column.render ? column.render(item, onAction) : item[column.key as keyof T]}
				</td>
			))}
		</tr>
	);
}

// Pagination Component
export const Pagination: React.FC<PaginationProps> = ({currentPage, totalPages, onPageChange, itemsPerPage, totalItems}) => {
	const startItem = (currentPage - 1) * itemsPerPage + 1;
	const endItem = Math.min(currentPage * itemsPerPage, totalItems);
	return (
		<div className="app-table-pagination">
			<div className="app-table-pagination-info">
				Showing {startItem} to {endItem} of {totalItems} entries
			</div>
			<div className="app-table-pagination-controls">
				<button onClick={() => onPageChange(currentPage - 1)} disabled={currentPage === 1} className="app-table-pagination-btn">
					<i className="bi bi-chevron-left"></i>
				</button>
				{[...Array(totalPages)].map((_, idx) => (
					<button key={idx} onClick={() => onPageChange(idx + 1)}
					        className={`app-table-pagination-page-btn ${currentPage === idx + 1 ? 'app-table-pagination-page-btn-active' : 'app-table-pagination-page-btn-inactive'}`}>
						{idx + 1}
					</button>
				))}
				<button onClick={() => onPageChange(currentPage + 1)} disabled={currentPage === totalPages} className="app-table-pagination-btn">
					<i className="bi bi-chevron-right"></i>
				</button>
			</div>
		</div>
	);
};

// Main Table Component
export function DataTable<T extends Record<string, any>>(props: DataTableProps<T>): React.JSX.Element {
	const {
		icon,
		data,
		title,
		columns,
		itemsPerPage = 5,
		onAction,
		isLazy = false,
		totalItems: externalTotalItems,
		loading = false,
		emptyText,
		onFetchData
	} = props;
	const [currentPage, setCurrentPage] = useState<number>(1);
	const [searchTerm, setSearchTerm] = useState<string>('');
	const [debouncedSearchTerm, setDebouncedSearchTerm] = useState<string>('');
	const [sortConfig, setSortConfig] = useState<SortConfig>({key: null, direction: 'asc'});
	const lastFetchedParams = useRef<string>('');

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
			const params = {page: currentPage, searchTerm: debouncedSearchTerm, sortConfig};
			const paramsString = JSON.stringify(params);

			// Only fetch if params have actually changed to avoid redundant calls
			if (paramsString !== lastFetchedParams.current) {
				onFetchData(params);
				lastFetchedParams.current = paramsString;
			}
		}
	}, [isLazy, currentPage, debouncedSearchTerm, sortConfig, onFetchData]);

	const filteredData = useMemo(() => {
		if (isLazy) return data;
		return data.filter((item) => Object.values(item).some((value) => String(value).toLowerCase().includes(searchTerm.toLowerCase())));
	}, [data, searchTerm, isLazy]);

	const sortedData = useMemo(() => {
		if (isLazy || !sortConfig.key) return filteredData;

		return [...filteredData].sort((a, b) => {
			const aValue = a[sortConfig.key as keyof T];
			const bValue = b[sortConfig.key as keyof T];

			if (aValue < bValue) {
				return sortConfig.direction === 'asc' ? -1 : 1;
			}
			if (aValue > bValue) {
				return sortConfig.direction === 'asc' ? 1 : -1;
			}
			return 0;
		});
	}, [filteredData, sortConfig, isLazy]);

	const totalItems = isLazy ? externalTotalItems || 0 : sortedData.length;
	const totalPages = Math.ceil(totalItems / itemsPerPage);

	const paginatedData = useMemo(() => {
		if (isLazy) return data;
		const startIndex = (currentPage - 1) * itemsPerPage;
		return sortedData.slice(startIndex, startIndex + itemsPerPage);
	}, [data, sortedData, currentPage, itemsPerPage, isLazy]);

	const handleSort = (key: string): void => {
		setSortConfig((prev) => ({
			key,
			direction: prev.key === key && prev.direction === 'asc' ? 'desc' : 'asc',
		}));
		setCurrentPage(1);
	};

	const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
		setSearchTerm(e.target.value);
	};

	return (
		<div className="app-table-container">
			{
				loading && (<div className="app-table-loading-overlay">
					<Spinner className="app-table-loading-spinner"/>
				</div>)
			}
			<div className="app-table-header-actions">
				<div className="app-table-actions-row">
					<div className="app-table-title-row">
						{icon}
						<h2 className="app-table-title">{title}</h2>
					</div>
					<div className="app-table-search-wrapper">
						<i className="bi bi-search app-table-search-icon"></i>
						<input type="text" placeholder="Search..." value={searchTerm} onChange={handleSearch} className="app-table-search-input"/>
					</div>
					<div className="app-table-filter-group">
						<Button
							icon="bi-chevron-down"
							className="app-table-filter-btn"
							label={'All Status'}
						/>
						<Button
							icon="bi-chevron-down"
							className="app-table-filter-btn"
							label={'More Filters'}
						/>
					</div>
				</div>
			</div>

			<div className="app-table-wrapper">
				<table className="app-table">
					<TableHeader columns={columns} onSort={handleSort} sortConfig={sortConfig}/>
					<tbody className="app-table-tbody">
					{paginatedData.map((item, index) => (
						<TableRow key={item.id || index} item={item} columns={columns} onAction={onAction}/>
					))}
					{paginatedData.length === 0 && !loading && (
						<tr>
							<td colSpan={columns.length} className="app-table-empty-state">
								{emptyText}
							</td>
						</tr>
					)}
					</tbody>
				</table>
			</div>

			<Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={setCurrentPage} itemsPerPage={itemsPerPage} totalItems={totalItems}/>
		</div>
	);
}

