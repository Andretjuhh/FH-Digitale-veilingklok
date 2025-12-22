// External imports
import React, {useEffect, useMemo, useState} from 'react';
import {useLocation} from 'react-router-dom';

// Internal imports
import LanguagePicker from '../buttons/LanguagePicker';
import clsx from 'clsx';
import {useRootContext} from '../contexts/RootContext';
import AccountAvatar from "../buttons/AccountAvatar";

function AppHeader() {
	let location = useLocation();
	const {loggedIn, t} = useRootContext();

	const [scrollScale, setScrollScale] = useState(0);
	const [isFilled, setIsFilled] = useState(false);

	useEffect(() => {
		const handleScroll = () => {
			const scrollTop = window.scrollY;
			// Animate the ripple over the first 200px of scrolling
			const scrollFraction = Math.min(scrollTop / 100, 1);
			setScrollScale(scrollFraction);
			setIsFilled(scrollFraction === 1);
		};

		window.addEventListener('scroll', handleScroll);

		return () => {
			window.removeEventListener('scroll', handleScroll);
		};
	}, []);

	const headerBottom = useMemo(() => {
		return loggedIn ? <AccountAvatar/> : <LanguagePicker/>
	}, [loggedIn]);
	
	return (
		<>
			<header className={clsx('app-header', isFilled && 'app-header-filled')} style={{'--scroll-scale': scrollScale} as React.CSSProperties}>
				<div className={'header-logo'}>
					<img className={'header-logo-img'} src={'/svg/logo-floriclock.svg'} alt={'FloriClock'}/>
				</div>
				<nav className="header-nav">
					<ul className={'nav-menu'}>
						{['', '/'].includes(location.pathname) ? (
							<>
								<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'}>
									{t('what_is_flori_clock')}
								</a>
								<a className={'nav-menu-anchor'} href={'#soort-bloemen'}>
									{t('flower_types')}
								</a>
								<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'}>
									{t('how_it_works')}
								</a>
								<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'}>
									{t('contact_us')}
								</a>
							</>
						) : (
							<>
								<a className={'nav-menu-anchor'} href={'#'}>
									{t('dashboard')}
								</a>
								<a className={'nav-menu-anchor'} href={'#'}>
									{t('orders')}
								</a>
								<a className={'nav-menu-anchor'} href={'#'}>
									{t('manage_account')}
								</a>
								<a className={'nav-menu-anchor'} href={'#'}>
									{t('settings')}
								</a>
							</>
						)}
					</ul>
				</nav>
			</header>
			{headerBottom}
		</>
	);
}

export default AppHeader;
