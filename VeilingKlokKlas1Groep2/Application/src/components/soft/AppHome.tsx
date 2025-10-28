// External imports
import React from 'react';

function AppHome() {
	return (
		<div className={'app-home'}>
			<div className={'app-home-ctn'}>
				<div className={'app-home-row app-home-ex'}>
					<h1 className={'app-home-title'}>{window.application.t('welcome_title')}</h1>
					<h2 className={'app-home-description'}>{window.application.t('welcome_description')}</h2>
				</div>
				<div className={'app-home-row'}>
					<img className={'app-home-img'} src={'/pictures/kweker.png'} alt={'kweker'}/>
				</div>
			</div>
		</div>
	);
}

export default AppHome;