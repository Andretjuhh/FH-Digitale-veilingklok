import React, { useState } from 'react';
import AppHeader from '../../components/nav/AppHeader';
import Button from '../../components/buttons/Button';
import '../../styles/Kweker.css';

type KwekerStats = {
    totalProducts: number;
    activeAuctions: number;
    totalRevenue: number;
    stemsSold: number;
};

type Product = {
    id: string;
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
    { id: '2', title: 'Product 2', description: 'Beschrijving van product 2', price: 24.50 },
    { id: '3', title: 'Product 3', description: 'Beschrijving van product 3', price: 12.00 },
    { id: '4', title: 'Product 4', description: 'Beschrijving van product 4', price: 34.75 },
    { id: '5', title: 'Product 5', description: 'Beschrijving van product 5', price: 9.99 },
    { id: '6', title: 'Product 6', description: 'Beschrijving van product 6', price: 49.00 },
    { id: '7', title: 'Product 7', description: 'Beschrijving van product 7', price: 22.00 },
];

const sampleHistory: Product[] = [
    { id: 'h1', title: 'Verkocht product 1', description: 'Beschrijving verkocht item 1', price: 14.25 },
    { id: 'h2', title: 'Verkocht product 2', description: 'Beschrijving verkocht item 2', price: 45.00 },
    { id: 'h3', title: 'Verkocht product 3', description: 'Beschrijving verkocht item 3', price: 22.10 },
    { id: 'h4', title: 'Verkocht product 4', description: 'Beschrijving verkocht item 4', price: 37.80 },
    { id: 'h5', title: 'Verkocht product 5', description: 'Beschrijving verkocht item 5', price: 8.75 },
    { id: 'h6', title: 'Verkocht product 6', description: 'Beschrijving verkocht item 6', price: 59.99 },
];

function formatEur(value: number) {
    return value.toLocaleString('nl-NL', { style: 'currency', currency: 'EUR', minimumFractionDigits: 2 });
}

export default function KwekerDashboard() {
    const [stats] = useState<KwekerStats>(initialStats);
    const [activeTab, setActiveTab] = useState<'my' | 'history'>('my');
    const displayedProducts = activeTab === 'my' ? sampleProducts : sampleHistory;

    return (
        <div className="kweker-page">
            <AppHeader />
            <main className="kweker-container">
                <section className="kweker-hallo">
                    <h1>Hallo, kweker!</h1>
                    <p className="kweker-desc">Welkom op de dashboard pagina! Bekijk hier uw producten!</p>
                </section>

                <section className="kweker-stats">
                    <div className="kweker-stat-card">
                        <div className="stat-label">Producten aangeboden</div>
                        <div className="stat-value">{stats.totalProducts}</div>
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
                        <button
                            className={`kweker-tab ${activeTab === 'my' ? 'active' : ''}`}
                            onClick={() => setActiveTab('my')}
                        >
                            Mijn producten
                        </button>
                        <button
                            className={`kweker-tab ${activeTab === 'history' ? 'active' : ''}`}
                            onClick={() => setActiveTab('history')}
                        >
                            Product geschiedenis
                        </button>
                    </div>
                </section>

                <section className="kweker-content">
                    <div className="content-inner">
                        <div className="product-grid">
                            {displayedProducts.map((p) => (
                                <article key={p.id} className="product-card">
                                    <div className="product-info">
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
                                <Button className="toevoegen-knop" label={"Voeg nieuw product toe"} onClick={() => { /* open submit form */ }}>
                                    Toevoegen
                                </Button>
                            )}
                        </div>
                    </div>
                </section>
            </main>
        </div>
    );
}
