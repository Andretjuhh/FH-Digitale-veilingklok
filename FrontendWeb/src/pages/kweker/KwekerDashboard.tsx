// External imports
import React, {useCallback, useEffect, useMemo, useState} from 'react';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

// Internal import
import {getOrders} from '../../controllers/server/kweker';
import {useRootContext} from '../../components/contexts/RootContext';
import {OrderKwekerOutput} from '../../declarations/dtos/output/OrderKwekerOutput';
import {delay, formatEur} from '../../utils/standards';

// Components
import {Column, DataTable} from '../../components/layout/Table';
import {KwekerDashboardStats} from '../../components/sections/kweker/KwekerStats';
import Button from "../../components/buttons/Button";
import {ClientAvatar} from "../../components/elements/ClientAvatar";
import OrderDetails from "../../components/sections/kweker/OrderDetails";
import Page from '../../components/nav/Page';
import {StatusBadge} from "../../components/elements/StatusBadge";


export default function KwekerDashboard() {
	const {t, account} = useRootContext();
	const pdfRef = React.useRef<HTMLDivElement>(null);

	const [orders, setOrders] = useState<OrderKwekerOutput[]>([]);
	const [selectedOrder, setSelectedOrder] = useState<OrderKwekerOutput | null>(null);
	const [generatingPdf, setGeneratingPdf] = useState(false);
	const [showOrderModal, setShowOrderModal] = useState({visible: false, editMode: false});

	// PDF Generation state

	useEffect(() => {
		initializeDashboard();
	}, []);

	const orderColumns: Column<OrderKwekerOutput>[] = useMemo(
		() => [
			{
				key: 'productName',
				label: 'Product',
				sortable: true,
				render: (item) => <span className="font-medium">{item.product.name}</span>,
			},
			{
				key: 'clientName',
				label: 'Client Name',
				sortable: true,
				render: (item) => {
					const fullName = `${item.koperInfo.firstName} ${item.koperInfo.lastName}`;
					return (
						<div className="flex items-center">
							<ClientAvatar name={fullName}/>
							<span className="font-medium">{fullName}</span>
						</div>
					);
				},
			},
			{
				key: 'status',
				label: 'Order Status',
				sortable: true,
				render: (item) => <StatusBadge status={item.status}/>,
			},
			{
				key: 'totalPrice',
				label: 'Total Price',
				sortable: true,
				render: (item) => {
					const total = item.quantity * (item.product.auctionedPrice || 0);
					return <span className="font-semibold">{formatEur(total)}</span>;
				},
			},
			{
				key: 'createdAt',
				label: 'Ordered At',
				sortable: true,
				render: (item) => <span>{new Date(item.createdAt).toLocaleDateString()}</span>,
			},

			{
				key: 'action',
				label: 'Action',
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
						<Button
							className={'app-table-action-btn'}
							icon={'bi-pen-fill'}
							onClick={() => {
								setSelectedOrder(item);
								showOrder(true, true);
							}}
						/>
						<Button
							className={'app-table-action-btn'}
							icon={'bi-download'}
							onClick={() => generateOrderPDF(item)}
						/>
					</div>

				),
			},
		],
		[t]
	);

	const initializeDashboard = useCallback(async () => {
		try {
			// Fetch orders
			const orderResponse = await getOrders();
			if (orderResponse.data) {
				// Remove duplicates by id
				const uniqueOrders = Array.from(
					new Map(orderResponse.data.data.map(order => [String(order.id), order])).values()
				);
				setOrders(uniqueOrders);
			}
		} catch (err) {
			console.error('Failed to initialize dashboard', err);
		}
	}, []);
	const showOrder = useCallback((open: boolean, editMode: boolean = false) => {
		setShowOrderModal({visible: open, editMode});
	}, []);
	const generateOrderPDF = useCallback(async (order: OrderKwekerOutput) => {
		try {
			setGeneratingPdf(true);
			setSelectedOrder(order);
			await delay(1000); // Wait for the hidden container to render

			const element = pdfRef.current;
			if (!element) return;

			const canvas = await html2canvas(element, {
				scale: 2.5, // Improve quality
				useCORS: true, // Handle external images if any
				logging: false
			});

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
			console.error("PDF generation failed", error);
		} finally {
			setGeneratingPdf(false);
		}
	}, []);

	return (
		<Page enableHeader className="kweker-page" enableHeaderAnimation={false}>
			<main className="kweker-container">
				<section className="kweker-hallo">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
				</section>

				<KwekerDashboardStats/>
				<DataTable<OrderKwekerOutput>
					data={orders}
					columns={orderColumns}
					itemsPerPage={5}
					icon={<i className="bi bi-cart4"></i>}
					title={t('recent_orders')}
					emptyText={t('no_orders')}
				/>


				{showOrderModal.visible && selectedOrder && (
					<div className="modal-overlay" onClick={() => showOrder(false)}>
						<OrderDetails order={selectedOrder} editMode={showOrderModal.editMode} onClose={() => showOrder(false)}/>
					</div>
				)}

				{/* Hidden container for PDF generation */}
				<div style={{position: 'absolute', left: '-9999px', top: 0, width: '210mm'}}>
					{(generatingPdf && selectedOrder) && (
						<div ref={pdfRef} style={{padding: '20px', background: 'white'}}>
							<OrderDetails order={selectedOrder} printMode/>
						</div>
					)}
				</div>
			</main>
		</Page>
	);
}
