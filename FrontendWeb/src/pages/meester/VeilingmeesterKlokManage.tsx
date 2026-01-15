import React, { useCallback, useEffect, useRef, useState } from 'react';
import Page from '../../components/nav/Page';
import { useRootContext } from '../../components/contexts/RootContext';
import { useParams } from 'react-router-dom';
import { useComponentStateReducer } from '../../hooks/useComponentStateReducer';
import { VeilingKlokOutputDto } from '../../declarations/dtos/output/VeilingKlokOutputDto';
import { delay, formatDate, formatEur, getNormalizedVeilingKlokStatus } from '../../utils/standards';
import { getVeilingKlok, startVeilingProduct, updateVeilingKlokStatus } from '../../controllers/server/veilingmeester';
import { isHttpError } from '../../declarations/types/HttpError';
import ComponentState, { ComponentStateCard } from '../../components/elements/ComponentState';
import Button from '../../components/buttons/Button';
import { KlokStatusBadge } from '../../components/elements/StatusBadge';
import AuctionClock, { AuctionClockRef } from '../../components/elements/AuctionClock';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { VeilingKlokStatus } from '../../declarations/enums/VeilingKlokStatus';
import ClockProductCard from '../../components/cards/ClockProductCard';
import clsx from 'clsx';
import { useVeilingKlokSignalR } from '../../hooks/useVeilingKlokSignalR';
import Modal from '../../components/elements/Modal';
import { VeilingPriceTickNotification, VeilingProductChangedNotification } from '../../declarations/models/VeilingNotifications';

function VeilingmeesterKlokManage() {
	const { klokId: id } = useParams<{ klokId: string }>();
	const { t, account, languageCode, navigate } = useRootContext();

	const klokRef = useRef<AuctionClockRef | null>(null);

	// Component state
	const [state, updateState] = useComponentStateReducer();
	const [actionState, updateActionState] = useComponentStateReducer();
	const [currentVeilingKlok, setCurrentVeilingKlok] = useState<VeilingKlokOutputDto>();
	const [currentProduct, setCurrentProduct] = useState<ProductOutputDto>();

	// SignalR event handlers wrapped in useCallback to prevent reconnection loops
	const handlePriceTick = useCallback((state: VeilingPriceTickNotification) => {
		// klokRef.current?.tick(state);
		console.log('Price tick received:', state);
	}, []);
	const handleVeilingEnded = useCallback(() => {
		setCurrentVeilingKlok((prev) => (prev ? { ...prev, status: VeilingKlokStatus.Ended } : prev));
	}, []);
	const handleVeilingStarted = useCallback(() => {
		setCurrentVeilingKlok((prev) => (prev ? { ...prev, status: VeilingKlokStatus.Started } : prev));
	}, []);
	const handleProductChanged = useCallback((state: VeilingProductChangedNotification) => {
		klokRef.current?.reset();
		setCurrentVeilingKlok((prev) => {
			if (!prev) return prev;
			const productIndex = prev.products.findIndex((p) => p.id === state.productId);
			if (productIndex !== -1) {
				const newProduct = prev.products[productIndex];
				setCurrentProduct(newProduct);
				return { ...prev, currentProductIndex: productIndex };
			}
			return prev;
		});
	}, []);

	// Veiling klok SignalR hook
	const klokSignalR = useVeilingKlokSignalR({
		region: account?.region!,
		clockRef: klokRef,
		onPriceTick: handlePriceTick,
		onVeilingEnded: handleVeilingEnded,
		onVeilingStarted: handleVeilingStarted,
		onProductChanged: handleProductChanged,
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
			updateState({ type: 'loading', message: t('veilingklok_loading') });
			await delay(1500);
			const response = await getVeilingKlok(id as string);
			setCurrentVeilingKlok(response.data);
			setCurrentProduct(response.data.products[(response.data.currentProductIndex || 0) % (response.data.products.length ?? 1)]);
			updateState({ type: 'succeed', message: t('veilingklok_loaded') });

			// Join klok SignalR group
			if (id) await klokSignalR.joinClock(id);
		} catch (e) {
			if (isHttpError(e) && e.message) updateState({ type: 'error', message: e.message });
			else updateState({ type: 'error', message: t('veilingklok_load_error') });
		} finally {
			await delay(100);
			updateState({ type: 'idle' });
		}
	}, [id]);
	const updateKlokStatus = useCallback(
		async (status: VeilingKlokStatus) => {
			console.log('Updating klok status to', status, currentVeilingKlok);
			if (!currentVeilingKlok) return;
			try {
				updateActionState({ type: 'loading', message: t('updating_veilingklok') });
				await updateVeilingKlokStatus(currentVeilingKlok.id, status);
				await delay(1500);
				updateActionState({ type: 'succeed', message: t('veilingklok_updated') });

				// Update prev state with the new status
				setCurrentVeilingKlok({
					...currentVeilingKlok,
					status: status,
				});
			} catch (e: any) {
				if (isHttpError(e) && e.message) updateActionState({ type: 'error', message: e.message });
				else updateActionState({ type: 'error', message: t('veilingklok_update_error') });
			} finally {
				await delay(2000);
				updateActionState({ type: 'idle' });
			}
		},
		[id, currentVeilingKlok]
	);
	const startVeilingKlok = useCallback(async () => {
		await updateKlokStatus(VeilingKlokStatus.Started);
	}, [updateKlokStatus]);
	const stopVeilingKlok = useCallback(async () => {
		await updateKlokStatus(VeilingKlokStatus.Stopped);
	}, [updateKlokStatus]);
	const pauseVeilingKlok = useCallback(async () => {
		await updateKlokStatus(VeilingKlokStatus.Paused);
	}, [updateKlokStatus]);
	const resumeVeilingKlok = useCallback(async () => {
		await updateKlokStatus(VeilingKlokStatus.Started);
	}, [updateKlokStatus]);
	const startProductVeiling = useCallback(
		async (productId: string) => {
			if (!currentVeilingKlok) return;
			const startedProduct = currentVeilingKlok.products.find((p) => p.id === productId);
			if (!startedProduct) return;
			try {
				updateActionState({ type: 'loading', message: t('starting_product_veiling') });
				await startVeilingProduct(currentVeilingKlok.id, productId);
				await delay(1500);
				updateActionState({ type: 'succeed', message: t('product_veiling_started') });

				// Update current product state to reflect that the auction has started get it from the klok products
				if (startedProduct) {
					setCurrentProduct(startedProduct);
				}
			} catch (e: any) {
				if (isHttpError(e) && e.message) updateActionState({ type: 'error', message: e.message });
				else updateActionState({ type: 'error', message: t('product_veiling_start_error') });
			} finally {
				await delay(2000);
				updateActionState({ type: 'idle' });
			}
		},
		[updateKlokStatus]
	);
	const onClose = () => navigate('/veilingmeester/veilingen-beheren');

	return (
		<Page enableHeader className="vm-veiling-info-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-veiling-info-page-ctn">
				{state.type === 'idle' && currentVeilingKlok && (
					<>
						<section className={'vm-veiling-info-left-panel'}>
							<div className={'vm-veiling-info-data'}>
								<div className={'vm-veiling-info-header'}>
									<Button className="modal-card-back-btn vm-veiling-info-btn" icon="bi-x" type="button" aria-label={t('aria_back_button')} onClick={onClose} />
									<h2 className={'vm-veiling-info-h1'}>
										<i className="bi bi-stopwatch-fill"></i>
										{t('manage_veiling_klok')}
									</h2>

									<div className={'vm-veiling-info-status !ml-auto'}>
										<span className={clsx(`app-table-status-badge text-[0.875rem]`, 'app-table-status-' + klokSignalR.klokConnectionStatus.toLowerCase())}>
											<i className="app-table-status-icon bi-wifi" />
											{t(klokSignalR.klokConnectionStatus as any)}
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
														<i className="app-table-status-icon bi-eye-fill text-blue-800" />
														{currentVeilingKlok.peakedLiveViews}
													</span>
												</div>
											</div>
											<div className={'vm-veiling-info-detail-item action'}>
												<span className={'vm-veiling-info-detail-label'}>{t('status')}:</span>
												<div className={'vm-veiling-info-status'}>
													<KlokStatusBadge status={currentVeilingKlok.status} />
												</div>
											</div>

											{getNormalizedVeilingKlokStatus(currentVeilingKlok.status)! === VeilingKlokStatus.Scheduled && <Button className={'vm-veiling-info-klok-action-primary-btn start'} label={t('start_veiling_klok')} icon="bi-play-fill" onClick={startVeilingKlok} />}

											{getNormalizedVeilingKlokStatus(currentVeilingKlok.status)! === VeilingKlokStatus.Started && <Button className={'vm-veiling-info-klok-action-primary-btn pause'} label={t('pause_veiling_klok')} icon="bi-pause-fill" onClick={pauseVeilingKlok} />}

											{getNormalizedVeilingKlokStatus(currentVeilingKlok.status)! == VeilingKlokStatus.Paused && <Button className={'vm-veiling-info-klok-action-primary-btn resume'} label={t('resume_veiling_klok')} icon="bi-play-fill" onClick={resumeVeilingKlok} />}

											{getNormalizedVeilingKlokStatus(currentVeilingKlok.status)! !== VeilingKlokStatus.Scheduled && getNormalizedVeilingKlokStatus(currentVeilingKlok.status)! < VeilingKlokStatus.Ended && (
												<>
													<Button className={'vm-veiling-info-klok-action-secondary-btn end'} label={t('stop_veiling_klok')} icon="bi-stop-fill" onClick={stopVeilingKlok} />
												</>
											)}
										</div>
									</div>

									<div className={'vm-veiling-info-klok-ctn'}>
										<AuctionClock
											ref={klokRef}
											highestRange={currentVeilingKlok.highestProductPrice}
											// Product prices
											startPrice={currentProduct?.auctionedPrice ?? 0}
											lowestPrice={currentProduct?.minimumPrice ?? 0}
											currentPrice={currentProduct?.auctionedPrice ?? 0}
											amountStock={currentProduct?.stock ?? 0}
											round={currentVeilingKlok.veilingRounds ?? 0}
										/>
									</div>
								</div>
							</div>

							{currentProduct && (
								<div className={'vm-veiling-klok-product'}>
									<div className={'vm-veiling-klok-product-img-parent'}>
										<img className={'vm-veiling-klok-product-img'} src="/pictures/flower-test.avif" alt={currentProduct.name} />
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
								<i className={'vm-veiling-info-line'} />

								<div className={'vm-veiling-info-products-scroll custom-scroll'}>
									<div className={'vm-veiling-info-products-list'}>
										{currentVeilingKlok?.products.map((product, index) => (
											<ClockProductCard key={index} product={product} isSelected={currentVeilingKlok.currentProductIndex === index} status={getNormalizedVeilingKlokStatus(currentVeilingKlok.status)!} clockRunning={getNormalizedVeilingKlokStatus(currentVeilingKlok.status) === VeilingKlokStatus.Started} onStartAuctionClick={() => startProductVeiling(product.id)} />
										))}
									</div>
								</div>
							</div>
						</section>
					</>
				)}

				{(state.type !== 'idle' || currentVeilingKlok) && <ComponentState state={state} />}

				<Modal enabled={actionState.type !== 'idle'} onClose={() => actionState.type !== 'loading' && updateActionState({ type: 'idle' })}>
					<ComponentStateCard state={actionState} />
				</Modal>
			</main>
		</Page>
	);
}

export default VeilingmeesterKlokManage;
