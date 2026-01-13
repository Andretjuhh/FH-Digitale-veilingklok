import React, {useCallback, useState} from 'react';
import Page from "../../components/nav/Page";
import {useRootContext} from "../../components/contexts/RootContext";
import {KwekerProductStats} from "../../components/sections/kweker/KwekerStats";
import GridTable from "../../components/layout/GridTable";
import ProductCard from "../../components/elements/ProductCard";
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import CreateEditProduct from "../../components/sections/kweker/CreateEditProduct";
import {PaginatedOutputDto} from "../../declarations/dtos/output/PaginatedOutputDto";
import {useComponentStateReducer} from "../../hooks/useComponentStateReducer";
import {OnFetchHandlerParams} from "../../components/layout/Table";
import {getProducts} from "../../controllers/server/kweker";
import Button from "../../components/buttons/Button";

function KwekerProducts() {
	const {t, account} = useRootContext();

	// State for Create/Edit Product Modal
	const [paginatedProductsState, setPaginatedProductsState] = useComponentStateReducer();
	const [paginatedProducts, setPaginatedProducts] = useState<PaginatedOutputDto<ProductOutputDto>>();
	const [openCreateEditModal, setOpenCreateEditModal] = useState<{ visible: boolean; edit?: ProductOutputDto }>({visible: false});

	const handleFetchProducts = useCallback(async (params: OnFetchHandlerParams) => {
		try {
			setPaginatedProductsState({type: 'loading'});
			const response = await getProducts(
				params.searchTerm,
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
		<Page enableHeader className="kweker-page" enableHeaderAnimation={false}>
			<main className="kweker-container">
				<section className="kweker-hallo">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2>
						{t('kweker_products_description')}
					</h2>
				</section>

				<section className={'kweker-stats-wrapper'}>
					<KwekerProductStats/>
					<div className="kweker-add-product-card">
						<div className="kweker-add-product-card-title">
					        <span>
						         <i className={'bi bi-bag-plus-fill'}/>
			                </span>
							<p className="kweker-stat-txt">
								{t('products')}
							</p>
						</div>

						<Button
							className={'kweker-add-product-btn'}
							icon={'bi bi-plus-circle-fill'}
							onClick={() => setOpenCreateEditModal({visible: true})}
							label={t('create_product')}
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

					title={t('your_products')}
					icon={<i className="bi bi-bag-fill"></i>}
					renderItem={(item, index) => (
						<ProductCard
							isKoper={false}
							product={item}
							index={index}
							onEdit={(product) => setOpenCreateEditModal({visible: true, edit: product})}
							onDelete={(product) => console.log('Delete product', product)}
						/>)
					}
					emptyText={t('no_orders')}
					className={'kweker-products-grid-table'}
				/>

				{openCreateEditModal.visible && (
					<div className="modal-overlay" onClick={() => setOpenCreateEditModal({visible: false})}>
						<CreateEditProduct editProduct={openCreateEditModal.edit} onClose={() => setOpenCreateEditModal({visible: false})}/>
					</div>
				)}
			</main>
		</Page>
	);
}

export default KwekerProducts;