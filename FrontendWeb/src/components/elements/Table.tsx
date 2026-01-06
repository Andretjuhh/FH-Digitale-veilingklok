import React, {useCallback, useEffect, useMemo, useRef, useState} from 'react';
import Spinner from './Spinner';

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

export interface DataTableProps<T> {
	data: T[];
	columns: Column<T>[];
	itemsPerPage?: number;
	onAction?: (item: T) => void;
	// Lazy loading props
	isLazy?: boolean;
	totalItems?: number;
	loading?: boolean;
	onFetchData?: (params: { page: number; searchTerm: string; sortConfig: SortConfig }) => void | Promise<void>;
}

export interface SortConfig {
	key: string | null;
	direction: 'asc' | 'desc';
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
const Pagination: React.FC<PaginationProps> = ({currentPage, totalPages, onPageChange, itemsPerPage, totalItems}) => {
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
export function DataTable<T extends Record<string, any>>({
	                                                         data,
	                                                         columns,
	                                                         itemsPerPage = 5,
	                                                         onAction,
	                                                         isLazy = false,
	                                                         totalItems: externalTotalItems,
	                                                         loading = false,
	                                                         onFetchData
                                                         }: DataTableProps<T>): React.JSX.Element {
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
			{loading && (
				<div className="app-table-loading-overlay">
					<Spinner className="app-table-loading-spinner"/>
				</div>
			)}
			<div className="app-table-header-actions">
				<div className="app-table-actions-row">
					<div className="app-table-search-wrapper">
						<i className="bi bi-search app-table-search-icon"></i>
						<input type="text" placeholder="Search..." value={searchTerm} onChange={handleSearch} className="app-table-search-input"/>
					</div>
					<div className="app-table-filter-group">
						<button className="app-table-filter-btn">
							All Status
							<i className="bi bi-chevron-down"></i>
						</button>
						<button className="app-table-filter-btn">
							<i className="bi bi-filter"></i>
							More Filters
						</button>
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
								No data found
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

// --- Example Usage (Order specific) ---

interface Order {
	id: number;
	itemName: string;
	category: string;
	orderId: string;
	orderDate: string;
	customer: string;
	customerType: string;
	avatar: string;
	price: number;
	paymentMethod: string;
	status: 'Accepted' | 'Completed' | 'Rejected' | 'Pending';
	icon: string;
}

const mockOrders: Order[] = [
	{
		id: 1,
		itemName: 'Choco Chucu',
		category: 'Iced',
		orderId: '#CFFE109283',
		orderDate: '11 Jan 2025, 23:39',
		customer: 'Paistudio',
		customerType: 'Star Customer',
		avatar: '🎨',
		price: 5.0,
		paymentMethod: 'Cash on Delivery',
		status: 'Accepted',
		icon: '🍫',
	},
	{
		id: 2,
		itemName: 'Brocoli Cinno',
		category: 'Iced',
		orderId: '#CFFEB42701',
		orderDate: '11 Jan 2025, 23:39',
		customer: 'Azrul Artistik',
		customerType: 'Star Customer',
		avatar: '👤',
		price: 8.2,
		paymentMethod: 'Paid by Gopay',
		status: 'Completed',
		icon: '🥦',
	},
	{
		id: 3,
		itemName: 'Cinta Kamu',
		category: 'Sweet',
		orderId: '#CKKE397512',
		orderDate: '1 Feb 2025, 23:39',
		customer: 'Raihan Rabka',
		customerType: 'New Customer',
		avatar: '👨',
		price: 6.1,
		paymentMethod: 'Paid by Gopay',
		status: 'Completed',
		icon: '❤️',
	},
	{
		id: 4,
		itemName: 'Choco Chips',
		category: 'Sweet & Salt',
		orderId: '#CKKE650384',
		orderDate: '1 Feb 2025, 23:39',
		customer: 'Tyo Ditya',
		customerType: 'Star Customer',
		avatar: '👔',
		price: 1.0,
		paymentMethod: 'Paid by OVO',
		status: 'Accepted',
		icon: '🍪',
	},
	{
		id: 5,
		itemName: 'Rumput Laut Latte',
		category: 'Hot',
		orderId: '#CFFE204971',
		orderDate: '1 Feb 2025, 23:39',
		customer: 'Noval Amarul',
		customerType: 'New Customer',
		avatar: '🧑',
		price: 10.0,
		paymentMethod: 'Cash on Pickup',
		status: 'Rejected',
		icon: '🌊',
	},
	{
		id: 6,
		itemName: 'Pitch Choco',
		category: 'Sweet',
		orderId: '#DGHT738620',
		orderDate: '1 Mar 2025, 23:39',
		customer: 'Design Lagi',
		customerType: 'Star Customer',
		avatar: '⚫',
		price: 69.5,
		paymentMethod: 'Paid by PayPal',
		status: 'Completed',
		icon: '🍫',
	},
	{
		id: 7,
		itemName: 'Bening Latte',
		category: 'Iced',
		orderId: '#CFFE416985',
		orderDate: '1 Apr 2025, 23:39',
		customer: 'John Doe',
		customerType: 'New Customer',
		avatar: '👨‍💼',
		price: 3.2,
		paymentMethod: 'Cash on Delivery',
		status: 'Pending',
		icon: '☕',
	},
];

export const StatusBadge: React.FC<{ status: Order['status'] }> = ({status}) => {
	const statusClass = `app-table-status-${status.toLowerCase()}`;

	return (
		<span className={`app-table-status-badge ${statusClass}`}>
			<span className="app-table-status-dot"></span>
			{status}
		</span>
	);
};

// App Component with Column Configuration
export default function App(): React.JSX.Element {
	const [lazyData, setLazyData] = useState<Order[]>([]);
	const [loading, setLoading] = useState(false);
	const [totalItems, setTotalItems] = useState(0);

	const columns: Column<Order>[] = useMemo(
		() => [
			{
				key: 'itemName',
				label: 'Item Name',
				sortable: true,
				render: (item: Order) => (
					<div className="app-table-cell-item">
						<div className="app-table-cell-icon">{item.icon}</div>
						<div>
							<div className="app-table-cell-title">{item.itemName}</div>
							<div className="app-table-cell-subtitle">{item.category}</div>
						</div>
					</div>
				),
			},
			{
				key: 'orderId',
				label: 'Order ID',
				sortable: true,
				render: (item: Order) => (
					<div>
						<div className="app-table-cell-title">{item.orderId}</div>
						<div className="app-table-cell-subtitle">{item.orderDate}</div>
					</div>
				),
			},
			{
				key: 'customer',
				label: 'Customer',
				sortable: true,
				render: (item: Order) => (
					<div className="app-table-cell-item">
						<div className="app-table-cell-avatar">{item.avatar}</div>
						<div>
							<div className="app-table-cell-title">{item.customer}</div>
							<div className="app-table-cell-subtitle">{item.customerType}</div>
						</div>
					</div>
				),
			},
			{
				key: 'price',
				label: 'Price',
				sortable: true,
				render: (item: Order) => (
					<div>
						<div className="app-table-cell-title">${item.price.toFixed(2)}</div>
						<div className="app-table-cell-subtitle">{item.paymentMethod}</div>
					</div>
				),
			},
			{
				key: 'status',
				label: 'Status',
				sortable: true,
				render: (item: Order) => <StatusBadge status={item.status}/>,
			},
			{
				key: 'action',
				label: 'Action',
				render: (item: Order, onAction?: (item: Order) => void) => (
					<button onClick={() => onAction?.(item)} className="app-table-action-btn">
						View Details
					</button>
				),
			},
		],
		[]
	);

	const handleAction = (item: Order): void => {
		alert(`Viewing details for ${item.itemName} (${item.orderId})`);
	};

	const handleFetchData = useCallback(async ({page, searchTerm, sortConfig}: { page: number; searchTerm: string; sortConfig: SortConfig }) => {
		setLoading(true);

		// Simulate API call delay
		await new Promise((resolve) => setTimeout(resolve, 1000));

		let filtered = [...mockOrders];
		if (searchTerm) {
			filtered = filtered.filter((item) => item.itemName.toLowerCase().includes(searchTerm.toLowerCase()) || item.orderId.toLowerCase().includes(searchTerm.toLowerCase()) || item.customer.toLowerCase().includes(searchTerm.toLowerCase()));
		}

		if (sortConfig.key) {
			filtered.sort((a, b) => {
				const aVal = a[sortConfig.key as keyof Order];
				const bVal = b[sortConfig.key as keyof Order];
				if (aVal < bVal) return sortConfig.direction === 'asc' ? -1 : 1;
				if (aVal > bVal) return sortConfig.direction === 'asc' ? 1 : -1;
				return 0;
			});
		}

		const itemsPerPage = 5;
		const start = (page - 1) * itemsPerPage;
		const paginated = filtered.slice(start, start + itemsPerPage);

		setLazyData(paginated);
		setTotalItems(filtered.length);
		setLoading(false);
	}, []);

	return (
		<div className="app-table-demo-container">
			<div className="app-table-demo-wrapper">
				<h1 className="app-table-demo-title">Product Orders (Client Side)</h1>
				<DataTable<Order> data={mockOrders} columns={columns} itemsPerPage={5} onAction={handleAction}/>
			</div>

			<div className="app-table-demo-wrapper">
				<h1 className="app-table-demo-title">Product Orders (Lazy Loading)</h1>
				<DataTable<Order> data={lazyData} columns={columns} itemsPerPage={5} onAction={handleAction} isLazy={true} loading={loading} totalItems={totalItems}
				                  onFetchData={handleFetchData}/>
			</div>
		</div>
	);
}
