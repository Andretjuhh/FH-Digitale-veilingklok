import React, {useCallback, useEffect, useState} from 'react';
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
import Table from "../../components/elements/Table";

export default function KwekerDashboard() {
	const {t, account} = useRootContext();
	const [stats, setStats] = useState<KwekerStatsOutputDto | null>(null);
	const [activeTab, setActiveTab] = useState<'my' | 'history'>('my');
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
	}, [activeTab]);

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

	return (
		<Page enableHeader className="kweker-page" enableHeaderAnimation={false}>
			<main className="kweker-container">
				<section className="kweker-hallo">
					<h1>Welcome, {account?.firstName} {account?.lastName}!</h1>
					<p className="kweker-desc">Welkom op de dashboard pagina! Bekijk hier uw producten!</p>
				</section>
				<KwekerStats/>
			<Table products={products}/>
				<section className="kweker-content">
					<div className="content-inner">
						<div className="content-footer">
							{!showForm && (
								<Button
									className="toevoegen-knop"
									label={'Voeg nieuw product toe'}
									onClick={() => {
										setShowForm(true);
										if (savedValues) reset(savedValues);
									}}
								>
									Toevoegen
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
													<h3>Nieuw product toevoegen</h3>
													<button
														className="modal-close"
														onClick={() => {
															setSavedValues(getValues());
															setShowForm(false);
														}}
														aria-label="Sluiten"
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
																		imageUrl: p.imageBase64, // Using base64 as preview
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
																setError('name', {type: 'server', message: 'Onbekende fout bij het aanmaken van product'});
															} finally {
																setIsCreating(false);
															}
														})}
														className="create-product-form"
													>
														<div className="form-row">
															<FormInputField id="name" label="Naam" {...register('name', {required: 'Naam is verplicht'})} isError={!!errors.name}
															                error={errors.name?.message as string}/>
														</div>

														<div className="form-row">
															<label>Beschrijving</label>
															<textarea {...register('description', {required: 'Beschrijving is verplicht'})} />
														</div>

														<div className="form-row">
															<FormInputField id="minimumPrice" label="Minimum prijs (�)" type="number"
															                step="0.01" {...register('minimumPrice', {required: 'Minimum prijs is verplicht', min: 0})}
															                isError={!!errors.minimumPrice} error={errors.minimumPrice?.message as string}/>
														</div>

														<div className="form-row">
															<FormInputField id="stock" label="Aantal" type="number" {...register('stock', {
																required: 'Aantal is verplicht',
																min: 0
															})} isError={!!errors.stock} error={errors.stock?.message as string}/>
														</div>

														<div className="form-row">
															<FormInputField id="imageBase64"
															                label="Afbeelding (Base64)" {...register('imageBase64')}
															                isError={!!errors.imageBase64} error={errors.imageBase64?.message as string}/>
														</div>

														<div className="form-row">
															<FormInputField id="dimension" label="Maat" {...register('dimension')} isError={!!errors.dimension}
															                error={errors.dimension?.message as string}/>
														</div>

														<div className="form-row form-actions">
															<button type="submit" className="toevoegen-knop btn-primary" disabled={isCreating}>
																{isCreating ? 'Bezig...' : 'Voeg nieuw product toe'}
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
															>
																Annuleren
															</button>
														</div>
													</form>
												</div>
											</div>
										</div>
									)}

									{/* Product preview modal kan verwijderd worden of behouden voor toekomstig gebruik */}
								</div>
							</div>
						</section>
					</main>
				</Page>
			);
		}
		