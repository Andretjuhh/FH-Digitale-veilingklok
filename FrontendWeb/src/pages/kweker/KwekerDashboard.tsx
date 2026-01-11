import React, {useCallback, useEffect, useMemo, useState} from 'react';
import Button from '../../components/buttons/Button';
import FormInputField from '../../components/elements/FormInputField';
import {useForm} from 'react-hook-form';
import Page from '../../components/nav/Page';
import {createProduct, getKwekerStats, getProductDetails, getProducts} from '../../controllers/server/kweker';
import {formatEur} from '../../utils/standards';
import {ProductOutputDto} from '../../declarations/dtos/output/ProductOutputDto';
import {ProductDetailsOutputDto} from '../../declarations/dtos/output/ProductDetailsOutputDto';
import {KwekerStatsOutputDto} from '../../declarations/dtos/output/KwekerStatsOutputDto';
import {CreateProductDTO} from '../../declarations/dtos/input/CreateProductDTO';
import {useRootContext} from "../../components/contexts/RootContext";
import {useComponentStateReducer} from "../../hooks/useComponentStateReducer";
import KwekerStats from "../../components/sections/kweker/KwekerStats";
import {DataTable, Column} from "../../components/elements/Table";

export default function KwekerDashboard() {
	const {t, account} = useRootContext();
	const [stats, setStats] = useState<KwekerStatsOutputDto | null>(null);
	const [products, setProducts] = useState<ProductOutputDto[]>([]);
	const [isCreating, setIsCreating] = useState(false);
	const [showForm, setShowForm] = useState(false);
	const [state, updateState] = useComponentStateReducer();

	// Preview modal state()
	const [showPreview, setShowPreview] = useState(false);
	const [previewLoading, setPreviewLoading] = useState(false);
	const [previewError, setPreviewError] = useState<string | null>(null);
	const [previewProduct, setPreviewProduct] = useState<ProductDetailsOutputDto | null>(null);

	const {
		register,
		handleSubmit,
		setError,
		formState: {errors},
		reset,
		getValues,
	} = useForm<CreateProductDTO>({
		defaultValues: {name: '', description: '', minimumPrice: 0, stock: 0, imageBase64: '', dimension: ''},
	});

	const [savedValues, setSavedValues] = useState<CreateProductDTO | null>(null);

	useEffect(() => {
		initializeProducts();
	}, []);

	const initializeProducts = useCallback(async () => {
		try {
			const response = await getProducts();
			if (response.data) {
				setProducts(response.data.data);
			}

			const statsResponse = await getKwekerStats();
			if (statsResponse.data) {
				setStats(statsResponse.data);
			}
		} catch (err) {
			console.error('Failed to initialize dashboard', err);
		}
	}, []);

	const openProductPreview = useCallback(async (product: ProductOutputDto) => {
		setShowPreview(true);
		setPreviewLoading(true);
		setPreviewError(null);
		setPreviewProduct(null);
		try {
			const res = await getProductDetails(product.id);
			if (res.data) {
				setPreviewProduct(res.data);
			}
		} catch (err) {
			console.error('Failed to load product preview', err);
			setPreviewError(t('kweker_preview_error'));
		} finally {
			setPreviewLoading(false);
		}
	}, [t]);

	const productColumns: Column<ProductOutputDto>[] = useMemo(
		() => [
			{
				key: 'name',
				label: t('kweker_table_product_label'),
				sortable: true,
				render: (item: ProductOutputDto) => (
					<div className="app-table-cell-item">
						<div>
							<div className="app-table-cell-title">{item.name}</div>
							<div className="app-table-cell-subtitle">{item.description}</div>
						</div>
					</div>
				),
			},
			{
				key: 'stock',
				label: t('kweker_table_stock_label'),
				sortable: true,
				render: (item: ProductOutputDto) => (
					<div className="app-table-cell-title">{item.stock}</div>
				),
			},
			{
				key: 'auctionedPrice',
				label: t('kweker_table_price_label'),
				sortable: true,
				render: (item: ProductOutputDto) => (
					<div className="app-table-cell-title">{formatEur(item.auctionedPrice ?? 0)}</div>
				),
			},
			{
				key: 'action',
				label: t('kweker_table_action_label'),
				render: (item: ProductOutputDto, onAction?: (item: ProductOutputDto) => void) => (
					<button
						onClick={() => onAction?.(item)}
						className="app-table-action-btn"
						aria-label={t('kweker_table_action_view_aria', {name: item.name})}
					>
						{t('kweker_table_action_view')}
					</button>
				),
			},
		],
		[t]
	);

	return (
		<Page enableHeader className="kweker-page" enableHeaderAnimation={false}>
			<main className="kweker-container">
				<section className="kweker-hallo">
					<h1>
						{t('kweker_welcome', {
							firstName: account?.firstName ?? '',
							lastName: account?.lastName ?? '',
						})}
					</h1>
					<p className="kweker-desc">{t('kweker_desc')}</p>
				</section>
				<KwekerStats/>
				<section className="kweker-new-products">
					<h2 className="kweker-section-title">{t('kweker_section_products')}</h2>
					<DataTable<ProductOutputDto>
						data={products}
						columns={productColumns}
						itemsPerPage={5}
						onAction={openProductPreview}
					/>
				</section>
				<section className="kweker-content">
					<div className="content-inner">
						{!showForm && (
							<Button
								className="toevoegen-knop"
								label={t('kweker_add_product_button')}
								aria-label={t('kweker_add_product_button_aria')}
								onClick={() => {
									setShowForm(true);
									if (savedValues) reset(savedValues);
								}}
							>
								{t('kweker_add_product_button')}
							</Button>
						)}
						{showForm && (
							<div
								className="modal-overlay"
								onClick={() => {
									if (!isCreating) {
										setSavedValues(getValues());
										setShowForm(false);
									}
								}}
							>
								<div className="modal" onClick={(e) => e.stopPropagation()} role="dialog" aria-modal="true">
									<div className="modal-header">
										<h3>{t('kweker_add_product_modal_title')}</h3>
										<button
											className="modal-close"
											onClick={() => {
												setSavedValues(getValues());
												setShowForm(false);
											}}
											aria-label={t('kweker_modal_close_aria')}
										>
											?
										</button>
									</div>
									<div className="modal-body">
										<form
											onSubmit={handleSubmit(async (data) => {
												if (isCreating) return;
												setIsCreating(true);
												try {
													const response = await createProduct(data);
													if (response.data) {
														const p = response.data;
														const mapped: ProductOutputDto = {
															id: p.id,
															name: p.name,
															description: p.description,
															imageUrl: p.imageBase64,
															auctionedPrice: p.auctionPrice,
															auctionedAt: p.auctionedAt,
															dimension: p.dimension,
															stock: p.stock,
															companyName: p.companyName,
														};
														setProducts((prev) => [mapped, ...prev]);
														setShowForm(false);
														reset();
														setSavedValues(null);
													}
												} catch (err: any) {
													console.error('createProduct failed', err);
													setError('name', {type: 'server', message: t('kweker_form_create_error')});
												} finally {
													setIsCreating(false);
												}
											})}
											className="create-product-form"
										>
											<div className="form-row">
												<FormInputField id="name" label={t('kweker_form_name_label')} {...register('name', {required: t('kweker_form_name_required')})} isError={!!errors.name}
													error={errors.name?.message as string}/>
											</div>
											<div className="form-row">
												<label>{t('kweker_form_description_label')}</label>
												<textarea {...register('description', {required: t('kweker_form_description_required')})} />
											</div>
											<div className="form-row">
												<FormInputField id="minimumPrice" label={t('kweker_form_min_price_label')} type="number"
													step="0.01" {...register('minimumPrice', {required: t('kweker_form_min_price_required'), min: 0})}
													isError={!!errors.minimumPrice} error={errors.minimumPrice?.message as string}/>
											</div>
											<div className="form-row">
												<FormInputField id="stock" label={t('kweker_form_stock_label')} type="number" {...register('stock', {
													required: t('kweker_form_stock_required'),
													min: 0
												})} isError={!!errors.stock} error={errors.stock?.message as string}/>
											</div>
											<div className="form-row">
												<FormInputField id="imageBase64"
													label={t('kweker_form_image_label')} {...register('imageBase64')}
													isError={!!errors.imageBase64} error={errors.imageBase64?.message as string}/>
											</div>
											<div className="form-row">
												<FormInputField id="dimension" label={t('kweker_form_dimension_label')} {...register('dimension')} isError={!!errors.dimension}
													error={errors.dimension?.message as string}/>
											</div>
											<div className="form-row form-actions">
												<button type="submit" className="toevoegen-knop btn-primary" disabled={isCreating}>
													{isCreating ? t('kweker_form_submit_busy') : t('kweker_form_submit')}
												</button>
												<button
													type="button"
													className="cancel-btn"
													onClick={() => {
														if (!isCreating) {
															setSavedValues(getValues());
															setShowForm(false);
														}
													}}
													disabled={isCreating}
												>{t('kweker_form_cancel')}</button>
											</div>
										</form>
									</div>
								</div>
							</div>
						)}
						{/* Product preview modal */}
						{showPreview && (
							<div
								className="modal-overlay"
								onClick={() => {
									if (!previewLoading) {
										setShowPreview(false);
										setPreviewProduct(null);
										setPreviewError(null);
									}
								}}
							>
								<div className="modal" onClick={(e) => e.stopPropagation()} role="dialog" aria-modal="true">
									<div className="modal-header">
										<h3>{t('kweker_preview_title')}</h3>
										<button
											className="modal-close"
											onClick={() => {
												setShowPreview(false);
												setPreviewProduct(null);
												setPreviewError(null);
											}}
											aria-label={t('kweker_modal_close_aria')}
										>
											?
										</button>
									</div>
									<div className="modal-body">
										{previewLoading && <div>{t('kweker_preview_loading')}</div>}
										{previewError && <div className="error">{previewError}</div>}
										{previewProduct && (
											<div className="product-details">
												<div className="product-details-grid">
													<div className="product-meta">
														<div className="product-meta-header">
															<h2 className="product-name">{previewProduct.name}</h2>
															<div className="product-price">{formatEur(previewProduct.auctionPrice ?? previewProduct.minimumPrice)}</div>
														</div>
														<div className="product-description">{previewProduct.description}</div>
														<div className="product-quantity">
															{t('kweker_preview_stock', {count: previewProduct.stock})}
														</div>
														{previewProduct.dimension && (
															<div className="product-size">
																{t('kweker_preview_size', {dimension: previewProduct.dimension})}
															</div>
														)}
														<div className="product-company">
															{t('kweker_preview_grower', {company: previewProduct.companyName})}
														</div>
													</div>
													<div className="product-image">{previewProduct.imageBase64 ?
															<img src={previewProduct.imageBase64} alt={previewProduct.name}/> :
															<div className="product-image-placeholder" aria-hidden="true"/>}</div>
													</div>
											</div>
										)}
									</div>
								</div>
							</div>
						)}
					</div>
				</section>
			</main>
		</Page>
	);
}




