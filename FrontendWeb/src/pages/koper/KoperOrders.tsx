import React, { useCallback, useMemo, useState } from 'react';
import { useRootContext } from '../../components/contexts/RootContext';
import { useComponentStateReducer } from '../../hooks/useComponentStateReducer';
import { PaginatedOutputDto } from '../../declarations/dtos/output/PaginatedOutputDto';
import { Column, DataTable, OnFetchHandlerParams } from '../../components/layout/Table';
import { StatusBadge } from '../../components/elements/StatusBadge';
import { delay, formatEur } from '../../utils/standards';
import Button from '../../components/buttons/Button';
import { getOrders } from '../../controllers/server/koper';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import Page from '../../components/nav/Page';
import { KoperStats } from '../../components/sections/koper/KoperStats';
import Modal from '../../components/elements/Modal';
import { OrderOutputDto } from '../../declarations/dtos/output/OrderOutputDto';
import KoperOrderDetails from '../../components/sections/koper/KoperOrderDetails';
import { ClientAvatar } from '../../components/elements/ClientAvatar';

function KoperOrders() {
	const { t, account } = useRootContext();
	const pdfRef = React.useRef<HTMLDivElement>(null);

	const [paginatedOrdersState, setPaginatedOrdersState] = useComponentStateReducer();
	const [paginatedOrders, setPaginatedOrders] = useState<PaginatedOutputDto<OrderOutputDto>>();
	const [selectedOrder, setSelectedOrder] = useState<OrderOutputDto | null>(null);
	const [generatingPdf, setGeneratingPdf] = useState(false);
	const [showOrderModal, setShowOrderModal] = useState({ visible: false, editMode: false });

	const orderColumns: Column<OrderOutputDto>[] = useMemo(
		() => [
			{
				key: 'productName',
				label: t('product_name'),
				sortable: true,
				render: (item) => <span className="font-medium">{item.productName}</span>,
			},
			{
				key: 'companyName',
				label: t('kweker_name'),
				sortable: true,
				render: (item) => {
					return (
						<div className="flex items-center">
							<ClientAvatar name={item.companyName} />
							<span className="font-medium">{item.companyName}</span>
						</div>
					);
				},
			},
			{
				key: 'status',
				label: t('order_status'),
				sortable: true,
				render: (item) => <StatusBadge status={item.status} />,
			},
			{
				key: 'totalAmount',
				label: t('total_value'),
				sortable: true,
				render: (item) => <span className="font-semibold">{formatEur(item.totalAmount)}</span>,
			},
			{
				key: 'totalItems',
				label: 'Total Items',
				sortable: true,
				render: (item) => <span>{item.totalItems}</span>,
			},
			{
				key: 'createdAt',
				label: t('order_datum'),
				sortable: true,
				render: (item) => <span>{new Date(item.createdAt).toLocaleDateString()}</span>,
			},

			{
				key: 'action',
				label: t('actions'),
				render: (item) => (
					<div className={'app-table-actions-row-btns'}>
						<Button
							className={'app-table-action-btn'}
							icon={'bi-file-earmark-text-fill'}
							onClick={() => {
								setSelectedOrder(item);
								showOrder(true);
							}}
						/>
						<Button className={'app-table-action-btn'} icon={'bi-download'} onClick={() => generateOrderPDF(item)} />
					</div>
				),
			},
		],
		[t],
	);

	const handleFetchOrders = useCallback(async (params: OnFetchHandlerParams) => {
		try {
			setPaginatedOrdersState({ type: 'loading' });
			const orderResponse = await getOrders(params.searchTerm, undefined, undefined, params.page, params.pageSize);
			if (orderResponse.data) setPaginatedOrders(orderResponse.data);
			setPaginatedOrdersState({ type: 'succeed' });
		} catch (err) {
			console.error('Failed to fetch orders', err);
		}
	}, []);
	const showOrder = useCallback((open: boolean, editMode: boolean = false) => {
		setShowOrderModal({ visible: open, editMode });
	}, []);
	const generateOrderPDF = useCallback(async (order: OrderOutputDto) => {
		try {
			// PDF generation logic here
			// For now, this is a stub as KwekerOrderDetails is not fully implemented for Koper
			console.log('Generating PDF for order', order.id);
			setGeneratingPdf(true);
			setSelectedOrder(order);
			await delay(1000);

			const element = pdfRef.current;
			if (!element) return;

			const canvas = await html2canvas(element, {
				scale: 2.5,
				useCORS: true,
				logging: false,
			});
			// ... (rest of logic mostly same if element exists)

			const imgData = canvas.toDataURL('image/png');
			const pdf = new jsPDF('p', 'mm', 'a4');
			const pdfWidth = pdf.internal.pageSize.getWidth();
			const pdfHeight = pdf.internal.pageSize.getHeight();
			const imgWidth = pdfWidth;
			const imgHeight = (canvas.height * imgWidth) / canvas.width;

			let heightLeft = imgHeight;
			let position = 0;

			pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
			heightLeft -= pdfHeight;

			while (heightLeft >= 0) {
				position = heightLeft - imgHeight;
				pdf.addPage();
				pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
				heightLeft -= pdfHeight;
			}

			pdf.save(`order-${order.id}.pdf`);
		} catch (error) {
			console.error('PDF generation failed', error);
		} finally {
			setGeneratingPdf(false);
		}
	}, []);

	return (
		<Page enableHeader className="kweker-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="kweker-products-page-ctn">
				<section className="page-title-section">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2>{t('kweker_orders_description')}</h2>
				</section>

				<KoperStats />

				<DataTable<OrderOutputDto>
					isLazy
					loading={paginatedOrdersState.type == 'loading'}
					data={paginatedOrders?.data || []}
					itemsPerPage={20}
					totalItems={paginatedOrders?.totalCount || 0}
					getItemKey={(item) => item.id}
					onFetchData={handleFetchOrders}
					onCellClick={(item) => {
						setSelectedOrder(item);
						showOrder(true);
					}}
					title={t('recent_orders')}
					icon={<i className="bi bi-cart4"></i>}
					filterGroups={
						<>
							<Button icon="bi-chevron-down" className="app-table-filter-btn" label={'All Status'} />
							<Button icon="bi-chevron-down" className="app-table-filter-btn" label={'More Filters'} />
						</>
					}
					columns={orderColumns}
					emptyText={t('no_orders')}
				/>

				<Modal enabled={showOrderModal.visible && selectedOrder != null} onClose={() => showOrder(false)}>
					{selectedOrder && <KoperOrderDetails orderId={selectedOrder.id} onClose={() => showOrder(false)} />}
				</Modal>

				{/* Hidden container for PDF generation */}
				<div style={{ position: 'absolute', left: '-9999px', top: 0, width: '210mm' }}>
					{generatingPdf && selectedOrder && (
						<div ref={pdfRef} style={{ padding: '20px', background: 'white' }}>
							{selectedOrder && <KoperOrderDetails orderId={selectedOrder.id} onClose={() => showOrder(false)} />}
						</div>
					)}
				</div>
			</main>
		</Page>
	);
}

export default KoperOrders;
