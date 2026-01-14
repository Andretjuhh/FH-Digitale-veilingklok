import React, {useCallback, useEffect, useState} from 'react';
import Page from "../../components/nav/Page";
import {useRootContext} from "../../components/contexts/RootContext";
import {useParams} from "react-router-dom";
import {useComponentStateReducer} from "../../hooks/useComponentStateReducer";
import {VeilingKlokOutputDto} from "../../declarations/dtos/output/VeilingKlokOutputDto";
import {delay, formatDate} from "../../utils/standards";
import {getVeilingKlok} from "../../controllers/server/veilingmeester";
import {isHttpError} from "../../declarations/types/HttpError";
import ComponentState from "../../components/elements/ComponentState";
import AuctionProductCard from "../../components/cards/AuctionProductCard";
import Button from "../../components/buttons/Button";
import {KlokStatusBadge} from "../../components/elements/StatusBadge";
import AuctionClock from "../../components/elements/AuctionClock";

function VeilingmeesterKlokManage() {
	const {t, account, languageCode, navigate} = useRootContext();
	const {klokId: id} = useParams<{ klokId: string }>();

	const [state, updateState] = useComponentStateReducer();
	const [currentVeilingKlok, setCurrentVeilingKlok] = useState<VeilingKlokOutputDto>();
	const [paginatedProductsState, setPaginatedProductsState] = useComponentStateReducer();
	const onClose = () => navigate('/veilingmeester/veilingen-beheren');

	useEffect(() => {
		initializeVeilingKlok().then(null);
	}, [id]);

	const initializeVeilingKlok = useCallback(async () => {
		try {
			updateState({type: 'loading', message: t('veilingklok_loading')});
			await delay(1500);
			const response = await getVeilingKlok(id as string);
			setCurrentVeilingKlok(response.data);
			updateState({type: 'succeed', message: t('veilingklok_loaded')});
		} catch (e) {
			if (isHttpError(e) && e.message)
				updateState({type: 'error', message: e.message});
			else
				updateState({type: 'error', message: t('veilingklok_load_error')});
		} finally {
			await delay(100);
			updateState({type: 'idle'});
		}
	}, [id]);

	return (
		<Page enableHeader className="vm-veiling-info-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-veiling-info-page-ctn">
				{
					(state.type === 'idle' && currentVeilingKlok) && (
						<>
							<section className={'vm-veiling-info-left-panel'}>
								<div className={'vm-veiling-info-data'}>
									<div className={'vm-veiling-info-header'}>
										<Button className="modal-card-back-btn vm-veiling-info-btn" icon="bi-x" type="button" aria-label={t('aria_back_button')} onClick={onClose}/>
										<h2 className={'vm-veiling-info-h1'}>
											<i className="bi bi-stopwatch-fill"></i>
											{t('manage_veiling_klok')}
										</h2>
									</div>

									<div className={'vm-veiling-info-klok-details'}>
										<div className={'vm-veiling-info-details'}>
											<div className={'vm-veiling-info-detail-item'}>
											 <span className={'vm-veiling-info-detail-label'}>
												Id:
											 </span>
												<span className={'vm-veiling-info-detail-value'}>
												 {currentVeilingKlok.id}
											 </span>
											</div>
											<div className={'vm-veiling-info-detail-item'}>
											 <span className={'vm-veiling-info-detail-label'}>
												{t('created')}:
											 </span>
												<span className={'vm-veiling-info-detail-value'}>
												{formatDate(currentVeilingKlok.createdAt, languageCode, 3)}
											 </span>
											</div>
											<div className={'vm-veiling-info-detail-item'}>
											<span className={'vm-veiling-info-detail-label'}>
												{t('status')}:
											 </span>
												<div className={'vm-veiling-info-status'}>
													<KlokStatusBadge status={currentVeilingKlok.status}/>
												</div>
											</div>
										</div>
										<div className={'vm-veiling-info-klok-ctn'}>
											<AuctionClock/>
										</div>
									</div>
								</div>
							</section>

							<section className={'vm-veiling-info-right-panel'}>
								<div className={'vm-veiling-info-products'}>
									<div className={'vm-veiling-info-header'}>
										<h2 className={'vm-veiling-info-h2'}>
											<i className="bi bi-list-nested"></i>
											{t('auction_products')}
										</h2>
									</div>
									<i className={'vm-veiling-info-line'}/>

									<div className={'vm-veiling-info-products-scroll custom-scroll'}>
										<div className={'vm-veiling-info-products-list'}>
											{
												currentVeilingKlok?.products.map((product, index) => (
													<AuctionProductCard key={index} product={product}/>
												))
											}
										</div>
									</div>
								</div>
							</section>
						</>
					)
				}

				{
					(state.type !== 'idle' || currentVeilingKlok) && <ComponentState state={state}/>
				}
			</main>
		</Page>
	);
}

export default VeilingmeesterKlokManage;