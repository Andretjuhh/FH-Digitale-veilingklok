// External imports
import React from 'react';
import {Link} from "react-router-dom";

// Internal imports
import LanguagePicker from "../buttons/LanguagePicker";

function AppHeader() {
	return (
		<header className="app-header">
			<div className={'header-logo'}>
				<img className={'header-logo-img'} src={'/svg/logo-floriclock.svg'} alt={'FloriClock'}/>
			</div>

			<nav className="header-nav">
				<ul className={'nav-menu'}>
					<a className={'nav-menu-anchor'} href={"#what-is-flori-clock"}>{window.application.t('what_is_flori_clock')}</a>
					<a className={'nav-menu-anchor'} href={"#soort-bloemen"}>{window.application.t('flower_types')}</a>
					<a className={'nav-menu-anchor'} href={"#what-is-flori-clock"}>{window.application.t('how_it_works')}</a>
					<a className={'nav-menu-anchor'} href={"#what-is-flori-clock"}>{window.application.t('contact_us')}</a>
				</ul>
			</nav>

			<LanguagePicker/>
		</header>
	);
}

export default AppHeader;