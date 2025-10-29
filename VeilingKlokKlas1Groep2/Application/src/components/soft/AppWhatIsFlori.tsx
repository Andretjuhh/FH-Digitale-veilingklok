import React from 'react';
import Button from "../buttons/Button";
import WhatIsCard from "../elements/WhatIsCard";

function AppWhatIsFlori() {
	return (
		<div className={'app-what-is'}>
			<div className={'app-what-is-info-ctn'}>
				<h2 className={'app-what-is-title'}>{window.application.t('what_is_flori_clock')}</h2>
				<span className={'app-what-is-txt'}>{window.application.t('what_is_flori_clock_description')}</span>
				<Button className={'app-home-s-btn app-what-is-btn'} label={window.application.t('contact_us')}/>
			</div>
			<div className={'app-what-is-steps-ctn'}>
				<WhatIsCard
					title={window.application.t('buyer')}
					description={window.application.t('what_do_buyer')}
					icon={'bi-flower2'}
				/>
				<WhatIsCard
					title={window.application.t('growers')}
					description={window.application.t('what_do_growers')}
					icon={'bi-person-arms-up'}
				/>
				<WhatIsCard
					title={window.application.t('veilingmeesters')}
					description={window.application.t('what_do_veilingmeesters')}
					icon={'bi-gear-fill'}
				/>
				<WhatIsCard
					title={window.application.t('veilingklok')}
					description={window.application.t('what_is_veilingklok')}
					icon={'bi-clock-history'}
				/>
			</div>
		</div>
	);
}

export default AppWhatIsFlori;