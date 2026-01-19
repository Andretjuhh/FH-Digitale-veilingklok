import React from 'react';
import { useRootContext } from '../../contexts/RootContext';

function AppFooter() {
	const { t } = useRootContext();
	return (
		<footer className="app-footer">
			<div className="user-footer-col" aria-labelledby="app-footer-about-title">
				<h4 id="app-footer-about-title" className="user-footer-title">Over FloriClock</h4>
				<p className="user-footer-line">Digitale veiling voor bloemen en planten.</p>
				<p className="user-footer-line">Gebouwd door studenten — demo omgeving.</p>
			</div>
			<div className="user-footer-col" aria-labelledby="app-footer-product-title">
				<h4 id="app-footer-product-title" className="user-footer-title">Product</h4>
				<ul className="user-footer-list">
					<li><a href="#" aria-label={t('aria_footer_live_auction')}>Live veiling</a></li>
					<li><a href="#" aria-label={t('aria_footer_price_history')}>Prijsgeschiedenis</a></li>
					<li><a href="#" aria-label={t('aria_footer_favorites')}>Favorieten</a></li>
				</ul>
			</div>
			<div className="user-footer-col" aria-labelledby="app-footer-resources-title">
				<h4 id="app-footer-resources-title" className="user-footer-title">Resources</h4>
				<ul className="user-footer-list">
					<li><a href="#" aria-label={t('aria_footer_docs')}>Documentatie</a></li>
					<li><a href="#" aria-label={t('aria_footer_faq')}>Veelgestelde vragen</a></li>
					<li><a href="#" aria-label={t('aria_footer_status')}>Status</a></li>
				</ul>
			</div>
			<div className="user-footer-col" aria-labelledby="app-footer-contact-title">
				<h4 id="app-footer-contact-title" className="user-footer-title">Contact</h4>
				<ul className="user-footer-list">
					<li><a href="#" aria-label={t('aria_footer_support')}>Service & support</a></li>
					<li><a href="#" aria-label={t('aria_footer_contact_form')}>Contactformulier</a></li>
					<li><a href="#" aria-label={t('aria_footer_locations')}>Locaties</a></li>
				</ul>
			</div>
		</footer>
	);
}

export default AppFooter;
