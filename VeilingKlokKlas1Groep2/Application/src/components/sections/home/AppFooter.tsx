import React from 'react';

function AppFooter() {
	return (
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

export default AppFooter;