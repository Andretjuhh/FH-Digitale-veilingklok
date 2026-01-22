import React, { useCallback, useEffect, useMemo, useState } from 'react';
import Page from '../../components/nav/Page';
import { useRootContext } from '../../components/contexts/RootContext';
import { useComponentStateReducer } from '../../hooks/useComponentStateReducer';
import { PaginatedOutputDto } from '../../declarations/dtos/output/PaginatedOutputDto';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import { Column, DataTable, OnFetchHandlerParams } from '../../components/layout/Table';
import { getProducts, getVeilingKlok, getVeilingKlokOrders } from '../../controllers/server/veilingmeester';
import GridTable from '../../components/layout/GridTable';
import Button from '../../components/buttons/Button';
import LabeledInfoCard from '../../components/cards/LabeledInfoCard';
import { KlokStatusBadge, StatusBadge } from '../../components/elements/StatusBadge';
import AuctionProductCard from '../../components/cards/AuctionProductCard';
import AddProductCard from '../../components/cards/AddProductCard';
import { VeilingKlokOutputDto } from '../../declarations/dtos/output/VeilingKlokOutputDto';
import { useParams } from 'react-router-dom';
import { isHttpError } from '../../declarations/types/HttpError';
import { delay, formatDate, formatEur } from '../../utils/standards';
import ComponentState from '../../components/elements/ComponentState';
import { OrderOutputDto } from '../../declarations/dtos/output/OrderOutputDto';

function VeilingmeesterKlokDetails() {
	const { t, account, languageCode, navigate } = useRootContext();
	const { klokId: id } = useParams<{ klokId: string }>();

	const [state, updateState] = useComponentStateReducer();
	const [currentVeilingKlok, setCurrentVeilingKlok] = useState<VeilingKlokOutputDto>();
	const [currentVeilingOrders, setCurrentVeilingOrders] = useState<OrderOutputDto[]>([]);
	const [paginatedProductsState, setPaginatedProductsState] = useComponentStateReducer();
	const [paginatedProducts, setPaginatedProducts] = useState<PaginatedOutputDto<ProductOutputDto>>();

	useEffect(() => {
		initializeVeilingKlok().then(null);
		initializedOrders().then(null);
	}, [id]);

	const initializeVeilingKlok = useCallback(async () => {
		try {
			updateState({ type: 'loading', message: t('veilingklok_loading') });
			await delay(1500);
			const response = await getVeilingKlok(id as string);
			setCurrentVeilingKlok(response.data);
			updateState({ type: 'succeed', message: t('veilingklok_loaded') });
		} catch (e) {
			if (isHttpError(e) && e.message) updateState({ type: 'error', message: e.message });
			else updateState({ type: 'error', message: t('veilingklok_load_error') });
		} finally {
			await delay(100);
			updateState({ type: 'idle' });
		}
	}, [id]);
	const initializedOrders = useCallback(async () => {
		try {
			const response = await getVeilingKlokOrders(id as string);
			setCurrentVeilingOrders(response.data);
		} catch (error) {
			setCurrentVeilingOrders([]);
		}
	}, [id]);
	const handleFetchProducts = useCallback(
		async (params: OnFetchHandlerParams) => {
			try {
				if (state.type != 'idle') return;
				setPaginatedProductsState({ type: 'loading' });
				const response = await getProducts(
					params.searchTerm,
					account?.region,
					undefined,
					undefined,
					undefined,
					params.page,
					params.pageSize,
				);
				if (response.data) setPaginatedProducts(response.data);
				setPaginatedProductsState({ type: 'succeed' });
			} catch (err) {
				console.error('Failed to fetch orders', err);
			}
		},
		[id, state],
	);
	const onClose = () => navigate('/veilingmeester/veilingen');

	const orderColumn: Column<OrderOutputDto>[] = useMemo(
		() => [
			{
				key: 'id',
				label: t('order_id'),
				sortable: true,
				render: (item) => <span className="font-medium">{item.id}</span>,
			},
			{
				key: 'status',
				label: t('order_status'),
				sortable: true,
				render: (item) => <StatusBadge status={item.status} />,
			},
			{
				key: 'totalPrice',
				label: t('total_value'),
				sortable: true,
				render: (item) => {
					return <span className="font-semibold">{formatEur(item.totalAmount)}</span>;
				},
			},
			{
				key: 'createdAt',
				label: t('order_datum'),
				sortable: true,
				render: (item) => <span>{formatDate(item.createdAt, languageCode, 0)}</span>,
			},
		],
		[t],
	);

	return (
		<Page enableHeader className="vm-veiling-info-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-veiling-info-page-ctn">
				{state.type === 'idle' && currentVeilingKlok && (
					<>
						<section className={'vm-veiling-info-left-panel'} aria-label={t('aria_meester_auction_details')}>
							<div className={'vm-veiling-info-data'}>
								<div className={'vm-veiling-info-header'}>
									<Button
										className="modal-card-back-btn vm-veiling-info-btn"
										icon="bi-x"
										type="button"
										aria-label={t('aria_back_button')}
										onClick={onClose}
									/>
									<h2 id="meester-veiling-details-title" className={'vm-veiling-info-h1'}>
										<i className="bi bi-layers-fill"></i>
										{t('auction_clock_details')}
									</h2>
								</div>

								<div className={'vm-veiling-info-details'}>
									<div className={'vm-veiling-info-detail-item'}>
										<span className={'vm-veiling-info-detail-label'}>{t('status')}:</span>
										<div className={'vm-veiling-info-status'}>
											<KlokStatusBadge status={currentVeilingKlok.status} />
										</div>
									</div>
									<div className={'vm-veiling-info-detail-item'}>
										<span className={'vm-veiling-info-detail-label'}>Id:</span>
										<span className={'vm-veiling-info-detail-value'}>{currentVeilingKlok.id}</span>
									</div>
									<div className={'vm-veiling-info-detail-item'}>
										<span className={'vm-veiling-info-detail-label'}>{t('created')}:</span>
										<span className={'vm-veiling-info-detail-value'}>{formatDate(currentVeilingKlok.createdAt, languageCode, 3)}</span>
									</div>
								</div>

								<div className={'vm-veiling-info-data-grid'}>
									<LabeledInfoCard
										color={'!bg-blue-500 border-blue-100 !shadow-blue-500/10'}
										title={t('scheduledDate')}
										value={formatDate(currentVeilingKlok.scheduledAt, languageCode, 0)}
										icon={<i className="bi bi-calendar-event-fill"></i>}
									/>
									<LabeledInfoCard
										color={'!bg-green-500 border-green-100 !shadow-green-500/10'}
										title={t('total_products')}
										value={currentVeilingKlok.products.length}
										icon={<i className="bi bi-bag-fill"></i>}
									/>

									<LabeledInfoCard
										color={'!bg-purple-500 border-purple-100 !shadow-purple-500/10'}
										title={t('totalBids')}
										value={`€ ${currentVeilingOrders?.reduce((acc, order) => acc + order.totalAmount, 0).toFixed(2)}`}
										icon={<i className="bi bi-currency-euro"></i>}
									/>

									<LabeledInfoCard
										color={'!bg-yellow-500 border-yellow-100 !shadow-yellow-500/10'}
										title={t('duration_per_bids')}
										value={`${currentVeilingKlok.veilingDurationSeconds} ${t('seconds')}`}
										icon={<i className="bi bi-hourglass-split"></i>}
									/>
								</div>
							</div>

							<>
								{currentVeilingKlok.status == ('Scheduled' as any) ? (
									<GridTable
										isLazy
										itemsPerPage={12}
										data={paginatedProducts?.data || []}
										loading={paginatedProductsState.type == 'loading'}
										totalItems={paginatedProducts?.totalCount || 0}
										onFetchData={handleFetchProducts}
										title={t('region_flowers')}
										icon={<i className="bi bi-bag-fill"></i>}
										renderItem={(item, index) => (
											<AddProductCard
												added={currentVeilingKlok.products?.some((e) => e.id == item.id)}
												mode={'meester'}
												product={item}
												index={index}
												veilingId={id}
												onAction={(type, product, index) => {
													if (type == 'add') {
														setCurrentVeilingKlok((prev) =>
															prev
																? {
																		...prev,
																		products: [...(prev.products || []), product],
																	}
																: prev,
														);
													} else if (type == 'remove') {
														setCurrentVeilingKlok((prev) =>
															prev
																? {
																		...prev,
																		products: prev.products.filter((p) => p.id != product.id),
																	}
																: prev,
														);
													}
												}}
											/>
										)}
										emptyText={t('no_products')}
										aria-label={t('aria_meester_region_products')}
									/>
								) : (
									<DataTable<OrderOutputDto>
										data={currentVeilingOrders}
										getItemKey={(item) => item.id}
										title={t('recent_orders')}
										icon={<i className="bi bi-cart4"></i>}
										columns={orderColumn}
										emptyText={t('no_orders')}
										aria-label={t('aria_meester_recent_orders')}
									/>
								)}
							</>
						</section>

						<section className={'vm-veiling-info-right-panel'} aria-label={t('aria_meester_clock_products')}>
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
											<AuctionProductCard key={index} product={product} />
										))}
									</div>
								</div>
							</div>
						</section>
					</>
				)}

				{(state.type !== 'idle' || currentVeilingKlok) && <ComponentState state={state} />}
			</main>
		</Page>
	);
}

export default VeilingmeesterKlokDetails;
