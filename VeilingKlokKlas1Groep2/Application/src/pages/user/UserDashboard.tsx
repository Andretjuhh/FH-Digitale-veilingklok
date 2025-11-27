// External imports
import React, { useEffect, useMemo, useState } from 'react';

// Internal imports
import Page from '../../components/nav/Page';
import Button from '../../components/buttons/Button';
import AuctionClock from '../../components/elements/AuctionClock';
import { useRootContext } from '../../contexts/RootContext';

function UserDashboard() {
	const { t } = useRootContext();
	const CLOCK_SECONDS = 4;
	const [price, setPrice] = useState<number>(0.65);
	// Productenlijst (dummy data)
	type Product = {
		supplier: string;
		avr: string;
		name: string;
		land: string;
		mps: string;
		brief: string;
		kwa: string;
		qi: string;
		minStemLen: string;
		stemsPerBundle: string;
		ripeness: string;
		image: string;
	};

	const products = useMemo<Product[]>(
		() => [
			{
				supplier: 'Kees van Os',
				avr: '4177',
				name: 'R Gr Red Naomi!',
				land: 'NL',
				mps: 'A',
				brief: '32214a',
				kwa: 'A1',
				qi: 'A',
				minStemLen: '50 cm',
				stemsPerBundle: '10',
				ripeness: '3-3',
				image: '/pictures/plant 4.png',
			},
			{
				supplier: 'Flora BV',
				avr: '5032',
				name: 'Tulipa Yellow King',
				land: 'NL',
				mps: 'A',
				brief: '11802b',
				kwa: 'A2',
				qi: 'B',
				minStemLen: '40 cm',
				stemsPerBundle: '20',
				ripeness: '2-3',
				image: '/pictures/plant 2.png',
			},
			{
				supplier: 'BloomCo',
				avr: '6120',
				name: 'Gerbera Mix',
				land: 'NL',
				mps: 'B',
				brief: '90211c',
				kwa: 'A1',
				qi: 'A',
				minStemLen: '45 cm',
				stemsPerBundle: '15',
				ripeness: '2-2',
				image: '/pictures/plant 1.png',
			},
			{
				supplier: 'GreenFields',
				avr: '7102',
				name: 'Chrysanthemum White',
				land: 'DE',
				mps: 'A',
				brief: '55231a',
				kwa: 'A2',
				qi: 'A',
				minStemLen: '55 cm',
				stemsPerBundle: '12',
				ripeness: '3-4',
				image: '/pictures/plant 3.png',
			},
			{
				supplier: 'PetalWorks',
				avr: '8215',
				name: 'Peony Coral Charm',
				land: 'FR',
				mps: 'A',
				brief: '77421d',
				kwa: 'A1',
				qi: 'A',
				minStemLen: '45 cm',
				stemsPerBundle: '8',
				ripeness: '2-3',
				image: '/pictures/plant 5.png',
			},
			{
				supplier: 'Sunrise Farms',
				avr: '9340',
				name: 'Sunflower Helio',
				land: 'ES',
				mps: 'B',
				brief: '66330f',
				kwa: 'A2',
				qi: 'B',
				minStemLen: '60 cm',
				stemsPerBundle: '6',
				ripeness: '3-4',
				image: '/pictures/plant 6.png',
			},
			{
				supplier: 'Nordic Blooms',
				avr: '1055',
				name: 'Eucalyptus Cinerea',
				land: 'NO',
				mps: 'A',
				brief: '33109e',
				kwa: 'A1',
				qi: 'A',
				minStemLen: '65 cm',
				stemsPerBundle: '25',
				ripeness: '2-2',
				image: '/pictures/plant 7.png',
			},
		],
		[]
	);

	const [productIndex, setProductIndex] = useState<number>(0);
	const current = products[productIndex];
	// afbeelding bron per product
	const [imgSrc, setImgSrc] = useState<string>(current.image);
	useEffect(() => {
		setImgSrc(current.image);
	}, [current]);

	const upcoming = useMemo(() => {
		const after = products.slice(productIndex + 1);
		const before = products.slice(0, productIndex);
		return [...after, ...before];
	}, [products, productIndex]);

	const [paused, setPaused] = useState<boolean>(false);
	const [resetToken, setResetToken] = useState<number>(0);
	// Voorraad per product en aankoop hoeveelheid
	const initialStock = useMemo<number[]>(
		() =>
			products.map((p) => {
				const perBundle = parseInt(p.stemsPerBundle, 10) || 1;
				return perBundle * 30; // start met 30 bossen
			}),
		[products]
	);
	const [stock, setStock] = useState<number[]>(initialStock);
	const currentStock = stock[productIndex] ?? 0;
	const [qty, setQty] = useState<number>(5);
	useEffect(() => {
		// clamp quantity when product or stock changes
		setQty((q) => Math.max(0, Math.min(currentStock, q)));
	}, [currentStock, productIndex]);

	// prijs wordt gestuurd door de klok (onTick)

	return (
		<Page enableHeader className="user-dashboard">
			<section className="user-hero">
				<div className="user-hero-head">
					<div>
						<h1 className="user-hero-title">{t('koper_dashboard')}</h1>
						<p className="user-hero-sub">{t('koper_dashboard_sub')}</p>
					</div>
				</div>
			</section>

			<section className="user-card-wrap">
				<div className="user-card">
					{/* Left media block */}
					<div className="user-card-mediaBlock">
						<img
							className="user-card-media"
							src={imgSrc}
							onError={() => setImgSrc((prev) => (prev.endsWith('.svg') ? '/pictures/kweker.png' : '/pictures/roses.svg'))}
							alt="Rozen"
						/>
						<div className="product-info">
							<div className="prod-row">
								<span className="prod-label">{t('koper_supplier')}</span>
								<span className="prod-val">{current.supplier}</span>
								<span className="prod-label">{t('koper_avr')}</span>
								<span className="prod-val">{current.avr}</span>
							</div>
							<div className="prod-row">
								<span className="prod-label">{t('koper_product')}</span>
								<span className="prod-val prod-val--wide">{current.name}</span>
								<span className="prod-label">{t('koper_country')}</span>
								<span className="prod-val">{current.land}</span>
							</div>
							<div className="prod-row">
								<span className="prod-label">{t('koper_stems_per_bundle')}</span>
								<span className="prod-val prod-val--wide">{current.stemsPerBundle}</span>
							</div>
						</div>
					</div>

					{/* Center profile area */}
					<div className="user-card-center">
						<AuctionClock
							totalSeconds={CLOCK_SECONDS}
							start
							paused={paused}
							resetToken={resetToken}
							round={1}
							coin={1}
							amountPerLot={parseInt(current.stemsPerBundle, 10) || 1}
							minAmount={1}
							price={price}
							onTick={(secs) => {
								const p = Math.max(0, (secs / CLOCK_SECONDS) * 0.65);
								setPrice(+p.toFixed(2));
							}}
						/>

						<div className="stock-text">{t('koper_stock', { count: currentStock })}</div>

						<div className="user-actions">
							<div className="buy-controls">
								<Button
									className="user-action-btn !bg-primary-main buy-full"
									label={`${t('koper_buy')} (${qty})`}
									onClick={() => {
										const cur = stock[productIndex] ?? 0;
										const delta = Math.min(qty, cur);
										const nextVal = Math.max(0, cur - delta);
										setStock((prev) => {
											const arr = [...prev];
											arr[productIndex] = nextVal;
											return arr;
										});
										setPaused(true);
										setTimeout(() => {
											setResetToken((v) => v + 1);
											setPrice(0.65);
											setPaused(false);
											if (nextVal === 0) {
												setProductIndex((i) => (i + 1) % products.length);
											}
										}, 500);
									}}
								/>
								<div className="buy-inline">
									<input
										type="number"
										min={0}
										max={currentStock}
										className="buy-inline-input"
										value={Number.isFinite(qty) ? qty : ''}
										onChange={(e) => {
											const val = parseInt(e.target.value, 10);
											if (Number.isNaN(val)) {
												setQty(0);
												return;
											}
											setQty(Math.max(0, Math.min(currentStock, val)));
										}}
										aria-label={t('koper_qty_input_aria')}
									/>
									<Button
										className="qty-max-btn btn-outline"
										label={t('koper_max_stock')}
										aria-label={t('koper_max_stock')}
										onClick={() => setQty(currentStock)}
										disabled={currentStock === 0}
									/>
								</div>
							</div>
						</div>
					</div>

					{/* Right side: compacte wachtrij */}
					<aside className="upcoming-side">
						<h4 className="upcoming-side-title">{t('koper_upcoming')}</h4>
						<ul className="upcoming-side-list">
							{upcoming.map((p, i) => (
								<li className="upcoming-side-item" key={i}>
									<img
										className="upcoming-side-thumb"
										src={p.image}
										alt={p.name}
										onError={(e) => {
											(e.currentTarget as HTMLImageElement).src = '/pictures/kweker.png';
										}}
									/>
									<div className="upcoming-side-info">
										<div className="upcoming-side-name">{p.name}</div>
										<div className="upcoming-side-meta">
											{p.supplier} • {p.minStemLen} • {t('koper_bundle_label')} {p.stemsPerBundle}
										</div>
									</div>
									<span className="upcoming-side-badge">{p.kwa}</span>
								</li>
							))}
						</ul>
					</aside>
				</div>
			</section>

			<footer className="app-footer">
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_about_title')}</h4>
					<p className="user-footer-line">{t('koper_footer_about_line1')}</p>
					<p className="user-footer-line">{t('koper_footer_about_line2')}</p>
				</div>
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_product_title')}</h4>
					<ul className="user-footer-list">
						<li>
							<a href="#">{t('koper_footer_live')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_history')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_favorites')}</a>
						</li>
					</ul>
				</div>
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_resources_title')}</h4>
					<ul className="user-footer-list">
						<li>
							<a href="#">{t('koper_footer_docs')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_faq')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_status')}</a>
						</li>
					</ul>
				</div>
				<div className="user-footer-col">
					<h4 className="user-footer-title">{t('koper_footer_contact_title')}</h4>
					<ul className="user-footer-list">
						<li>
							<a href="#">{t('koper_footer_support')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_form')}</a>
						</li>
						<li>
							<a href="#">{t('koper_footer_locations')}</a>
						</li>
					</ul>
				</div>
			</footer>
		</Page>
	);
}

export default UserDashboard;


