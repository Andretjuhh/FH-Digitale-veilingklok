import React, {useState} from 'react';
import Page from "../../components/nav/Page";
import {useRootContext} from "../../components/contexts/RootContext";
import {KwekerProductStats} from "../../components/sections/kweker/KwekerStats";
import GridTable from "../../components/layout/GridTable";
import ProductCard from "../../components/elements/ProductCard";
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import CreateEditProduct from "../../components/sections/kweker/CreateEditProduct";

function KwekerProducts() {
	const {t, account} = useRootContext();

	// State for Create/Edit Product Modal
	const [openCreateEditModal, setOpenCreateEditModal] = useState<{ visible: boolean; edit?: ProductOutputDto }>({visible: false});

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
				<KwekerProductStats/>
				<GridTable
					data={Array(100).fill({})}
					itemsPerPage={24}
					icon={<i className="bi bi-bag-fill"></i>}
					title={t('your_products')}
					renderItem={(item, index) => (
						<ProductCard
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