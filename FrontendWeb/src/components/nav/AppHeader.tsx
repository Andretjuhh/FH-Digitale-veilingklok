// External imports
import React, {useEffect, useMemo, useState} from 'react';
import {useLocation} from 'react-router-dom';
import clsx from 'clsx';

// Internal imports
import LanguagePicker from '../buttons/LanguagePicker';
import {useRootContext} from '../contexts/RootContext';
import AccountAvatar from "../buttons/AccountAvatar";
import {AccountType} from "../../declarations/enums/AccountTypes";

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
		return loggedIn ? <AccountAvatar/> : <LanguagePicker/>
	}, [loggedIn]);

	return (
		<>
			<header className={clsx('app-header', props.className, isFilled && 'app-header-filled')} style={{'--scroll-scale': scrollScale} as React.CSSProperties}>
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
								{
									account?.accountType == AccountType.Kweker &&
									<>
										{/*<a className={'nav-menu-anchor'} href={'#'}>*/}
										{/*	{t('dashboard')}*/}
										{/*</a>*/}

										{/*<a className={'nav-menu-anchor'} href={'#'}>*/}
										{/*	{t('koper_product')}*/}
										{/*</a>*/}

										{/*<a className={'nav-menu-anchor'} href={'#'}>*/}
										{/*	{t('orders')}*/}
										{/*</a>*/}
									</>
								}
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
