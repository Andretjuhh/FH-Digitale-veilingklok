// External imports
import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from "../buttons/Button";
import ButterflyParticles from "../../particles/ButterflyParticles";

function AppHome() {
	const navigate = useNavigate();
	return (
		<section id={'home'} className={'app-home'}>
			<ButterflyParticles/>
			<div className={'app-home-ctn'}>
				<div className={'app-home-row z-10'}>
					<div className={'app-home-ex'}>
						<h1 className={'app-home-title'}>{window.application.t('welcome_title')}</h1>
						<h2 className={'app-home-description'}>{window.application.t('welcome_description')}</h2>
						<div className={'app-home-cta'}>
							<span className={'app-home-cta-text'}>{window.application.t('welcome_cta_text')}</span>
							<div className={'app-home-cta-btns'}>
								<Button
									className={'app-home-p-btn'}
									label={window.application.t('get_Started')}
									icon={'bi-person-plus-fill'}
								/>
								<Button
									className={'app-home-s-btn'}
									label={window.application.t('login')}
									onClick={() => navigate('/user-dashboard')}
								/>
							</div>
						</div>
					</div>
				</div>
				<div className={'app-home-row'}>
					<img className={'app-home-img'} src={'/pictures/kweker.png'} alt={'kweker'}/>
				</div>
			</div>
			<div className={'app-home-round-bt'}/>
		</section>
	);
}

export default AppHome;
