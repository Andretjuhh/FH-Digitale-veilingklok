import React, { useCallback, useEffect, useState } from 'react';
import Button from '../../components/buttons/Button';
import FormInputField from '../../components/elements/FormInputField';
import { useForm, SubmitHandler } from 'react-hook-form';
import Page from '../../components/nav/Page';
import { getKwekerProducts, getKwekerStats } from '../../controllers/kweker';
import { createProduct, getProductById } from '../../controllers/Product';
import { ProductDetails } from '../../declarations/ProductDetails';
import { formatEur } from '../../utils/formatters';

type KwekerStats = {
	totalProducts: number;
	activeAuctions: number;
	totalRevenue: number;
	stemsSold: number;
};

type Product = {
	id: string | number;
	title: string;
	description: string;
	price: number;
};

// initialStats and hook removed — state is declared inside the component

const sampleProducts: Product[] = [
	{ id: '1', title: 'Product 1', description: 'Beschrijving van product 1', price: 19.99 },
	{ id: '2', title: 'Product 2', description: 'Beschrijving van product 2', price: 24.5 },
	{ id: '3', title: 'Product 3', description: 'Beschrijving van product 3', price: 12.0 },
	{ id: '4', title: 'Product 4', description: 'Beschrijving van product 4', price: 34.75 },
	{ id: '5', title: 'Product 5', description: 'Beschrijving van product 5', price: 9.99 },
	{ id: '6', title: 'Product 6', description: 'Beschrijving van product 6', price: 49.0 },
	{ id: '7', title: 'Product 7', description: 'Beschrijving van product 7', price: 22.0 },
];

const sampleHistory: Product[] = [
	{ id: 'h1', title: 'Verkocht product 1', description: 'Beschrijving verkocht item 1', price: 14.25 },
	{ id: 'h2', title: 'Verkocht product 2', description: 'Beschrijving verkocht item 2', price: 45.0 },
	{ id: 'h3', title: 'Verkocht product 3', description: 'Beschrijving verkocht item 3', price: 22.1 },
	{ id: 'h4', title: 'Verkocht product 4', description: 'Beschrijving verkocht item 4', price: 37.8 },
	{ id: 'h5', title: 'Verkocht product 5', description: 'Beschrijving verkocht item 5', price: 8.75 },
	{ id: 'h6', title: 'Verkocht product 6', description: 'Beschrijving verkocht item 6', price: 59.99 },
];

// `formatEur` moved to `Application/src/utils/formatters.ts` and is imported above

export default function KwekerDashboard() {
	const [stats, setStats] = useState<KwekerStats | null>(null);
	const [activeTab, setActiveTab] = useState<'my' | 'history'>('my');
	const [products, setProducts] = useState<Product[]>(activeTab === 'my' ? sampleProducts : sampleHistory);
	const [isCreating, setIsCreating] = useState(false);
	const [showForm, setShowForm] = useState(false);
	// Preview modal state
	const [showPreview, setShowPreview] = useState(false);
	const [previewLoading, setPreviewLoading] = useState(false);
	const [previewError, setPreviewError] = useState<string | null>(null);
	const [previewProduct, setPreviewProduct] = useState<ProductDetails | null>(null);

	// React Hook Form for create product modal
	type CreateForm = {
		name: string;
		description?: string;
		price: number | '';
		minimumPrice?: number | '';
		quantity?: number | '';
		imageUrl?: string;
		size?: string;
	};

	const { register, handleSubmit, setError, formState: { errors }, reset, getValues } = useForm<CreateForm>({
		defaultValues: { name: '', description: '', price: '', minimumPrice: '', quantity: '', imageUrl: '', size: '' },
	});

	const [savedValues, setSavedValues] = useState<CreateForm | null>(null);

	useEffect(() => {
		initializeProducts();
	}, [activeTab]);

	const initializeProducts = useCallback(async () => {
		const data = await getKwekerProducts().catch(null);
		const mappedProducts: Product[] = data.products.map((p) => {
			const newP: Product = {
				id: p.id,
				title: p.name,
				description: p.description,
				price: p.price,
			};
			return newP;
		});
		setProducts(mappedProducts);

		// Try to fetch aggregated stats from backend; if it fails, we'll fall back to client-side derived stats
		try {
			const s = await getKwekerStats().catch(() => null);
			if (s) {
				setStats(s as KwekerStats);
			}
		} catch (err) {
			// ignore and fallback to derived stats
		}
	}, []);

	// derived fallback values (minimal): prefer server stats; fallback to 0 to avoid counting unsold inventory
	const derivedTotalProducts = products.length;
	const derivedTotalRevenue = 0; // do not sum product prices — revenue must come from Orders (sales history)
	const derivedActiveAuctions = 0;
	const derivedStemsSold = 0;

	return (
		<Page enableHeader enableFooter className="kweker-page">
			<main className="kweker-container">
				<section className="kweker-hallo">
					<h1>Hallo, kweker!</h1>
					<p className="kweker-desc">Welkom op de dashboard pagina! Bekijk hier uw producten!</p>
				</section>

				<section className="kweker-stats">
					<div className="kweker-stat-card">
						<div className="stat-label">Producten aangeboden</div>
						<div className="stat-value">{products.length}</div>
					</div>

					<div className="kweker-stat-card">
						<div className="stat-label">Producten verkocht</div>
						<div className="stat-value">{stats?.activeAuctions ?? 0}</div>
					</div>

					<div className="kweker-stat-card">
						<div className="stat-label">Totale omzet</div>
						<div className="stat-value">{formatEur(stats?.totalRevenue ?? 0)}</div>
					</div>

					<div className="kweker-stat-card">
						<div className="stat-label">Bloemstelen verkocht</div>
						<div className="stat-value">{stats?.stemsSold ?? 0}</div>
					</div>
				</section>

				<section className="kweker-tabs-wrap">
					<div className="kweker-tabs">
						<button className={`kweker-tab ${activeTab === 'my' ? 'active' : ''}`} onClick={() => setActiveTab('my')}>
							Mijn producten
						</button>
						<button className={`kweker-tab ${activeTab === 'history' ? 'active' : ''}`} onClick={() => setActiveTab('history')}>
							Product geschiedenis
						</button>
					</div>
				</section>

				<section className="kweker-content">
					<div className="content-inner">
						<div className="product-grid">
							{products.map((p) => (
								<article key={p.id} className="product-card clickable" role="button" tabIndex={0} onClick={async () => {
									// open preview modal and fetch details
									setShowPreview(true);
									setPreviewLoading(true);
									setPreviewError(null);
									setPreviewProduct(null);
									try {
										const res = await getProductById(Number(p.id));
										setPreviewProduct(res as ProductDetails);
									} catch (err) {
										console.error('Failed to load product preview', err);
										setPreviewError('Kon het product niet laden.');
									} finally {
										setPreviewLoading(false);
									}
								}} onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') { (e.currentTarget as HTMLElement).click(); } }}>
									<div className="product-info-kweker">
										<h3 className="product-title">{p.title}</h3>
										<p className="product-desc">{p.description}</p>
									</div>
									<div className="product-price">
										<div className="product-price">{formatEur(p.price)}</div>
										<div className="product-thumb" />
									</div>
								</article>
							))}
						</div>
						<div className="content-footer">
							{activeTab === 'my' && (
								<>
									{ !showForm && (
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
										<div className="modal-overlay" onClick={() => {
											if (!isCreating) {
												setSavedValues(getValues());
												setShowForm(false);
											}
										}}>
											<div className="modal" onClick={(e) => e.stopPropagation()} role="dialog" aria-modal="true">
												<div className="modal-header">
													<h3>Nieuw product toevoegen</h3>
													<button className="modal-close" onClick={() => {
														setSavedValues(getValues());
														setShowForm(false);
													}} aria-label="Sluiten">✕</button>
												</div>
												<div className="modal-body">
													<form onSubmit={handleSubmit(async (data) => {
														if (isCreating) return;
														setIsCreating(true);
														try {
															// Basic client-side check
															if (!data.name || data.price === '' || Number(data.price) <= 0) {
																setError('name', { type: 'manual', message: 'Vul ten minste naam en een geldige prijs in.' });
																return;
															}

															const payload = {
																name: data.name,
																description: data.description || null,
																price: Number(data.price),
																minimumPrice: data.minimumPrice === '' || data.minimumPrice === undefined ? Number(data.price) : Number(data.minimumPrice),
																quantity: data.quantity === '' || data.quantity === undefined ? 1 : Number(data.quantity),
																imageUrl: data.imageUrl || null,
																size: data.size || null,
															};

															const res = await createProduct(payload as any);
															if (res && res.product) {
																const p = res.product;
																const mapped: Product = {
																	id: p.id,
																	title: p.name,
																	description: p.description,
																	price: p.price,
																};
																setProducts((prev) => [mapped, ...prev]);
																setShowForm(false);
																reset();
																setSavedValues(null);
															}
														} catch (err: any) {
															console.error('createProduct failed', err);
															const dataErr = err?.data;
															if (dataErr && typeof dataErr === 'object') {
																// Map server-side field errors to RHF fields
																for (const key of Object.keys(dataErr)) {
																	const val = dataErr[key];
																	const msg = Array.isArray(val) ? val.join(', ') : String(val);
																	try {
																		setError(key as any, { type: 'server', message: msg });
																	} catch (setErr) {
																		// If the server returns a form-level error
																		setError('name', { type: 'server', message: msg });
																	}
																}
															} else {
																// generic message
																setError('name', { type: 'server', message: String(dataErr || err?.message || 'Onbekende fout') });
															}
														} finally {
															setIsCreating(false);
														}
													})} className="create-product-form">

														<div className="form-row">
															<FormInputField id="name" label="Naam" {...register('name', { required: 'Naam is verplicht' })} isError={!!errors.name} error={errors.name?.message as string} />
														</div>

														<div className="form-row">
															<label>Beschrijving</label>
															<textarea {...register('description')} />
														</div>

														<div className="form-row">
															<FormInputField id="price" label="Prijs (€)" type="number" step="0.01" {...register('price', { required: 'Prijs is verplicht' })} isError={!!errors.price} error={errors.price?.message as string} />
														</div>

														<div className="form-row">
															<FormInputField id="minimumPrice" label="Minimum prijs (€)" type="number" step="0.01" {...register('minimumPrice')} isError={!!errors.minimumPrice} error={errors.minimumPrice?.message as string} />
														</div>

														<div className="form-row">
															<FormInputField id="quantity" label="Aantal" type="number" {...register('quantity')} isError={!!errors.quantity} error={errors.quantity?.message as string} />
														</div>

														<div className="form-row">
															<FormInputField id="imageUrl" label="Afbeelding URL" {...register('imageUrl')} isError={!!errors.imageUrl} error={errors.imageUrl?.message as string} />
														</div>

														<div className="form-row">
															<FormInputField id="size" label="Maat" {...register('size')} isError={!!errors.size} error={errors.size?.message as string} />
														</div>

														<div className="form-row form-actions">
															<button type="submit" className="toevoegen-knop btn-primary" disabled={isCreating}>{isCreating ? 'Bezig...' : 'Voeg nieuw product toe'}</button>
															<button type="button" className="cancel-btn" onClick={() => {
																if (!isCreating) {
																	setSavedValues(getValues());
																	setShowForm(false);
																}
															}} disabled={isCreating}>Annuleren</button>
														</div>
													</form>
											</div>
											</div>
										</div>
									)}

									{/* Product preview modal */}
									{showPreview && (
										<div className="modal-overlay" onClick={() => { if (!previewLoading) { setShowPreview(false); setPreviewProduct(null); setPreviewError(null); } }}>
											<div className="modal" onClick={(e) => e.stopPropagation()} role="dialog" aria-modal="true">
												<div className="modal-header">
													<h3>Product</h3>
													<button className="modal-close" onClick={() => { setShowPreview(false); setPreviewProduct(null); setPreviewError(null); }} aria-label="Sluiten">✕</button>
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
																		<div className="product-price">{formatEur(previewProduct.price)}</div>
																	</div>
																	<div className="product-description">{previewProduct.description}</div>
																	{previewProduct.quantity !== undefined && <div className="product-quantity">Aantal: {previewProduct.quantity}</div>}
																	{previewProduct.size && <div className="product-size">Maat: {previewProduct.size}</div>}
																</div>
																<div className="product-image">
																	{previewProduct.imageUrl ? (
																		<img src={previewProduct.imageUrl} alt={previewProduct.name} />
																	) : (
																		<div className="product-image-placeholder" aria-hidden="true" />
																	)}
																</div>
															</div>
														</div>
													)}
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
