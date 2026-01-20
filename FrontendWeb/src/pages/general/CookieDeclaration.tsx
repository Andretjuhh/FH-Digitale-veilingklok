// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';

function CookieDeclaration() {
	return (
		<Page enableHeader enableFooter className="legal-page">
			<div className="legal-page__content">
				<h1>Cookieverklaring</h1>
				<p>
					Wij gebruiken cookies en vergelijkbare technieken om de website
					goed te laten werken en de gebruikerservaring te verbeteren.
				</p>

				<h2>Welke cookies gebruiken wij</h2>
				<ul>
					<li>Functionele cookies: noodzakelijk voor inloggen en sessies.</li>
					<li>Voorkeurscookies: onthouden jouw instellingen.</li>
				</ul>

				<h2>Bewaartermijnen</h2>
				<p>
					Functionele cookies blijven geldig zolang je ingelogd bent of tot ze
					verlopen. Voorkeurscookies blijven bewaard tot je ze verwijdert.
				</p>

				<h2>Cookies beheren</h2>
				<p>
					Je kunt cookies verwijderen of blokkeren via je browserinstellingen.
					Houd er rekening mee dat de website dan mogelijk minder goed werkt.
				</p>

				<h2>Contact</h2>
				<p>
					Vragen over cookies? Neem contact op via support@floriclock.example.
				</p>
			</div>
		</Page>
	);
}

export default CookieDeclaration;
