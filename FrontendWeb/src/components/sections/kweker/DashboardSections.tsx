import React, {useEffect, useState} from 'react';
import {useRootContext} from '../../contexts/RootContext';
import {getOrders, getProducts} from '../../../controllers/server/kweker';
import {OrderKwekerOutput} from '../../../declarations/dtos/output/OrderKwekerOutput';
import {ProductOutputDto} from '../../../declarations/dtos/output/ProductOutputDto';
import {OrderStatus} from '../../../declarations/enums/OrderStatus';

export function RecentOrdersSection() {
	const {t} = useRootContext();
	const [orders, setOrders] = useState<OrderKwekerOutput[]>([]);
	const [loading, setLoading] = useState(true);

	useEffect(() => {
		const fetchRecentOrders = async () => {
			try {
				const response = await getOrders(undefined, undefined, undefined, undefined, undefined, undefined, 1, 5);
				if (response.success && response.data) {
					setOrders(response.data.data);
				}
			} catch (error) {
				console.error('Failed to fetch recent orders:', error);
			} finally {
				setLoading(false);
			}
		};

		fetchRecentOrders();
	}, []);

	const getOrderStatusBadge = (status: OrderStatus) => {
		const statusMap = {
			[OrderStatus.Open]: { class: 'badge-info', text: 'Open' },
			[OrderStatus.Processing]: { class: 'badge-warning', text: 'In behandeling' },
			[OrderStatus.Processed]: { class: 'badge-success', text: 'Verwerkt' },
			[OrderStatus.Delivered]: { class: 'badge-success', text: 'Geleverd' },
			[OrderStatus.Cancelled]: { class: 'badge-danger', text: 'Geannuleerd' },
			[OrderStatus.Returned]: { class: 'badge-warning', text: 'Geretourneerd' },
		};
		return statusMap[status] || { class: 'badge-secondary', text: 'Onbekend' };
	};

	if (loading) {
		return (
			<section className="kweker-dashboard-section">
				<h2 className="section-title">Recente bestellingen</h2>
				<p>Loading...</p>
			</section>
		);
	}

	return (
		<section className="kweker-dashboard-section">
			<div className="section-header">
				<h2 className="section-title">Recente bestellingen</h2>
				<a href="/kweker/orders" className="section-link">
					Alle bestellingen <i className="bi bi-arrow-right"></i>
				</a>
			</div>
			
			{orders.length === 0 ? (
				<div className="empty-state">
					<i className="bi bi-inbox" style={{fontSize: '3rem', opacity: 0.3}}></i>
					<p>Geen bestellingen gevonden</p>
				</div>
			) : (
				<div className="orders-list">
					{orders.map((order) => (
						<div key={order.id} className="order-card">
							<div className="order-header">
								<div className="order-info">
									<h3 className="order-product-name">{order.product.name}</h3>
									<p className="order-buyer">
										<i className="bi bi-person"></i> {order.koperInfo.firstName} {order.koperInfo.lastName}
									</p>
								</div>
								<span className={`order-status-badge ${getOrderStatusBadge(order.status).class}`}>
									{getOrderStatusBadge(order.status).text}
								</span>
							</div>
							<div className="order-details">
								<div className="order-detail-item">
									<span className="detail-label">Datum:</span>
									<span className="detail-value">
										{new Date(order.createdAt).toLocaleDateString('nl-NL')}
									</span>
								</div>
								<div className="order-detail-item">
									<span className="detail-label">Totaal:</span>
									<span className="detail-value order-total">
										€ {order.totalPrice.toFixed(2)}
									</span>
								</div>
							</div>
						</div>
					))}
				</div>
			)}
		</section>
	);
}

export function ProductsOverviewSection() {
	const {t} = useRootContext();
	const [products, setProducts] = useState<ProductOutputDto[]>([]);
	const [loading, setLoading] = useState(true);

	useEffect(() => {
		const fetchProducts = async () => {
			try {
				const response = await getProducts(undefined, undefined, undefined, 1, 6);
				if (response.success && response.data) {
					setProducts(response.data.data);
				}
			} catch (error) {
				console.error('Failed to fetch products:', error);
			} finally {
				setLoading(false);
			}
		};

		fetchProducts();
	}, []);

	if (loading) {
		return (
			<section className="kweker-dashboard-section">
				<h2 className="section-title">Mijn producten</h2>
				<p>Loading...</p>
			</section>
		);
	}

	return (
		<section className="kweker-dashboard-section">
			<div className="section-header">
				<h2 className="section-title">Mijn producten</h2>
				<a href="/kweker/products" className="section-link">
					Alle producten <i className="bi bi-arrow-right"></i>
				</a>
			</div>

			{products.length === 0 ? (
				<div className="empty-state">
					<i className="bi bi-box" style={{fontSize: '3rem', opacity: 0.3}}></i>
					<p>Nog geen producten toegevoegd</p>
					<a href="/kweker/create-product" className="btn-primary" style={{marginTop: '1rem'}}>
						<i className="bi bi-plus-circle"></i> Product toevoegen
					</a>
				</div>
			) : (
				<div className="products-grid">
					{products.map((product) => (
						<div key={product.id} className="product-card">
							<div className="product-image-wrapper">
								<img 
									src={product.imageUrl || '/pictures/placeholder.jpg'} 
									alt={product.name}
									className="product-image"
								/>
								{product.auctionPlanned && (
									<span className="product-badge auction-badge">
										<i className="bi bi-clock"></i> Actief
									</span>
								)}
							</div>
							<div className="product-info">
								<h3 className="product-name">{product.name}</h3>
								<div className="product-details">
									<div className="product-detail-row">
										<span className="detail-label">
											<i className="bi bi-tag"></i> Prijs:
										</span>
										<span className="detail-value">€ {(product.minimumPrice || 0).toFixed(2)}</span>
									</div>
									<div className="product-detail-row">
										<span className="detail-label">
											<i className="bi bi-box"></i> Voorraad:
										</span>
										<span className="detail-value">{product.stock}</span>
									</div>
									{product.region && (
										<div className="product-detail-row">
											<span className="detail-label">
												<i className="bi bi-geo-alt"></i> Regio:
											</span>
											<span className="detail-value">{product.region}</span>
										</div>
									)}
								</div>
								<a href={`/kweker/product/${product.id}`} className="product-link">
									Details bekijken <i className="bi bi-arrow-right"></i>
								</a>
							</div>
						</div>
					))}
				</div>
			)}
		</section>
	);
}
