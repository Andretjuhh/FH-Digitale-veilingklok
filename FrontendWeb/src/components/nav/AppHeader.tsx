// External imports
import React, {useEffect, useMemo, useState} from 'react';
import {useLocation} from 'react-router-dom';
import clsx from 'clsx';

// Internal imports
import LanguageDropdown from '../buttons/LanguageDropdown';
import {useRootContext} from '../contexts/RootContext';
import AccountAvatar from "../buttons/AccountAvatar";
import {AccountType} from "../../declarations/enums/AccountTypes";
import NavBar from "./NavBar";

function AppHeader(props: { className?: string, slideAnimation?: boolean }) {
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
		if (props.slideAnimation == true)
			window.addEventListener('scroll', handleScroll);
		return () => {
			window.removeEventListener('scroll', handleScroll);
		};
	}, []);

	const headerBottom = useMemo(() => {
		return loggedIn ? <AccountAvatar/> : <LanguageDropdown/>
	}, [loggedIn]);

	return (
		<>
			<header className={clsx('app-header', props.className, isFilled && 'app-header-filled')} style={{'--scroll-scale': scrollScale} as React.CSSProperties}>
				<a className={'header-logo'} href={'/'}>
					<img className={'header-logo-img'} src={'/svg/logo-floriclock.svg'} alt={'FloriClock'}/>
				</a>
				<div className={'navbar-ctn'}>
					{['', '/'].includes(location.pathname) ? (
						<nav className={'nav-menu'}>
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
						</nav>
					) : (
						<>
							{
								account?.accountType == AccountType.Kweker &&
								<NavBar
									pages={[
										{key: 'dashboard', name: t('dashboard'), location: '/kweker/dashboard', icon: 'bi-grid-fill'},
										{key: 'products', name: t('products'), location: '/kweker/products', icon: 'bi-bag-fill'},
										{key: 'orders', name: t('orders'), location: '/kweker/orders', icon: 'bi-cart-fill'},
									]}
								/>
							}

							{
								account?.accountType == AccountType.Koper &&
								<NavBar
									pages={[
										{key: 'dashboard', name: t('dashboard'), location: '/koper/dashboard', icon: 'bi-grid-fill'},
										{key: 'browse', name: t('browse_flowers'), location: '/koper/browse', icon: 'bi-search'},
										{key: 'orders', name: t('my_orders'), location: '/koper/orders', icon: 'bi-cart-fill'},
									]}
								/>
							}

							{
								account?.accountType == AccountType.Veilingmeester &&
								<NavBar
									pages={[
										{key: 'manage_auction', name: t('manage_auction'), location: '/veilingmeester/veilingen-beheren', icon: 'bi-grid-fill'},
										{key: 'auctions', name: t('auctions'), location: '/veilingmeester/veilingen', icon: 'bi-clock-history', strict: false},
										{key: 'region_flowers', name: t('region_flowers'), location: '/veilingmeester/region-flowers', icon: 'bi-tags-fill'},
									]}
								/>
							}
						</>
					)}
				</div>
			</header>
			{headerBottom}
		</>
	);
}

export default AppHeader;
