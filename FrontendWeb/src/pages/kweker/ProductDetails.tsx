import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getProductDetails } from '../../controllers/server/kweker';
import Page from '../../components/nav/Page';
import { ProductDetailsOutputDto } from '../../declarations/dtos/output/ProductDetailsOutputDto';
import { useRootContext } from '../../components/contexts/RootContext';

export default function ProductDetails() {
	const { id } = useParams<{ id: string }>();
	const navigate = useNavigate();
	const { t } = useRootContext();
	const [product, setProduct] = useState<ProductDetailsOutputDto | null>(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		if (!id) return;
		setLoading(true);
		getProductDetails(id)
			.then((res) => {
				if (res.data) {
					setProduct(res.data);
				}
			})
			.catch((err) => {
				console.error('Failed to load product', err);
				setError('Kon het product niet laden.');
			})
			.finally(() => setLoading(false));
	}, [id]);

	return (
		<Page enableHeader enableFooter>
			<main className="product-details-page">
				<div className="container">
					<button className="back-button" aria-label={t('aria_product_details_back')} onClick={() => navigate(-1)}>
						Terug
					</button>

					{loading && <div role="status" aria-label={t('aria_product_details_loading')}>Bezig met laden...</div>}
					{error && <div className="error" role="alert" aria-label={t('aria_product_details_error')}>{error}</div>}

					{product && (
						<div className="product-details" aria-labelledby="product-details-title">
							<h1 id="product-details-title">{product.name}</h1>
							<div className="product-meta">
								<div className="product-price">€{(product.auctionPrice || product.minimumPrice).toFixed(2)}</div>
								{product.dimension && <div className="product-size">Maat: {product.dimension}</div>}
								{product.stock !== undefined && <div className="product-quantity">Aantal: {product.stock}</div>}
							</div>
							<div className="product-description">{product.description}</div>
							{product.imageBase64 && (
								<div className="product-image">
									<img src={product.imageBase64} alt={product.name} />
								</div>
							)}
						</div>
					)}
				</div>
			</main>
		</Page>
	);
}
