import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getProductById } from '../../controllers/Product';
import Page from '../../components/nav/Page';

type Product = {
    id: number | string;
    name: string;
    description?: string | null;
    price: number;
    minimumPrice?: number | null;
    quantity?: number | null;
    imageUrl?: string | null;
    size?: string | null;
};

export default function ProductDetails() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [product, setProduct] = useState<Product | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!id) return;
        setLoading(true);
        getProductById(Number(id))
            .then((res) => {
                setProduct(res as Product);
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
                    <button className="back-button" onClick={() => navigate(-1)}>Terug</button>

                    {loading && <div>Bezig met laden...</div>}
                    {error && <div className="error">{error}</div>}

                    {product && (
                        <div className="product-details">
                            <h1>{product.name}</h1>
                            <div className="product-meta">
                                <div className="product-price">€{product.price.toFixed(2)}</div>
                                {product.size && <div className="product-size">Maat: {product.size}</div>}
                                {product.quantity !== undefined && <div className="product-quantity">Aantal: {product.quantity}</div>}
                            </div>
                            <div className="product-description">{product.description}</div>
                            {product.imageUrl && (
                                <div className="product-image">
                                    <img src={product.imageUrl} alt={product.name} />
                                </div>
                            )}
                        </div>
                    )}
                </div>
            </main>
        </Page>
    );
}
