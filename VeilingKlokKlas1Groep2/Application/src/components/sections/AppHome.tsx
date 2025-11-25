// External imports
import React from 'react';
import Button from '../buttons/Button';
import ButterflyParticles from '../../particles/ButterflyParticles';
import { useRootContext } from '../../contexts/RootContext';
import { AccountType } from '../../types/AccountTypes';

function AppHome() {
	const { loggedIn, account, t, navigate } = useRootContext();

	const handleNavigateToDashboard = () => {
		if (account?.accountType === AccountType.Kweker) {
			navigate('/kweker');
		} else if (account?.accountType === AccountType.Koper) {
			navigate('/dashboard/koper');
		} else if (account?.accountType === AccountType.Veilingmeester) {
			navigate('/dashboard/veilingmeester');
		}
	};

	return (
		<section id={'home'} className={'app-home'}>
			<ButterflyParticles />
			<div className={'app-home-ctn'}>
				<div className={'app-home-row z-10'}>
					<div className={'app-home-ex'}>
						<h1 className={'app-home-title'}>{t('welcome_title')}</h1>
						<h2 className={'app-home-description'}>{t('welcome_description')}</h2>
						<div className={'app-home-cta'}>
							<span className={'app-home-cta-text'}>{t('welcome_cta_text')}</span>
							<div className={'app-home-cta-btns'}>
								{!loggedIn ? (
									<>
										<Button className={'app-home-p-btn'} label={t('get_Started')} icon={'bi-person-plus-fill'} onClick={() => navigate('/register')} />
										<Button className="app-home-s-btn" label={t('login')} onClick={() => navigate('/login')} />
									</>
								) : (
									<Button className="app-home-s-btn" label={t('dashboard')} onClick={handleNavigateToDashboard} />
								)}
							</div>
						</div>
					</div>
				</div>
				<div className={'app-home-row'}>
					<img className={'app-home-img'} src={'/pictures/kweker.png'} alt={'kweker'} />
				</div>
			</div>
			<div className={'app-home-round-bt'} />
		</section>
	);
}

export default AppHome;
