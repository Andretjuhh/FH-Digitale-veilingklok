// External imports
import React from 'react';

// Internal imports
import Page from '../../components/nav/Page';

function Privacy() {
	return (
		<Page enableHeader enableFooter className="legal-page">
			<div className="legal-page__content">
				<h1>Privacyverklaring</h1>
				<p>
					In deze verklaring leggen we uit welke persoonsgegevens we gebruiken,
					waarom we dat doen, en welke rechten je hebt.
				</p>

				<h2>Welke gegevens verwerken wij</h2>
				<ul>
					<li>Accountgegevens zoals naam, e-mail en wachtwoord (versleuteld).</li>
					<li>Transactiegegevens zoals biedingen, aankopen en orders.</li>
					<li>Technische gegevens zoals IP-adres, browser en loggegevens.</li>
				</ul>

				<h2>Waarvoor gebruiken wij deze gegevens</h2>
				<ul>
					<li>Het aanmaken en beheren van je account.</li>
					<li>Het uitvoeren van veilingen en bestellingen.</li>
					<li>Beveiliging, monitoring en het voorkomen van misbruik.</li>
					<li>Klantenservice en communicatie over je account.</li>
				</ul>

				<h2>Bewaartermijnen</h2>
				<p>
					Wij bewaren gegevens zolang dat nodig is voor de doelen hierboven,
					en zolang wettelijk verplicht.
				</p>

				<h2>Jouw rechten</h2>
				<ul>
					<li>Inzage, correctie en verwijdering van je gegevens.</li>
					<li>Bezwaar maken tegen verwerking of beperking vragen.</li>
					<li>Gegevensoverdracht aanvragen waar van toepassing.</li>
				</ul>

				<h2>Contact</h2>
				<p>
					Vragen over privacy? Neem contact op via support@floriclock.example.
				</p>
			</div>
		</Page>
	);
}

export default Privacy;
