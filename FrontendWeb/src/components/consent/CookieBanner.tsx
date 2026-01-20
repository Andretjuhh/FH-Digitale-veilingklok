import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

const cookieKey = 'floriclock.cookies.accepted';

export default function CookieBanner() {
	const [visible, setVisible] = useState(false);

	useEffect(() => {
		const accepted = localStorage.getItem(cookieKey) === 'true';
		setVisible(!accepted);
	}, []);

	if (!visible) {
		return null;
	}

	const accept = () => {
		localStorage.setItem(cookieKey, 'true');
		setVisible(false);
	};

	return (
		<div className="cookie-banner" role="dialog" aria-live="polite">
			<div className="cookie-banner__content">
				<p className="cookie-banner__text">
					Wij gebruiken cookies voor basisfunctionaliteit en om de ervaring te verbeteren.
				</p>
				<div className="cookie-banner__actions">
					<Link to="/cookies" className="cookie-banner__link">Cookieverklaring</Link>
					<button type="button" className="cookie-banner__button" onClick={accept}>
						Akkoord
					</button>
				</div>
			</div>
		</div>
	);
}
