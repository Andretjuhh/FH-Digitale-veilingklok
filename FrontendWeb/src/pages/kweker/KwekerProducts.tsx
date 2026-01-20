import React, { useCallback, useState } from 'react';
import Page from '../../components/nav/Page';
import { useRootContext } from '../../components/contexts/RootContext';
import { KwekerProductStats } from '../../components/sections/kweker/KwekerStats';
import GridTable from '../../components/layout/GridTable';
import ProductCard from '../../components/cards/ProductCard';
import { ProductOutputDto } from '../../declarations/dtos/output/ProductOutputDto';
import CreateEditProduct from '../../components/sections/kweker/CreateEditProduct';
import { PaginatedOutputDto } from '../../declarations/dtos/output/PaginatedOutputDto';
import { useComponentStateReducer } from '../../hooks/useComponentStateReducer';
import { OnFetchHandlerParams } from '../../components/layout/Table';
import { getProducts, deleteProduct } from '../../controllers/server/kweker';
import Button from '../../components/buttons/Button';
import Modal from '../../components/elements/Modal';
import ComponentState, { ComponentStateCard } from '../../components/elements/ComponentState';

function KwekerProducts() {
	const { t, account } = useRootContext();

	// State for Create/Edit Product Modal
	const [paginatedProductsState, setPaginatedProductsState] = useComponentStateReducer();
	const [productState, setProductState] = useComponentStateReducer();
	const [paginatedProducts, setPaginatedProducts] = useState<PaginatedOutputDto<ProductOutputDto>>();
	const [openCreateEditModal, setOpenCreateEditModal] = useState<{ visible: boolean; product?: ProductOutputDto }>({ visible: false });

	const handleFetchProducts = useCallback(
		async (params: OnFetchHandlerParams) => {
			try {
				setPaginatedProductsState({ type: 'loading' });
				const response = await getProducts(params.searchTerm, undefined, undefined, params.page, params.pageSize);
				if (response.data) setPaginatedProducts(response.data);
				setPaginatedProductsState({ type: 'succeed' });
			} catch (err) {
				console.error('Failed to fetch orders', err);
				setPaginatedProductsState({ type: 'error', message: t('error_fetching_products') });
			}
		},
		[t, setPaginatedProductsState],
	);

	const handleDeleteProduct = useCallback(
		async (product: ProductOutputDto) => {
			if (!window.confirm(t('confirm_delete_product_warning'))) return;

			try {
				setProductState({ type: 'loading' });
				const response = await deleteProduct(product.id);
				if (response.success) {
					setProductState({ type: 'succeed', message: response.message });
					setPaginatedProducts((prev) => {
						if (!prev) return prev;
						return {
							...prev,
							data: prev.data.filter((p) => p.id !== product.id),
							totalCount: prev.totalCount - 1,
						};
					});
				} else {
					setProductState({ type: 'error', message: response.message });
				}
			} catch (err) {
				setProductState({ type: 'error', message: t('error_deleting_product') });
				console.error('Failed to delete product', err);
			}
		},
		[t, handleFetchProducts, setProductState],
	);

	return (
		<Page enableHeader className="kweker-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="kweker-products-page-ctn">
				<section className="page-title-section" aria-labelledby="kweker-products-title kweker-products-subtitle">
					<h1 id="kweker-products-title">
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2 id="kweker-products-subtitle">{t('kweker_products_description')}</h2>
				</section>

				<section className={'products-page-stats'} aria-label={t('aria_kweker_products_stats')}>
					<KwekerProductStats />
					<div className="products-page-action-card" aria-label={t('aria_kweker_products_actions')}>
						<div className="products-page-action-card-title">
							<span>
								<i className={'bi bi-bag-plus-fill'} />
							</span>
							<p className="products-page-action-card-txt">{t('products')}</p>
						</div>

						<Button className={'products-page-action-card-btn'} icon={'bi bi-plus-circle-fill'} onClick={() => setOpenCreateEditModal({ visible: true })} label={t('create_product')} aria-label={t('aria_kweker_create_product')} />
					</div>
				</section>

				<section aria-label={t('aria_kweker_products_list')}>
					<GridTable
						isLazy
						itemsPerPage={12}
						data={paginatedProducts?.data || []}
						loading={paginatedProductsState.type == 'loading'}
						totalItems={paginatedProducts?.totalCount || 0}
						onFetchData={handleFetchProducts}
						title={t('your_products')}
						icon={<i className="bi bi-bag-fill"></i>}
						renderItem={(item, index) => (
							<ProductCard
								isKoper={false}
								product={item}
								index={index}
								onAction={(action, product) => {
									if (action == 'edit') {
										setOpenCreateEditModal({ visible: true, product: product });
									} else if (action == 'set_pricing') {
										console.log('Set pricing for product', product);
									} else if (action == 'delete') {
										handleDeleteProduct(product);
									}
								}}
							/>
						)}
						emptyText={t('no_orders')}
					/>
				</section>

				<Modal enabled={openCreateEditModal.visible} onClose={() => setOpenCreateEditModal({ visible: false })}>
					<CreateEditProduct product={openCreateEditModal.product} onClose={() => setOpenCreateEditModal({ visible: false })} />
				</Modal>

				<Modal enabled={productState.type !== 'idle'} onClose={() => setProductState({ type: 'idle' })}>
					<ComponentStateCard state={productState} onClose={() => setProductState({ type: 'idle' })} />
				</Modal>
			</main>
		</Page>
	);
}

export default KwekerProducts;
