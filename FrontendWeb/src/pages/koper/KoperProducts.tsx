import React, {useCallback, useState} from 'react';
import {useRootContext} from "../../components/contexts/RootContext";
import {useComponentStateReducer} from "../../hooks/useComponentStateReducer";
import {PaginatedOutputDto} from "../../declarations/dtos/output/PaginatedOutputDto";
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import {OnFetchHandlerParams} from "../../components/layout/Table";
import {getProducts} from "../../controllers/server/koper";
import Page from "../../components/nav/Page";
import GridTable from "../../components/layout/GridTable";
import ProductCard from "../../components/cards/ProductCard";

function KoperProducts() {
	const {t, account} = useRootContext();

	// State for Create/Edit Product Modal
	const [paginatedProductsState, setPaginatedProductsState] = useComponentStateReducer();
	const [paginatedProducts, setPaginatedProducts] = useState<PaginatedOutputDto<ProductOutputDto>>();

	const handleFetchProducts = useCallback(async (params: OnFetchHandlerParams) => {
		try {
			setPaginatedProductsState({type: 'loading'});
			const response = await getProducts(
				params.searchTerm,
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
		<Page enableHeader className="kweker-products-page" enableHeaderAnimation={false} headerClassName={'header-normal-sticky'}>
			<main className="kweker-products-page-ctn">
				<section className="page-title-section">
					<h1>
						{t('welcome')}, {account?.firstName} {account?.lastName}
					</h1>
					<h2>
						{t('kweker_products_description')}
					</h2>
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
							isKoper={true}
							product={item}
							index={index}
						/>)
					}
					emptyText={t('no_orders')}
				/>

			</main>
		</Page>
	);
}

export default KoperProducts;