import React, {useCallback, useEffect, useRef, useState} from 'react';
import Page from '../../components/nav/Page';
import {useRootContext} from '../../components/contexts/RootContext';
import {useParams} from 'react-router-dom';
import {useComponentStateReducer} from '../../hooks/useComponentStateReducer';
import {VeilingKlokOutputDto} from '../../declarations/dtos/output/VeilingKlokOutputDto';
import {delay, formatDate, formatEur, getNormalizedVeilingKlokStatus} from '../../utils/standards';
import {getVeilingKlok} from '../../controllers/server/veilingmeester';
import {isHttpError} from '../../declarations/types/HttpError';
import ComponentState from '../../components/elements/ComponentState';
import Button from '../../components/buttons/Button';
import {KlokStatusBadge} from '../../components/elements/StatusBadge';
import AuctionClock, {AuctionClockRef} from '../../components/elements/AuctionClock';
import {ProductOutputDto} from '../../declarations/dtos/output/ProductOutputDto';
import {VeilingKlokStatus} from '../../declarations/enums/VeilingKlokStatus';
import ClockProductCard from "../../components/cards/ClockProductCard";
import {HubConnectionState} from "@microsoft/signalr";
import clsx from "clsx";
import {useVeilingKlokSignalR} from "../../hooks/useVeilingKlokSignalR";

function VeilingmeesterKlokManage() {
	const {klokId: id} = useParams<{ klokId: string }>();
	const {t, account, languageCode, navigate} = useRootContext();

	const klokRef = useRef<AuctionClockRef | null>(null);

	// Component state
	const klokSignalR = useVeilingKlokSignalR({regionGroupName: account?.region!, clockRef: klokRef});
	const [state, updateState] = useComponentStateReducer();
	const [actionState, updateActionState] = useComponentStateReducer();
	const [currentVeilingKlok, setCurrentVeilingKlok] = useState<VeilingKlokOutputDto>();
	const [currentProduct, setCurrentProduct] = useState<ProductOutputDto>({
		id: '1415b9d3-9204-4ded-8eba-7e2213515247',
		name: "Tulipa 'Red Emperor'",
		description: 'Premium selection of vibrant red tulips, ideal for large-scale landscaping and spring auctions.',
		imageUrl: 'https://images.example.com/flowers/tulips-red-emperor.jpg',
		auctionedPrice: 1250.5,
		minimumPrice: 900.0,
		auctionedAt: '2026-01-14T12:00:00Z',
		region: 'Bollenstreek',
		dimension: 'Height: 45cm, Bulb size: 12/+',
		stock: 5000,
		companyName: 'Bloemenexport BV',
		auctionPlanned: true,
	});
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
			if (isHttpError(e) && e.message) updateState({type: 'error', message: e.message});
			else updateState({type: 'error', message: t('veilingklok_load_error')});
		} finally {
			await delay(100);
			updateState({type: 'idle'});
		}
	}, [id]);

	const startVeilingKlok = useCallback(async () => {
	}, [id]);
	const stopVeilingKlok = useCallback(async () => {
	}, [id]);
	const pauseVeilingKlok = useCallback(async () => {
	}, [id]);
	const resumeVeilingKlok = useCallback(async () => {
	}, [id]);
	const startProductVeiling = useCallback(async (productId: string) => {
	}, [id]);

	return (
		<Page enableHeader className="vm-veiling-info-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-veiling-info-page-ctn">
				{state.type === 'idle' && currentVeilingKlok && (
					<>
						<section className={'vm-veiling-info-left-panel'}>
							<div className={'vm-veiling-info-data'}>
								<div className={'vm-veiling-info-header'}>
									<Button className="modal-card-back-btn vm-veiling-info-btn" icon="bi-x" type="button" aria-label={t('aria_back_button')} onClick={onClose}/>
									<h2 className={'vm-veiling-info-h1'}>
										<i className="bi bi-stopwatch-fill"></i>
										{t('manage_veiling_klok')}
									</h2>

									<div className={'vm-veiling-info-status !ml-auto'}>
											<span
												className={clsx(
													`app-table-status-badge text-[0.875rem]`,
													"app-table-status-" + HubConnectionState.Connected.toLowerCase()
												)}
											>
												<i className="app-table-status-icon bi-wifi"/>
												{t('Connected')}
											</span>
									</div>
								</div>

								<div className={'vm-veiling-info-klok-details'}>
									<div className={'vm-veiling-info-details'}>
										<div className={'vm-veiling-info-detail-item'}>
											<span className={'vm-veiling-info-detail-label'}>Id:</span>
											<span className={'vm-veiling-info-detail-value'}>{currentVeilingKlok.id}</span>
										</div>
										<div className={'vm-veiling-info-detail-item'}>
											<span className={'vm-veiling-info-detail-label'}>{t('created')}:</span>
											<span className={'vm-veiling-info-detail-value'}>{formatDate(currentVeilingKlok.createdAt, languageCode, 3)}</span>
										</div>
										<div className={'vm-veiling-info-klok-actions'}>
											<div className={'vm-veiling-info-detail-item action'}>
												<span className={'vm-veiling-info-detail-label'}>{t('live_views')}:</span>
												<div className={'vm-veiling-info-status'}>
													<span className={`app-table-status-badge bg-blue-100 text-blue-800 text-[1.2rem]`}>
														<i className="app-table-status-icon bi-eye-fill text-blue-800"/>
														{currentVeilingKlok.peakedLiveViews}
													</span>
												</div>
											</div>
											<div className={'vm-veiling-info-detail-item action'}>
												<span className={'vm-veiling-info-detail-label'}>{t('status')}:</span>
												<div className={'vm-veiling-info-status'}>
													<KlokStatusBadge status={currentVeilingKlok.status}/>
												</div>
											</div>

											{currentVeilingKlok.status === ('Scheduled' as any) &&
												<Button
													className={'vm-veiling-info-klok-action-primary-btn start'}
													label={t('start_veiling_klok')}
													icon="bi-play-fill"
													onClick={startVeilingKlok}
												/>
											}

											{currentVeilingKlok.status === ('Started' as any) &&
												<Button
													className={'vm-veiling-info-klok-action-primary-btn pause'}
													label={t('pause_veiling_klok')}
													icon="bi-pause-fill"
													onClick={pauseVeilingKlok}
												/>
											}

											{
												currentVeilingKlok.status === ('Paused' as any) &&
												<Button
													className={'vm-veiling-info-klok-action-primary-btn resume'}
													label={t('resume_veiling_klok')}
													icon="bi-play-fill"
													onClick={resumeVeilingKlok}
												/>
											}

											{getNormalizedVeilingKlokStatus(currentVeilingKlok.status)! !== VeilingKlokStatus.Scheduled && getNormalizedVeilingKlokStatus(currentVeilingKlok.status)! < VeilingKlokStatus.Ended && (
												<>
													<Button
														className={'vm-veiling-info-klok-action-secondary-btn end'}
														label={t('stop_veiling_klok')}
														icon="bi-stop-fill"
														onClick={stopVeilingKlok}
													/>
												</>
											)}
										</div>
									</div>

									<div className={'vm-veiling-info-klok-ctn'}>
										<AuctionClock ref={klokRef}/>
									</div>
								</div>
							</div>

							{currentProduct && (
								<div className={'vm-veiling-klok-product'}>
									<div className={'vm-veiling-klok-product-img-parent'}>
										<img className={'vm-veiling-klok-product-img'} src="/pictures/flower-test.avif" alt={currentProduct.name}/>
									</div>

									<div className={'vm-veiling-klok-product-info'}>
										<div className={'vm-veiling-klok-product-field small-box'}>
											<div className={'vm-veiling-klok-product-field-label'}>{t('product_id')}:</div>
											<div className={'vm-veiling-klok-product-field-box'}>
												<span className="vm-veiling-klok-product-field-value">{currentProduct.id}</span>
											</div>
										</div>

										<div className={'vm-veiling-klok-product-info-row'}>
											<div className={'vm-veiling-klok-product-field'}>
												<div className={'vm-veiling-klok-product-field-label'}>{t('aanvoerder')}:</div>
												<div className={'vm-veiling-klok-product-field-box'}>
													<span className="vm-veiling-klok-product-field-value">{currentProduct.companyName}</span>
												</div>
											</div>

											<div className={'vm-veiling-klok-product-field'}>
												<div className={'vm-veiling-klok-product-field-label'}>{t('product_naam')}:</div>
												<div className={'vm-veiling-klok-product-field-box'}>
													<span className="vm-veiling-klok-product-field-value">{currentProduct.name}</span>
												</div>
											</div>
										</div>

										<div className={'vm-veiling-klok-product-info-row'}>
											<div className={'vm-veiling-klok-product-field'}>
												<div className={'vm-veiling-klok-product-field-label'}>{t('start_price')}:</div>
												<div className={'vm-veiling-klok-product-field-box'}>
													<span className="vm-veiling-klok-product-field-value text-primary-600">{formatEur(currentProduct.auctionedPrice ?? 0)}</span>
												</div>
											</div>

											{currentProduct.minimumPrice && (
												<div className={'vm-veiling-klok-product-field small-box'}>
													<div className={'vm-veiling-klok-product-field-label'}>{t('minimum_price')}:</div>
													<div className={'vm-veiling-klok-product-field-box'}>
														<span className="vm-veiling-klok-product-field-value">{formatEur(currentProduct.minimumPrice)}</span>
													</div>
												</div>
											)}
										</div>

										<div className={'vm-veiling-klok-product-field small-box'}>
											<div className={'vm-veiling-klok-product-field-label'}>{t('region')}:</div>
											<div className={'vm-veiling-klok-product-field-box'}>
												<span className="vm-veiling-klok-product-field-value">{currentProduct.region}</span>
											</div>
										</div>

										<div className={'vm-veiling-klok-product-field'}>
											<div className={'vm-veiling-klok-product-field-label'}>{t('dimension')}:</div>
											<div className={'vm-veiling-klok-product-field-box'}>
												<span className="vm-veiling-klok-product-field-value">{currentProduct.dimension}</span>
											</div>
										</div>
									</div>
								</div>
							)}
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
										{currentVeilingKlok?.products.map((product, index) => (
											<ClockProductCard key={index} product={product}/>
										))}
									</div>
								</div>
							</div>
						</section>
					</>
				)}

				{(state.type !== 'idle' || currentVeilingKlok) && <ComponentState state={state}/>}
			</main>
		</Page>
	);
}

export default VeilingmeesterKlokManage;
