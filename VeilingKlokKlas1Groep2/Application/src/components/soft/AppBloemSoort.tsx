// External dependencies
import React from 'react';
import CardSwap, {Card} from "../../libraries/react-bits/CardSwap";
import TinyCardCounter from "../elements/TinyCardCounter";

function AppBloemSoort() {
	return (
		<div className={'app-bloems'}>
			<div className={'app-bloems-ctn'}>
				<div className='app-bloems-hd'>
					<h2 className={'app-bloems-hd-title'}>{window.application.t('flower_types')}</h2>
					<span className={'app-bloems-hd-description'}>{window.application.t('flower_types_description')}</span>
				</div>
				<div className='app-bloems-bdy'>
					<div className={'app-bloems-bdy-row'}>
						<TinyCardCounter
							counter={1000000}
							counterPrefix={"+"}
							className={'app-bloems-counter-card'}
							title={window.application.t('flowers')}
							description={window.application.t('flowers_description')}
							icon={"bi-flower2"}
						/>
						<TinyCardCounter
							counter={100}
							counterPrefix={"+"}
							className={'app-bloems-counter-card'}
							title={window.application.t('growers')}
							description={window.application.t('growers_description')}
							icon={"bi-person-arms-up"}
						/>
						<TinyCardCounter
							counter={100}
							counterPrefix={"+"}
							className={'app-bloems-counter-card'}
							title={window.application.t('transactions')}
							description={window.application.t('transactions_description')}
							icon={"bi-cart-check-fill"}
						/>
					</div>

					<div className={'app-bloems-bdy-row app-bloems-cardswap-ctn'}>
						<CardSwap
							width={'16vw'}
							height={'18vw'}
							cardDistance={60}
							verticalDistance={70}
							delay={5000}
							pauseOnHover={false}
						>
							<Card customClass={'app-bloems-card'}>
								<h3 className={'app-bloems-card-title'}>Euphorbia</h3>
								<span className={'app-bloems-card-txt'}>Felrode schutbladeren (bracteeën) en groene bladeren.</span>
								<img className={'app-bloems-card-img'} src={'/pictures/plant 1.png'} alt={'plant1'}/>
							</Card>
							<Card customClass={'app-bloems-card'}>
								<h3 className={'app-bloems-card-title'}>Gerbera</h3>
								<span className={'app-bloems-card-txt'}>Grote, vrolijke gele bloemen met een donker hart, lijkend op een margriet.</span>
								<img className={'app-bloems-card-img'} src={'/pictures/plant 2.png'} alt={'plant1'}/>
							</Card>
							<Card customClass={'app-bloems-card'}>
								<h3 className={'app-bloems-card-title'}>Petunia</h3>
								<span className={'app-bloems-card-txt'}>Overvloedige, kleinere, trompetvormige bloemen in felroze/magenta.</span>
								<img className={'app-bloems-card-img'} src={'/pictures/plant 3.png'} alt={'plant1'}/>
							</Card>
						</CardSwap>
					</div>
				</div>
			</div>
		</div>
	);
}

export default AppBloemSoort;