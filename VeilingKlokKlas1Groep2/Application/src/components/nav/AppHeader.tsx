// External imports
import React from 'react';
import {Link} from "react-router-dom";

function AppHeader() {
	return (
		<div className="app-header">
			<div className={'header-logo'}>
				<img className={'header-logo-img'} src={'/svg/logo-floriclock.svg'} alt={'FloriClock'}/>
			</div>

			<nav className="header-nav">
				<ul className={'nav-menu'}>
					<Link to="/">{}</Link>
				</ul>
			</nav>
		</div>
	);
}

export default AppHeader;