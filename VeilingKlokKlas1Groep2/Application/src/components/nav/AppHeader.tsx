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
					<Link className={'nav-menu-anchor'} to="/">{window.application.t('what_is_flori_clock')}</Link>
					<Link className={'nav-menu-anchor'} to="/">{window.application.t('flower_types')}</Link>
					<Link className={'nav-menu-anchor'} to="/">{window.application.t('how_it_works')}</Link>
					<Link className={'nav-menu-anchor'} to="/">{window.application.t('contact_us')}</Link>
				</ul>
			</nav>

			<LanguagePicker/>
		</header>
	);
}

export default AppHeader;