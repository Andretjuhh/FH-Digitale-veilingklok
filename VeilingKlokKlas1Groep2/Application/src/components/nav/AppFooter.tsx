import React from "react";

export default function AppFooter() {
	return (
		// <footer className="rfh-footer" role="contentinfo">
		// 	<div className="rfh-footer__top">
		// 		<div className="rfh-footer__brand">
		// 			<div className="rfh-footer__logo" aria-hidden>VK</div>
		// 			<div className="rfh-footer__title">Veilingplatform</div>
		// 		</div>
		//
		// 		<nav className="rfh-footer__links" aria-label="Footer links">
		// 			<ul>
		// 				<li><Link to="/status">Storingen / onderhoud</Link></li>
		// 				<li><Link to="/apps">Onze apps</Link></li>
		// 				<li><Link to="/pers">Pers & media</Link></li>
		// 				<li><Link to="/nieuwsbrief">Aanmelden nieuwsbrief</Link></li>
		// 				<li><Link to="/intranet">Intranet</Link></li>
		// 			</ul>
		// 			<ul>
		// 				<li><Link to="/service">Service & contact</Link></li>
		// 				<li><Link to="/remote-help">Hulp op afstand</Link></li>
		// 				<li><a href="https://wa.me/31600000000" target="_blank" rel="noreferrer">WhatsApp</a></li>
		// 				<li><Link to="/contact">Contactformulier</Link></li>
		// 				<li><Link to="/locaties">Locaties</Link></li>
		// 			</ul>
		// 		</nav>
		//
		// 		<div className="rfh-footer__social" aria-label="Social media">
		// 			<a aria-label="LinkedIn" href="#" className="soc"><LinkedInIcon/></a>
		// 			<a aria-label="YouTube" href="#" className="soc"><YouTubeIcon/></a>
		// 			<a aria-label="Facebook" href="#" className="soc"><FacebookIcon/></a>
		// 			<a aria-label="Instagram" href="#" className="soc"><InstagramIcon/></a>
		// 		</div>
		//
		// 		{/* decoratieve curve */}
		// 		<div className="rfh-footer__curve" aria-hidden/>
		// 	</div>
		//
		// 	<div className="rfh-footer__bottom">
		// 		<div>© {new Date().getFullYear()} Veilingplatform</div>
		// 		<nav aria-label="Juridisch">
		// 			<a href="#">Privacyverklaring</a>
		// 			<a href="#">Cookieverklaring</a>
		// 			<a href="#">CVD</a>
		// 			<a href="#">Algemene voorwaarden</a>
		// 		</nav>
		// 	</div>
		// </footer>
		<footer className="app-footer">
			<div className="user-footer-col">
				<h4 className="user-footer-title">Over FloriClock</h4>
				<p className="user-footer-line">Digitale veiling voor bloemen en planten.</p>
				<p className="user-footer-line">Gebouwd door studenten — demo omgeving.</p>
			</div>
			<div className="user-footer-col">
				<h4 className="user-footer-title">Product</h4>
				<ul className="user-footer-list">
					<li><a href="#">Live veiling</a></li>
					<li><a href="#">Prijsgeschiedenis</a></li>
					<li><a href="#">Favorieten</a></li>
				</ul>
			</div>
			<div className="user-footer-col">
				<h4 className="user-footer-title">Resources</h4>
				<ul className="user-footer-list">
					<li><a href="#">Documentatie</a></li>
					<li><a href="#">Veelgestelde vragen</a></li>
					<li><a href="#">Status</a></li>
				</ul>
			</div>
			<div className="user-footer-col">
				<h4 className="user-footer-title">Contact</h4>
				<ul className="user-footer-list">
					<li><a href="#">Service & support</a></li>
					<li><a href="#">Contactformulier</a></li>
					<li><a href="#">Locaties</a></li>
				</ul>
			</div>
		</footer>
	);
}
