import React from 'react';
import Button from '../../buttons/Button';
import WhatIsCard from '../../cards/WhatIsCard';
import {useRootContext} from '../../contexts/RootContext';

function AppWhatIsFlori() {
	const {t} = useRootContext();

	return (
		<section id={'what-is-flori-clock'} className={'app-what-is'} aria-labelledby="app-what-is-title">
			<div className={'app-what-is-info-ctn'}>
				<h2 id="app-what-is-title" className={'app-what-is-title'}>{t('what_is_flori_clock')}</h2>
				<span className={'app-what-is-txt'}>{t('what_is_flori_clock_description')}</span>
				<Button className={'app-home-s-btn app-what-is-btn'} label={t('contact_us')} aria-label={t('aria_what_is_contact')}/>
			</div>
			<div className={'app-what-is-steps-ctn'}>
				<WhatIsCard title={t('buyer')} description={t('what_do_buyer')} icon={'bi-flower2'}/>
				<WhatIsCard title={t('growers')} description={t('what_do_growers')} icon={'bi-person-arms-up'}/>
				<WhatIsCard title={t('veilingmeesters')} description={t('what_do_veilingmeesters')} icon={'bi-gear-fill'}/>
				<WhatIsCard title={t('veilingklok')} description={t('what_is_veilingklok')} icon={'bi-clock-history'}/>
			</div>
		</section>
	);
}

export default AppWhatIsFlori;
