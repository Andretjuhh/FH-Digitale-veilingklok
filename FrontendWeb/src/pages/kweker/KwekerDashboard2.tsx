import React, {useCallback, useEffect, useMemo, useState} from 'react';
import Page from '../../components/nav/Page';
import {getOrders} from '../../controllers/server/kweker';
import {formatEur} from '../../utils/standards';
import {OrderKwekerOutput} from '../../declarations/dtos/output/OrderKwekerOutput';
import {useRootContext} from '../../components/contexts/RootContext';
import KwekerStats from '../../components/sections/kweker/KwekerStats';
import {Column, DataTable, StatusBadge} from '../../components/elements/Table';
import Button from "../../components/buttons/Button";
import {ClientAvatar} from "../../components/elements/ClientAvatar";
import OrderDetails from "../../components/sections/kweker/OrderDetails";


export default function KwekerDashboard() {
	const {t, account} = useRootContext();
	const [orders, setOrders] = useState<OrderKwekerOutput[]>([]);

	// Order detail modal state
	const [selectedOrder, setSelectedOrder] = useState<OrderKwekerOutput | null>(null);
	const [showOrderModal, setShowOrderModal] = useState(false);

	useEffect(() => {
		initializeDashboard();
	}, []);

	const orderColumns: Column<OrderKwekerOutput>[] = useMemo(
		() => [
			// {
			// 	key: 'id',
			// 	label: 'ID',
			// 	sortable: true,
			// 	render: (item) => <span className="text-xs font-mono">{/*item.id.substring(0, 12)*/item.id}</span>,
			// },
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
								setShowOrderModal(true);
							}}
						/>
						<Button
							className={'app-table-action-btn'}
							icon={'bi-pen-fill'}
							onClick={() => {
								setSelectedOrder(item);
								setShowOrderModal(true);
							}}
						/>
						<Button
							className={'app-table-action-btn'}
							icon={'bi-download'}
							onClick={() => {
								setSelectedOrder(item);
								setShowOrderModal(true);
							}}
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
				setOrders(orderResponse.data.data);
			}
		} catch (err) {
			console.error('Failed to initialize dashboard', err);
		}
	}, []);

	return (
		<Page enableHeader className="kweker-page" enableHeaderAnimation={false}>
			<main className="kweker-container">
				<section className="kweker-hallo">
					<h1>
						Welcome, {account?.firstName} {account?.lastName}
					</h1>
				</section>

				<KwekerStats/>
				<DataTable<OrderKwekerOutput>
					data={orders}
					columns={orderColumns}
					itemsPerPage={5}
					icon={<i className="bi bi-cart4"></i>}
					title={'Recente Bestellingen'}
				/>

				<section className="kweker-content">
					<div className="content-inner">
						{showOrderModal && selectedOrder && (
							<div className="modal-overlay" onClick={() => setShowOrderModal(false)}>
								{/*<div className="modal max-w-2xl" onClick={(e) => e.stopPropagation()}>*/}
								{/*	<div className="modal-header">*/}
								{/*		<h3>Bestelling Details</h3>*/}
								{/*		<button className="modal-close" onClick={() => setShowOrderModal(false)}>*/}
								{/*			✕*/}
								{/*		</button>*/}
								{/*	</div>*/}
								{/*	<div className="modal-body">*/}
								{/*		<div className="order-details-content p-2">*/}
								{/*			<div className="grid grid-cols-1 md:grid-cols-2 gap-6">*/}
								{/*				<div className="bg-gray-50 p-4 rounded-xl">*/}
								{/*					<h4 className="font-bold text-primary-main mb-3 flex items-center">*/}
								{/*						<i className="bi bi-person-circle mr-2"></i> Klantgegevens*/}
								{/*					</h4>*/}
								{/*					<div className="space-y-1">*/}
								{/*						<p className="text-base font-semibold text-gray-800">*/}
								{/*							{selectedOrder.koperInfo.firstName} {selectedOrder.koperInfo.lastName}*/}
								{/*						</p>*/}
								{/*						<p className="text-sm text-gray-600 flex items-center">*/}
								{/*							<i className="bi bi-envelope mr-2 text-primary-light"></i> {selectedOrder.koperInfo.email}*/}
								{/*						</p>*/}
								{/*						<p className="text-sm text-gray-600 flex items-center">*/}
								{/*							<i className="bi bi-telephone mr-2 text-primary-light"></i> {selectedOrder.koperInfo.telephone}*/}
								{/*						</p>*/}
								{/*					</div>*/}
								{/*				</div>*/}

								{/*				<div className="bg-gray-50 p-4 rounded-xl">*/}
								{/*					<h4 className="font-bold text-primary-main mb-3 flex items-center">*/}
								{/*						<i className="bi bi-geo-alt-fill mr-2"></i> Afleveradres*/}
								{/*					</h4>*/}
								{/*					<div className="space-y-1">*/}
								{/*						<p className="text-sm text-gray-800 leading-relaxed">*/}
								{/*							{selectedOrder.koperInfo.address.street}*/}
								{/*							<br/>*/}
								{/*							<span className="font-medium">*/}
								{/*											{selectedOrder.koperInfo.address.postalCode} {selectedOrder.koperInfo.address.city}*/}
								{/*										</span>*/}
								{/*							<br/>*/}
								{/*							<span className="text-gray-500 uppercase text-xs tracking-wider">*/}
								{/*											{selectedOrder.koperInfo.address.regionOrState}, {selectedOrder.koperInfo.address.country}*/}
								{/*										</span>*/}
								{/*						</p>*/}
								{/*					</div>*/}
								{/*				</div>*/}
								{/*			</div>*/}

								{/*			<div className="mt-6">*/}
								{/*				<h4 className="font-bold text-primary-main mb-3 flex items-center px-1">*/}
								{/*					<i className="bi bi-box-seam-fill mr-2"></i> Bestelde Producten*/}
								{/*				</h4>*/}
								{/*				<div className="bg-white border border-gray-100 rounded-xl overflow-hidden shadow-sm">*/}
								{/*					<div className="flex items-center p-4">*/}
								{/*						<div*/}
								{/*							className="w-16 h-16 bg-gray-100 rounded-lg shrink-0 mr-4 border border-gray-100"*/}
								{/*							style={{*/}
								{/*								backgroundImage: `url(${selectedOrder.product.imageUrl || '/pictures/kweker.png'})`,*/}
								{/*								backgroundSize: 'cover',*/}
								{/*								backgroundPosition: 'center',*/}
								{/*							}}*/}
								{/*						/>*/}
								{/*						<div className="flex-1">*/}
								{/*							<p className="font-bold text-gray-800 text-lg">{selectedOrder.product.name}</p>*/}
								{/*							<p className="text-sm text-gray-500 font-medium">*/}
								{/*								{selectedOrder.quantity} stuks <span*/}
								{/*								className="text-gray-300 mx-1">×</span> {formatEur(selectedOrder.product.auctionedPrice || 0)}*/}
								{/*							</p>*/}
								{/*						</div>*/}
								{/*						<div className="text-right">*/}
								{/*							<p className="text-lg font-black text-primary-main">{formatEur(selectedOrder.quantity * (selectedOrder.product.auctionedPrice || 0))}</p>*/}
								{/*						</div>*/}
								{/*					</div>*/}
								{/*				</div>*/}
								{/*			</div>*/}

								{/*			<div className="mt-8 grid grid-cols-2 gap-4 border-t border-gray-100 pt-6 px-1">*/}
								{/*				<div className="space-y-1">*/}
								{/*					<p className="text-xs text-gray-400 uppercase font-bold tracking-widest">Besteldatum</p>*/}
								{/*					<p className="text-sm font-medium text-gray-700">*/}
								{/*						{new Date(selectedOrder.createdAt).toLocaleString('nl-NL', {*/}
								{/*							day: '2-digit',*/}
								{/*							month: 'long',*/}
								{/*							year: 'numeric',*/}
								{/*							hour: '2-digit',*/}
								{/*							minute: '2-digit',*/}
								{/*						})}*/}
								{/*					</p>*/}
								{/*				</div>*/}
								{/*				<div className="text-right space-y-1">*/}
								{/*					<p className="text-xs text-gray-400 uppercase font-bold tracking-widest text-right">Status</p>*/}
								{/*					<div className="flex justify-end">*/}
								{/*						<StatusBadge status={selectedOrder.status}/>*/}
								{/*					</div>*/}
								{/*				</div>*/}
								{/*			</div>*/}
								{/*		</div>*/}
								{/*	</div>*/}
								{/*</div>*/}
								<OrderDetails order={selectedOrder}/>
							</div>
						)}
					</div>
				</section>
			</main>
		</Page>
	);
}
