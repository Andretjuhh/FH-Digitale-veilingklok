import React, {useCallback, useEffect, useMemo, useState} from 'react';
import Button from '../../components/buttons/Button';
import FormInputField from '../../components/elements/FormInputField';
import {useForm} from 'react-hook-form';
import Page from '../../components/nav/Page';
import {createProduct, getOrders, getProductDetails, getProducts} from '../../controllers/server/kweker';
import {formatEur} from '../../utils/standards';
import {ProductOutputDto} from '../../declarations/dtos/output/ProductOutputDto';
import {ProductDetailsOutputDto} from '../../declarations/dtos/output/ProductDetailsOutputDto';
import {OrderKwekerOutput} from '../../declarations/dtos/output/OrderKwekerOutput';
import {CreateProductDTO} from '../../declarations/dtos/input/CreateProductDTO';
import {useRootContext} from '../../components/contexts/RootContext';
import {useComponentStateReducer} from '../../hooks/useComponentStateReducer';
import KwekerStats from '../../components/sections/kweker/KwekerStats';
import {Column, DataTable, StatusBadge} from '../../components/elements/Table';

const getRandomColor = (name: string) => {
	const colors = ['#EF4444', '#F59E0B', '#10B981', '#3B82F6', '#6366F1', '#8B5CF6', '#EC4899'];
	let hash = 0;
	for (let i = 0; i < name.length; i++) {
		hash = name.charCodeAt(i) + ((hash << 5) - hash);
	}
	return colors[Math.abs(hash) % colors.length];
};

const ClientAvatar = ({name}: { name: string }) => {
	const initials = name
		.split(' ')
		.map((n) => n[0])
		.join('')
		.toUpperCase()
		.substring(0, 2);
	const color = getRandomColor(name);
	return (
		<div style={{backgroundColor: color}} className="w-8 h-8 rounded-full flex items-center justify-center text-white text-xs font-bold mr-3 overflow-hidden shrink-0">
			{initials}
		</div>
	);
};

export default function KwekerDashboard() {
	const {t, account} = useRootContext();
	const [activeTab, setActiveTab] = useState<'my' | 'history'>('my');
	const [products, setProducts] = useState<ProductOutputDto[]>([]);
	const [orders, setOrders] = useState<OrderKwekerOutput[]>([]);
	const [isCreating, setIsCreating] = useState(false);
	const [showForm, setShowForm] = useState(false);
	const [state, updateState] = useComponentStateReducer();

	// Preview modal state
	const [showPreview, setShowPreview] = useState(false);
	const [previewLoading, setPreviewLoading] = useState(false);
	const [previewError, setPreviewError] = useState<string | null>(null);
	const [previewProduct, setPreviewProduct] = useState<ProductDetailsOutputDto | null>(null);

	// Order detail modal state
	const [selectedOrder, setSelectedOrder] = useState<OrderKwekerOutput | null>(null);
	const [showOrderModal, setShowOrderModal] = useState(false);

	const {
		register,
		handleSubmit,
		setError,
		formState: {errors},
		reset,
		getValues,
	} = useForm<CreateProductDTO>({
		defaultValues: {name: '', description: '', minimumPrice: 0, stock: 0, imageBase64: '', dimension: ''},
	});

	const [savedValues, setSavedValues] = useState<CreateProductDTO | null>(null);

	useEffect(() => {
		initializeDashboard();
	}, [activeTab]);

	const orderColumns: Column<OrderKwekerOutput>[] = useMemo(
		() => [
			{
				key: 'id',
				label: 'ID',
				sortable: true,
				render: (item) => <span className="text-xs font-mono">{/*item.id.substring(0, 12)*/item.id}</span>,
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
					<button
						onClick={() => {
							setSelectedOrder(item);
							setShowOrderModal(true);
						}}
						className="px-3 py-1 bg-primary-light text-primary-main rounded-md text-sm font-medium hover:bg-primary-main hover:text-white transition-colors"
					>
						View Details
					</button>
				),
			},
		],
		[t]
	);

	const initializeDashboard = useCallback(async () => {
		try {
			// Fetch products
			const prodResponse = await getProducts();
			if (prodResponse.data) {
				setProducts(prodResponse.data.data);
			}

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
					<p className="kweker-desc">Welkom op de dashboard pagina! Bekijk hier uw producten!</p>
				</section>
				<KwekerStats/>
				<div className="mt-8">
					<h2 className="text-xl font-bold mb-4">Recente Bestellingen</h2>
					<DataTable<OrderKwekerOutput> data={orders} columns={orderColumns} itemsPerPage={5} title={'Recente Bestellingen'}/>
				</div>
				<section className="kweker-content">
					<div className="content-inner">
						<div className="product-grid">
							{products.map((p) => (
								<article
									key={p.id}
									className="product-card clickable"
									role="button"
									tabIndex={0}
									onClick={async () => {
										// open preview modal and fetch details
										setShowPreview(true);
										setPreviewLoading(true);
										setPreviewError(null);
										setPreviewProduct(null);
										try {
											const res = await getProductDetails(p.id);
											if (res.data) {
												setPreviewProduct(res.data);
											}
										} catch (err) {
											console.error('Failed to load product preview', err);
											setPreviewError('Kon het product niet laden.');
										} finally {
											setPreviewLoading(false);
										}
									}}
									onKeyDown={(e) => {
										if (e.key === 'Enter' || e.key === ' ') {
											(e.currentTarget as HTMLElement).click();
										}
									}}
								>
									<div className="product-info-kweker">
										<h3 className="product-title">{p.name}</h3>
										<p className="product-desc">{p.description}</p>
									</div>
									<div className="product-price">
										<div className="product-price">{formatEur(p.auctionedPrice ?? 0)}</div>
										<div className="product-thumb" style={{backgroundImage: `url(${p.imageUrl})`}}/>
									</div>
								</article>
							))}
						</div>
						<div className="content-footer">
							{activeTab === 'my' && (
								<>
									{!showForm && (
										<Button
											className="toevoegen-knop"
											label={'Voeg nieuw product toe'}
											onClick={() => {
												setShowForm(true);
												if (savedValues) reset(savedValues);
											}}
										>
											Toevoegen
										</Button>
									)}
									{showForm && (
										<div
											className="modal-overlay"
											onClick={() => {
												if (!isCreating) {
													setSavedValues(getValues());
													setShowForm(false);
												}
											}}
										>
											<div className="modal" onClick={(e) => e.stopPropagation()} role="dialog" aria-modal="true">
												<div className="modal-header">
													<h3>Nieuw product toevoegen</h3>
													<button
														className="modal-close"
														onClick={() => {
															setSavedValues(getValues());
															setShowForm(false);
														}}
														aria-label="Sluiten"
													>
														✕
													</button>
												</div>
												<div className="modal-body">
													<form
														onSubmit={handleSubmit(async (data) => {
															if (isCreating) return;
															setIsCreating(true);
															try {
																const response = await createProduct(data);
																if (response.data) {
																	const p = response.data;
																	const mapped: ProductOutputDto = {
																		id: p.id,
																		name: p.name,
																		description: p.description,
																		imageUrl: p.imageBase64, // Using base64 as preview
																		auctionedPrice: p.auctionPrice,
																		auctionedAt: p.auctionedAt,
																		dimension: p.dimension,
																		stock: p.stock,
																		companyName: p.companyName,
																	};
																	setProducts((prev) => [mapped, ...prev]);
																	setShowForm(false);
																	reset();
																	setSavedValues(null);
																}
															} catch (err: any) {
																console.error('createProduct failed', err);
																setError('name', {type: 'server', message: 'Onbekende fout bij het aanmaken van product'});
															} finally {
																setIsCreating(false);
															}
														})}
														className="create-product-form"
													>
														<div className="form-row">
															<FormInputField id="name" label="Naam" {...register('name', {required: 'Naam is verplicht'})} isError={!!errors.name}
															                error={errors.name?.message as string}/>
														</div>

														<div className="form-row">
															<label>Beschrijving</label>
															<textarea {...register('description', {required: 'Beschrijving is verplicht'})} />
														</div>

														<div className="form-row">
															<FormInputField id="minimumPrice" label="Minimum prijs (€)" type="number"
															                step="0.01" {...register('minimumPrice', {required: 'Minimum prijs is verplicht', min: 0})}
															                isError={!!errors.minimumPrice} error={errors.minimumPrice?.message as string}/>
														</div>

														<div className="form-row">
															<FormInputField
																id="stock"
																label="Aantal"
																type="number"
																{...register('stock', {
																	required: 'Aantal is verplicht',
																	min: 0,
																})}
																isError={!!errors.stock}
																error={errors.stock?.message as string}
															/>
														</div>

														<div className="form-row">
															<FormInputField id="imageBase64"
															                label="Afbeelding (Base64)" {...register('imageBase64', {required: 'Afbeelding is verplicht'})}
															                isError={!!errors.imageBase64} error={errors.imageBase64?.message as string}/>
														</div>

														<div className="form-row">
															<FormInputField id="dimension" label="Maat" {...register('dimension')} isError={!!errors.dimension}
															                error={errors.dimension?.message as string}/>
														</div>

														<div className="form-row form-actions">
															<button type="submit" className="toevoegen-knop btn-primary" disabled={isCreating}>
																{isCreating ? 'Bezig...' : 'Voeg nieuw product toe'}
															</button>
															<button
																type="button"
																className="cancel-btn"
																onClick={() => {
																	if (!isCreating) {
																		setSavedValues(getValues());
																		setShowForm(false);
																	}
																}}
																disabled={isCreating}
															>
																Annuleren
															</button>
														</div>
													</form>
												</div>
											</div>
										</div>
									)}

									{/* Product preview modal */}
									{showPreview && (
										<div
											className="modal-overlay"
											onClick={() => {
												if (!previewLoading) {
													setShowPreview(false);
													setPreviewProduct(null);
													setPreviewError(null);
												}
											}}
										>
											<div className="modal" onClick={(e) => e.stopPropagation()} role="dialog" aria-modal="true">
												<div className="modal-header">
													<h3>Product</h3>
													<button
														className="modal-close"
														onClick={() => {
															setShowPreview(false);
															setPreviewProduct(null);
															setPreviewError(null);
														}}
														aria-label="Sluiten"
													>
														✕
													</button>
												</div>
												<div className="modal-body">
													{previewLoading && <div>Bezig met laden...</div>}
													{previewError && <div className="error">{previewError}</div>}
													{previewProduct && (
														<div className="product-details">
															<div className="product-details-grid">
																<div className="product-meta">
																	<div className="product-meta-header">
																		<h2 className="product-name">{previewProduct.name}</h2>
																		<div className="product-price">{formatEur(previewProduct.auctionPrice ?? previewProduct.minimumPrice)}</div>
																	</div>
																	<div className="product-description">{previewProduct.description}</div>
																	<div className="product-quantity">Voorraad: {previewProduct.stock}</div>
																	{previewProduct.dimension && <div className="product-size">Maat: {previewProduct.dimension}</div>}
																	<div className="product-company">Kweker: {previewProduct.companyName}</div>
																</div>
																<div className="product-image">{previewProduct.imageBase64 ?
																	<img src={previewProduct.imageBase64} alt={previewProduct.name}/> :
																	<div className="product-image-placeholder" aria-hidden="true"/>}</div>
															</div>
														</div>
													)}
												</div>
											</div>
										</div>
									)}

									{/* Order Detail Modal */}
									{showOrderModal && selectedOrder && (
										<div className="modal-overlay" onClick={() => setShowOrderModal(false)}>
											<div className="modal max-w-2xl" onClick={(e) => e.stopPropagation()}>
												<div className="modal-header">
													<h3>Bestelling Details</h3>
													<button className="modal-close" onClick={() => setShowOrderModal(false)}>
														✕
													</button>
												</div>
												<div className="modal-body">
													<div className="order-details-content p-2">
														<div className="grid grid-cols-1 md:grid-cols-2 gap-6">
															<div className="bg-gray-50 p-4 rounded-xl">
																<h4 className="font-bold text-primary-main mb-3 flex items-center">
																	<i className="bi bi-person-circle mr-2"></i> Klantgegevens
																</h4>
																<div className="space-y-1">
																	<p className="text-base font-semibold text-gray-800">
																		{selectedOrder.koperInfo.firstName} {selectedOrder.koperInfo.lastName}
																	</p>
																	<p className="text-sm text-gray-600 flex items-center">
																		<i className="bi bi-envelope mr-2 text-primary-light"></i> {selectedOrder.koperInfo.email}
																	</p>
																	<p className="text-sm text-gray-600 flex items-center">
																		<i className="bi bi-telephone mr-2 text-primary-light"></i> {selectedOrder.koperInfo.telephone}
																	</p>
																</div>
															</div>

															<div className="bg-gray-50 p-4 rounded-xl">
																<h4 className="font-bold text-primary-main mb-3 flex items-center">
																	<i className="bi bi-geo-alt-fill mr-2"></i> Afleveradres
																</h4>
																<div className="space-y-1">
																	<p className="text-sm text-gray-800 leading-relaxed">
																		{selectedOrder.koperInfo.address.street}
																		<br/>
																		<span className="font-medium">
																			{selectedOrder.koperInfo.address.postalCode} {selectedOrder.koperInfo.address.city}
																		</span>
																		<br/>
																		<span className="text-gray-500 uppercase text-xs tracking-wider">
																			{selectedOrder.koperInfo.address.regionOrState}, {selectedOrder.koperInfo.address.country}
																		</span>
																	</p>
																</div>
															</div>
														</div>

														<div className="mt-6">
															<h4 className="font-bold text-primary-main mb-3 flex items-center px-1">
																<i className="bi bi-box-seam-fill mr-2"></i> Bestelde Producten
															</h4>
															<div className="bg-white border border-gray-100 rounded-xl overflow-hidden shadow-sm">
																<div className="flex items-center p-4">
																	<div
																		className="w-16 h-16 bg-gray-100 rounded-lg shrink-0 mr-4 border border-gray-100"
																		style={{
																			backgroundImage: `url(${selectedOrder.product.imageUrl || '/pictures/kweker.png'})`,
																			backgroundSize: 'cover',
																			backgroundPosition: 'center',
																		}}
																	/>
																	<div className="flex-1">
																		<p className="font-bold text-gray-800 text-lg">{selectedOrder.product.name}</p>
																		<p className="text-sm text-gray-500 font-medium">
																			{selectedOrder.quantity} stuks <span
																			className="text-gray-300 mx-1">×</span> {formatEur(selectedOrder.product.auctionedPrice || 0)}
																		</p>
																	</div>
																	<div className="text-right">
																		<p className="text-lg font-black text-primary-main">{formatEur(selectedOrder.quantity * (selectedOrder.product.auctionedPrice || 0))}</p>
																	</div>
																</div>
															</div>
														</div>

														<div className="mt-8 grid grid-cols-2 gap-4 border-t border-gray-100 pt-6 px-1">
															<div className="space-y-1">
																<p className="text-xs text-gray-400 uppercase font-bold tracking-widest">Besteldatum</p>
																<p className="text-sm font-medium text-gray-700">
																	{new Date(selectedOrder.createdAt).toLocaleString('nl-NL', {
																		day: '2-digit',
																		month: 'long',
																		year: 'numeric',
																		hour: '2-digit',
																		minute: '2-digit',
																	})}
																</p>
															</div>
															<div className="text-right space-y-1">
																<p className="text-xs text-gray-400 uppercase font-bold tracking-widest text-right">Status</p>
																<div className="flex justify-end">
																	<StatusBadge status={selectedOrder.status}/>
																</div>
															</div>
														</div>
													</div>
												</div>
											</div>
										</div>
									)}
								</>
							)}
						</div>
					</div>
				</section>
			</main>
		</Page>
	);
}
