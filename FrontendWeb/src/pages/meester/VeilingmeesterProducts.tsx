import React, {useCallback, useState} from 'react';
import {useRootContext} from "../../components/contexts/RootContext";
import {useComponentStateReducer} from "../../hooks/useComponentStateReducer";
import {PaginatedOutputDto} from "../../declarations/dtos/output/PaginatedOutputDto";
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import {OnFetchHandlerParams} from "../../components/layout/Table";
import {getProducts} from "../../controllers/server/veilingmeester";
import Page from "../../components/nav/Page";
import {KwekerProductStats} from "../../components/sections/kweker/KwekerStats";
import Button from "../../components/buttons/Button";
import GridTable from "../../components/layout/GridTable";
import ProductCard from "../../components/cards/ProductCard";
import SetProductVeilingPrice from "../../components/sections/veiling-dashboard/SetProductVeilingPrice";

function VeilingmeesterProducts() {
	const {t, account} = useRootContext();

	// State for Create/Edit Product Modal
	const [paginatedProductsState, setPaginatedProductsState] = useComponentStateReducer();
	const [paginatedProducts, setPaginatedProducts] = useState<PaginatedOutputDto<ProductOutputDto>>();
	const [openCreateEditModal, openPricingModal] = useState<{ visible: boolean; product?: ProductOutputDto }>({visible: false});

	const handleFetchProducts = useCallback(async (params: OnFetchHandlerParams) => {
		try {
			setPaginatedProductsState({type: 'loading'});
			const response = await getProducts(
				params.searchTerm,
				account?.region,
				undefined,
				undefined,
				undefined,
				params.page,
				params.pageSize
			);
			if (response.data) setPaginatedProducts(response.data);
			setPaginatedProductsState({type: 'succeed'});
		} catch (err) {
			console.error('Failed to fetch orders', err);
		}

	}, []);

	return (
		<Page enableHeader className="vm-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="vm-products-page-ctn">
				<section className="page-title-section">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2>
						{t('manage_product_price_txt')}
					</h2>
				</section>
				<section className={'products-page-stats'}>
					<KwekerProductStats/>

					<div className="products-page-action-card">
						<div className="products-page-action-card-title">
					        <span>
						         <i className={'bi bi-clock-fill'}/>
			                </span>
							<p className="products-page-action-card-txt">
								{t('veilingklok')}
							</p>
						</div>

						<Button
							className={'products-page-action-card-btn'}
							icon={'bi bi-plus-circle-fill'}
							label={t('schedule_veilingklok')}
						/>
					</div>
				</section>
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
						<ProductCard
							mode={'meester'}
							product={item}
							index={index}
							onAction={
								(action, product) => {
									if (action == 'edit') {
										openPricingModal({visible: true, product: product});
									} else if (action == 'set_pricing') {
										openPricingModal({visible: true, product: product});
									} else if (action == 'delete') {
										console.log('Delete product', product);
									}
								}
							}
						/>)
					}
					emptyText={t('no_orders')}
				/>

				{(openCreateEditModal.visible && openCreateEditModal.product) && (
					<div className="modal-overlay" onClick={() => openPricingModal({visible: false})}>
						<SetProductVeilingPrice product={openCreateEditModal.product} onClose={() => openPricingModal({visible: false})}/>
					</div>
				)}
			</main>
		</Page>
	);
}

export default VeilingmeesterProducts;