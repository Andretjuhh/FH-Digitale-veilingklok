import React from 'react';
import {useRootContext} from '../../components/contexts/RootContext';
import Page from '../../components/nav/Page';
import Button from '../../components/buttons/Button';

function ErrorPage() {
	const {t, navigate} = useRootContext();

	const handleGoBack = () => {
		// if path is the same as home , cause a page reload

		if (window.location.pathname === '/') {
			window.location.reload();
			return;
		}

		navigate('/', {replace: true});
	};

	return (
		<Page>
			<main className={'page-not-found'}>
				<div className={'page-not-found-gallery'}>
					<div className={'contact-form-gallery-gradient'}/>
				</div>

				<div className={'page-not-found-info'}>
					<img className={'not-found-img mb-4'} src={'/svg/error-bug-page.svg'} alt={t('alt_error_bug_picture')}/>
					<h1 className={'page-not-found-h1'}>{t('something_went_wrong')}</h1>
					<p className={'page-not-found-txt'}>{t('unexpected_happened')}</p>
					<Button className={'page-not-found-btn'} icon={'bi-arrow-left-short'} label={t('go_back_home')} onClick={handleGoBack}/>
				</div>
			</main>
		</Page>
	);
}

export default ErrorPage;
