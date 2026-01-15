import React, {useCallback, useEffect, useRef, useState} from 'react';
import {useParams} from 'react-router-dom';
import {useRootContext} from '../../components/contexts/RootContext';
import AuctionClock, {AuctionClockRef} from '../../components/elements/AuctionClock';
import {useComponentStateReducer} from '../../hooks/useComponentStateReducer';
import {VeilingKlokOutputDto} from '../../declarations/dtos/output/VeilingKlokOutputDto';
import {ProductOutputDto} from '../../declarations/dtos/output/ProductOutputDto';
import {VeilingKlokStatus} from '../../declarations/enums/VeilingKlokStatus';
import {VeilingBodNotification, VeilingProductChangedNotification} from '../../declarations/models/VeilingNotifications';
import {useVeilingKlokSignalR} from '../../hooks/useVeilingKlokSignalR';
import {delay, formatDate, formatEur, getNormalizedVeilingKlokStatus} from '../../utils/standards';
import {getVeilingKlok} from '../../controllers/server/koper';
import {isHttpError} from '../../declarations/types/HttpError';
import Page from '../../components/nav/Page';
import Button from '../../components/buttons/Button';
import clsx from 'clsx';
import {KlokStatusBadge} from '../../components/elements/StatusBadge';
import ClockProductCard from '../../components/cards/ClockProductCard';
import ComponentState, {ComponentStateCard} from '../../components/elements/ComponentState';
import Modal from '../../components/elements/Modal';

function KoperVeilingKlok() {
	const {klokId: id} = useParams<{ klokId: string }>();
	const {t, account, languageCode, navigate} = useRootContext();

	const klokRef = useRef<AuctionClockRef | null>(null);

	// Component state
	const [state, updateState] = useComponentStateReducer();
	const [actionState, updateActionState] = useComponentStateReducer();
	const [clockWaitingProduct, setClockWaitingProduct] = useState<boolean>(false);
	const [currentVeilingKlok, setCurrentVeilingKlok] = useState<VeilingKlokOutputDto>();
	const [currentProduct, setCurrentProduct] = useState<ProductOutputDto>();
	const [quantity, setQuantity] = useState(0); // always set to max when product changed

	// SignalR event handlers wrapped in useCallback to prevent reconnection loops
	const handleVeilingEnded = useCallback(() => {
		setCurrentVeilingKlok((prev) => (prev ? {...prev, status: VeilingKlokStatus.Ended} : prev));
	}, []);
	const handleVeilingStarted = useCallback(() => {
		setCurrentVeilingKlok((prev) => (prev ? {...prev, status: VeilingKlokStatus.Started} : prev));
	}, []);
	const handleProductChanged = useCallback((state: VeilingProductChangedNotification) => {
		klokRef.current?.reset();
		setClockWaitingProduct(false);
		setCurrentVeilingKlok((prev) => {
			if (!prev) return prev;
			const productIndex = prev.products.findIndex((p) => p.id === state.productId);
			if (productIndex !== -1) {
				const newProduct = prev.products[productIndex];
				setCurrentProduct(newProduct);
				return {...prev, currentProductIndex: productIndex};
			}
			return prev;
		});
	}, []);
	const handleBidPlaced = useCallback((state: VeilingBodNotification) => {
		// Update product reamaining stock and current price
		setCurrentProduct((prev) => {
			if (!prev) return prev;
			return {
				...prev,
				stock: state.remainingQuantity,
			};
		});

		// Update klok product stock as well
		setCurrentVeilingKlok((prev) => {
			if (!prev) return prev;
			const updatedProducts = prev.products.map((p) => {
				if (p.id === state.productId) {
					return {
						...p,
						stock: state.remainingQuantity,
					};
				}
				return p;
			});

			return {...prev, products: updatedProducts};
		});
	}, []);
	const handleWaitingForProduct = useCallback(() => {
		setClockWaitingProduct(true);
	}, []);
	const handleTick = useCallback(() => {
		setClockWaitingProduct(false);
	}, []);
	const handleLiveViewsUpdate = useCallback((liveViews: number) => {
		setCurrentVeilingKlok((prev) => {
			if (!prev) return prev;
			return {...prev, peakedLiveViews: liveViews};
		});
	}, []);

	// Veiling klok SignalR hook
	const klokSignalR = useVeilingKlokSignalR({
		region: account?.region!,
		clockRef: klokRef,
		onVeilingEnded: handleVeilingEnded,
		onVeilingStarted: handleVeilingStarted,
		onProductChanged: handleProductChanged,
		onBidPlaced: handleBidPlaced,
		onProductWaitingForNext: handleWaitingForProduct,
		onPriceTick: handleTick,
		onViewerCountChanged: handleLiveViewsUpdate,
	});

	useEffect(() => {
		initializeVeilingKlok().then(null);

		return () => {
			// Leave klok SignalR group
			if (id) klokSignalR.leaveClock(id).then(null).catch(null);
		};
	}, [id]);

	const initializeVeilingKlok = useCallback(async () => {
		try {
			updateState({type: 'loading', message: t('veilingklok_loading')});
			await delay(1500);
			const response = await getVeilingKlok(id as string);
			setCurrentVeilingKlok(response.data);
			setCurrentProduct(response.data.products[(response.data.currentProductIndex || 0) % (response.data.products.length ?? 1)]);
			// Update quantity to max available
			setQuantity(response.data.products[(response.data.currentProductIndex || 0) % (response.data.products.length ?? 1)]?.stock || 0);

			updateState({type: 'succeed', message: t('veilingklok_loaded')});
			// Join klok SignalR group
			if (id) await klokSignalR.joinClock(id);
		} catch (e) {
			if (isHttpError(e) && e.message) updateState({type: 'error', message: e.message});
			else updateState({type: 'error', message: t('veilingklok_load_error')});
		} finally {
			await delay(100);
			updateState({type: 'idle'});
		}
	}, [id]);
	const placeBid = useCallback(async () => {
		if (clockWaitingProduct || !currentProduct) return;
		try {

		} catch (e) {


		}
	}, [clockWaitingProduct, currentProduct]);
	const increaseQuantity = useCallback(() => {
		setQuantity((prev) => prev + 1);
	}, []);
	const decreaseQuantity = useCallback(() => {
		setQuantity((prev) => (prev > 0 ? prev - 1 : 0));
	}, []);
	const onClose = () => navigate('/koper/veilingen');

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

									<div className={'!ml-auto flex flex-row gap-4'}>
										<div className={'vm-veiling-info-status'}>
											<span className={clsx(`app-table-status-badge text-[0.875rem] bg-blue-300`)}>
											<i className="app-table-status-icon bi-geo-alt-fill"/>
												{currentVeilingKlok.regionOrState}
											</span>
										</div>
										<div className={'vm-veiling-info-status'}>
											<span className={clsx(`app-table-status-badge text-[0.875rem]`, 'app-table-status-' + klokSignalR.klokConnectionStatus.toLowerCase())}>
											<i className="app-table-status-icon bi-wifi"/>
												{t(klokSignalR.klokConnectionStatus as any)}
										</span>
										</div>
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
											<div className={'flex flex-row gap-4 flex-nowrap'}>
												<div className={'vm-veiling-info-detail-item action'}>
													<span className={'vm-veiling-info-detail-label'}>{t('live_views')}:</span>
													<div className={'vm-veiling-info-status'}>
														<span className={`app-table-status-badge bg-blue-100 text-blue-800`}>
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
											</div>

											<div className={'vm-veiling-buy-actions'}>
												<div className={'vm-veiling-buy-actions-card'}>
													<div className={'vm-veiling-buy-actions-header'}>
														<h2 className={'vm-veiling-buy-actions-header-h1'}>
															<i className="bi bi-info-circle-fill"/>
															{t('select_quantity')}
														</h2>
														<span className={'vm-veiling-buy-actions-header-h2'}>
															{t('total_price')}
															<p className={'vm-veiling-buy-price'}>{formatEur((currentProduct?.auctionedPrice || 0) * quantity)}</p>
														</span>
													</div>

													<div className={'vm-veiling-buy-actions-selector-ctn'}>
														<label htmlFor="quantity" className="vm-veiling-buy-actions-selector-label">
															{t('quantity')}
														</label>
														<div className={'vm-veiling-buy-actions-selector'}>
															<Button className={'vm-veiling-buy-actions-btn'} icon={'bi-dash'} onClick={decreaseQuantity}/>
															<input id="quantity" type="number" className="vm-veiling-buy-actions-selector-input input-clean-number"
															       defaultValue={quantity} min={1} step={1} value={quantity}
															       onChange={(e) => setQuantity(parseInt(e.target.value, 10))}/>
															<Button className={'vm-veiling-buy-actions-btn'} icon={'bi-plus'} onClick={increaseQuantity}/>
														</div>
													</div>
												</div>
												<Button className={'vm-veiling-buy-action-btn'} label={t('place_bid')} onClick={placeBid}/>
											</div>
										</div>
									</div>
									<div className={'vm-veiling-info-klok-ctn smaller'}>
										<AuctionClock
											ref={klokRef}
											highestRange={currentVeilingKlok.highestProductPrice}
											// Product prices
											startPrice={currentProduct?.auctionedPrice ?? 0}
											lowestPrice={0}
											currentPrice={currentProduct?.auctionedPrice ?? 0}
											amountStock={currentProduct?.stock ?? 0}
											round={currentVeilingKlok.veilingRounds ?? 0}
											hideLowestPrice
										/>
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

											{/*{currentProduct.minimumPrice && (*/}
											{/*	<div className={'vm-veiling-klok-product-field small-box'}>*/}
											{/*		<div className={'vm-veiling-klok-product-field-label'}>{t('minimum_price')}:</div>*/}
											{/*		<div className={'vm-veiling-klok-product-field-box'}>*/}
											{/*			<span className="vm-veiling-klok-product-field-value">{formatEur(currentProduct.minimumPrice)}</span>*/}
											{/*		</div>*/}
											{/*	</div>*/}
											{/*)}*/}
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
											<ClockProductCard key={index} product={product} isSelected={currentVeilingKlok.currentProductIndex === index}
											                  status={getNormalizedVeilingKlokStatus(currentVeilingKlok.status)!} clockRunning={!clockWaitingProduct}/>
										))}
									</div>
								</div>
							</div>
						</section>
					</>
				)}

				{(state.type !== 'idle' || currentVeilingKlok) && <ComponentState state={state}/>}

				<Modal enabled={actionState.type !== 'idle'} onClose={() => actionState.type !== 'loading' && updateActionState({type: 'idle'})}>
					<ComponentStateCard state={actionState}/>
				</Modal>
			</main>
		</Page>
	);
}

export default KoperVeilingKlok;
