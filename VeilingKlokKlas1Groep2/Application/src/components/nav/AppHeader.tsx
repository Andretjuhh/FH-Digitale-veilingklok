// External imports
import React, { useEffect, useState } from 'react';

// Internal imports
import LanguagePicker from '../buttons/LanguagePicker';
import clsx from 'clsx';

function AppHeader() {
	const [scrollScale, setScrollScale] = useState(0);
	const [isFilled, setIsFilled] = useState(false);

	useEffect(() => {
		const handleScroll = () => {
			const scrollTop = window.scrollY;
			// Animate the ripple over the first 200px of scrolling
			const scrollFraction = Math.min(scrollTop / 200, 1);
			setScrollScale(scrollFraction);
			setIsFilled(scrollFraction === 1);
		};

		window.addEventListener('scroll', handleScroll);

		return () => {
			window.removeEventListener('scroll', handleScroll);
		};
	}, []);

	return (
		<header className={clsx('app-header', isFilled && 'app-header-filled')} style={{ '--scroll-scale': scrollScale } as React.CSSProperties}>
			<div className={'header-logo'}>
				<img className={'header-logo-img'} src={'/svg/logo-floriclock.svg'} alt={'FloriClock'} />
			</div>

			<nav className="header-nav">
				<ul className={'nav-menu'}>
					<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'}>
						{window.application.t('what_is_flori_clock')}
					</a>
					<a className={'nav-menu-anchor'} href={'#soort-bloemen'}>
						{window.application.t('flower_types')}
					</a>
					<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'}>
						{window.application.t('how_it_works')}
					</a>
					<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'}>
						{window.application.t('contact_us')}
					</a>
				</ul>
			</nav>

			<LanguagePicker />
		</header>
	);
}

export default AppHeader;
