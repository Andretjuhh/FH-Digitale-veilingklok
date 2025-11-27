import React, { useCallback, useEffect, useState } from 'react';
import Button from '../../components/buttons/Button';
import Page from '../../components/nav/Page';
import { getKwekerProducts } from '../../controllers/kweker';
import { createProduct } from '../../controllers/Product';

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

const initialStats: KwekerStats = {
	totalProducts: 12,
	activeAuctions: 10,
	totalRevenue: 12345.69,
	stemsSold: 128,
};

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

function formatEur(value: number) {
	return value.toLocaleString('nl-NL', { style: 'currency', currency: 'EUR', minimumFractionDigits: 2 });
}

export default function KwekerDashboard() {
	const [stats] = useState<KwekerStats>(initialStats);
	const [activeTab, setActiveTab] = useState<'my' | 'history'>('my');
	const [products, setProducts] = useState<Product[]>(activeTab === 'my' ? sampleProducts : sampleHistory);
	const [isCreating, setIsCreating] = useState(false);
	const [showForm, setShowForm] = useState(false);
	const [name, setName] = useState('');
	const [description, setDescription] = useState('');
	const [price, setPrice] = useState<number | ''>('');
	const [minimumPrice, setMinimumPrice] = useState<number | ''>('');
	const [quantity, setQuantity] = useState<number | ''>('');
	const [imageUrl, setImageUrl] = useState('');
	const [size, setSize] = useState('');

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
	}, []);

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
						<div className="stat-value">{stats.activeAuctions}</div>
					</div>

					<div className="kweker-stat-card">
						<div className="stat-label">Totale omzet</div>
						<div className="stat-value">{formatEur(stats.totalRevenue)}</div>
					</div>

					<div className="kweker-stat-card">
						<div className="stat-label">Bloemstelen verkocht</div>
						<div className="stat-value">{stats.stemsSold}</div>
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
								<article key={p.id} className="product-card">
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
									{!showForm && (
										<Button
											className="toevoegen-knop"
											label={'Voeg nieuw product toe'}
											onClick={() => setShowForm(true)}
										>
											Toevoegen
										</Button>
									)}
									{showForm && (
										<div className="modal-overlay" onClick={() => !isCreating && setShowForm(false)}>
											<div className="modal" onClick={(e) => e.stopPropagation()} role="dialog" aria-modal="true">
												<div className="modal-header">
													<h3>Nieuw product toevoegen</h3>
													<button className="modal-close" onClick={() => setShowForm(false)} aria-label="Sluiten">✕</button>
												</div>
												<div className="modal-body">
													<form
				onSubmit={async (e) => {
						e.preventDefault();
						if (isCreating) return;
						// basic validation
						if (!name || price === '' || Number(price) <= 0) {
							alert('Vul ten minste naam en een geldige prijs in.');
							return;
						}
						setIsCreating(true);
						try {
							const payload = {
								name,
								description,
								price: Number(price),
								minimumPrice: minimumPrice === '' ? Number(price) : Number(minimumPrice),
								quantity: quantity === '' ? 1 : Number(quantity),
								imageUrl,
								size,
							};
							const res = await createProduct(payload).catch((err) => {
								console.error('createProduct failed', err);
								return null;
							});
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
								// reset form
								setName('');
								setDescription('');
								setPrice('');
								setMinimumPrice('');
								setQuantity('');
								setImageUrl('');
								setSize('');
							} else {
								alert('Het aanmaken van het product is mislukt.');
							}
						} finally {
							setIsCreating(false);
						}
					}}
					className="create-product-form"
					>
					<div className="form-row">
						<label>Naam</label>
						<input value={name} onChange={(e) => setName(e.target.value)} />
					</div>
					<div className="form-row">
						<label>Beschrijving</label>
						<textarea value={description} onChange={(e) => setDescription(e.target.value)} />
					</div>
					<div className="form-row">
						<label>Prijs (€)</label>
						<input type="number" step="0.01" value={price} onChange={(e) => setPrice(e.target.value === '' ? '' : Number(e.target.value))} />
					</div>
					<div className="form-row">
						<label>Minimum prijs (€)</label>
						<input type="number" step="0.01" value={minimumPrice} onChange={(e) => setMinimumPrice(e.target.value === '' ? '' : Number(e.target.value))} />
					</div>
					<div className="form-row">
						<label>Aantal</label>
						<input type="number" value={quantity} onChange={(e) => setQuantity(e.target.value === '' ? '' : Number(e.target.value))} />
					</div>
					<div className="form-row">
						<label>Afbeelding URL</label>
						<input value={imageUrl} onChange={(e) => setImageUrl(e.target.value)} />
					</div>
					<div className="form-row">
						<label>Maat</label>
						<input value={size} onChange={(e) => setSize(e.target.value)} />
					</div>
					<div className="form-row form-actions">
						<button type="submit" className="toevoegen-knop btn-primary" disabled={isCreating}>{isCreating ? 'Bezig...' : 'Voeg nieuw product toe'}</button>
						<button type="button" className="cancel-btn" onClick={() => setShowForm(false)} disabled={isCreating}>Annuleren</button>
					</div>
					</form>
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
