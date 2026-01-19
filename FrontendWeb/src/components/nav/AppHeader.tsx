// External imports
import React, {useEffect, useMemo, useState} from 'react';
import {useLocation} from 'react-router-dom';
import clsx from 'clsx';

// Internal imports
import LanguageDropdown from '../buttons/LanguageDropdown';
import {useRootContext} from '../contexts/RootContext';
import AccountAvatar from '../buttons/AccountAvatar';
import {AccountType} from '../../declarations/enums/AccountTypes';
import NavBar from './NavBar';

function AppHeader(props: { className?: string; slideAnimation?: boolean }) {
	let location = useLocation();
	const {loggedIn, t, account} = useRootContext();

	const [scrollScale, setScrollScale] = useState(0);
	const [isFilled, setIsFilled] = useState(false);

	useEffect(() => {
		const handleScroll = () => {
			const scrollTop = window.scrollY;
			const scrollFraction = Math.min(scrollTop / 100, 1);
			setScrollScale(scrollFraction);
			setIsFilled(scrollFraction === 1);
		};
		if (props.slideAnimation == true) window.addEventListener('scroll', handleScroll);
		return () => {
			window.removeEventListener('scroll', handleScroll);
		};
	}, []);

	const headerBottom = useMemo(() => {
		return loggedIn ? <AccountAvatar/> : <LanguageDropdown/>;
	}, [loggedIn]);

	return (
		<>
			<header className={clsx('app-header', props.className, isFilled && 'app-header-filled')} style={{'--scroll-scale': scrollScale} as React.CSSProperties}>
				<a className={'header-logo'} href={'/'} aria-label={t('aria_nav_home')}>
					<img className={'header-logo-img'} src={'/svg/logo-floriclock.svg'} alt={'FloriClock'}/>
				</a>
				<div className={'navbar-ctn'}>
					{['', '/'].includes(location.pathname) ? (
						<nav className={'nav-menu'} aria-label={t('aria_nav_home_menu')}>
							<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'} aria-label={t('aria_nav_what_is')}>
								{t('what_is_flori_clock')}
							</a>
							<a className={'nav-menu-anchor'} href={'#soort-bloemen'} aria-label={t('aria_nav_flower_types')}>
								{t('flower_types')}
							</a>
							<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'} aria-label={t('aria_nav_how_it_works')}>
								{t('how_it_works')}
							</a>
							<a className={'nav-menu-anchor'} href={'#what-is-flori-clock'} aria-label={t('aria_nav_contact')}>
								{t('contact_us')}
							</a>
						</nav>
					) : (
						<>
							{account?.accountType == AccountType.Kweker && (
								<NavBar
									pages={[
										{key: 'dashboard', name: t('dashboard'), location: '/kweker/dashboard', icon: 'bi-grid-fill', ariaLabel: t('aria_nav_dashboard')},
										{key: 'products', name: t('products'), location: '/kweker/products', icon: 'bi-bag-fill', ariaLabel: t('aria_nav_products')},
										{key: 'orders', name: t('orders'), location: '/kweker/orders', icon: 'bi-cart-fill', ariaLabel: t('aria_nav_orders')},
									]}
								/>
							)}

							{account?.accountType == AccountType.Koper && (
								<NavBar
									pages={[
										{key: 'auctions', name: t('auctions'), location: '/koper/veilingen', icon: 'bi-grid-fill', strict: false, ariaLabel: t('aria_nav_auctions')},
										{key: 'browse', name: t('browse_flowers'), location: '/koper/zoeken', icon: 'bi-search', ariaLabel: t('aria_nav_browse_flowers')},
										{key: 'orders', name: t('my_orders'), location: '/koper/orders', icon: 'bi-cart-fill', strict: false, ariaLabel: t('aria_nav_my_orders')},
									]}
								/>
							)}

							{account?.accountType == AccountType.Veilingmeester && (
								<NavBar
									pages={[
										{key: 'manage_auction', name: t('manage_auction'), location: '/veilingmeester/veilingen-beheren', icon: 'bi-grid-fill', strict: false, ariaLabel: t('aria_nav_manage_auction')},
										{key: 'auctions', name: t('auctions'), location: '/veilingmeester/veilingen', icon: 'bi-clock-history', strict: false, ariaLabel: t('aria_nav_auction_clocks')},
										{key: 'region_flowers', name: t('region_flowers'), location: '/veilingmeester/region-flowers', icon: 'bi-tags-fill', ariaLabel: t('aria_nav_region_flowers')},
									]}
								/>
							)}
						</>
					)}
				</div>
			</header>
			{headerBottom}
		</>
	);
}

export default AppHeader;
