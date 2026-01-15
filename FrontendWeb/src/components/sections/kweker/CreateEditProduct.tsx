import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {CreateProductDTO} from '../../../declarations/dtos/input/CreateProductDTO';
import {UpdateProductDTO} from '../../../declarations/dtos/input/UpdateProductDTO';
import {useRootContext} from '../../contexts/RootContext';
import {useComponentStateReducer} from '../../../hooks/useComponentStateReducer';
import {LayoutGroup, motion} from 'framer-motion';
import {ProductOutputDto} from '../../../declarations/dtos/output/ProductOutputDto';
import Button from '../../buttons/Button';
import ComponentState from '../../elements/ComponentState';
import {useForm} from 'react-hook-form';
import FormInputField from '../../form-elements/FormInputField';
import FormSelectField from '../../form-elements/FormSelectField';
import FormTextareaField from '../../form-elements/FormTextareaField';
import clsx from 'clsx';
import {buildFieldLayout, ProductFormFields} from '../../../constant/forms';
import {InputField} from '../../../declarations/types/FormField';
import {createProduct, updateProduct} from "../../../controllers/server/kweker";
import {delay} from "../../../utils/standards";
import {isHttpError} from "../../../declarations/types/HttpError";
import {getRegions} from "../../../controllers/server/account";

type Props = {
	product?: ProductOutputDto;
	onClose?: () => void;
	onCreate?: (product: CreateProductDTO) => void;
	onUpdate?: (product: UpdateProductDTO) => void;
};
type Region = {
	id: string;
	name: string;
};

// Define the form data shape
type ProductFormState = {
	imageBase64: string;
	product_name: string;
	product_description: string;
	region: string;
	minimum_price: number;
	stock_quantity: number;
	product_dimension: string;
};

function CreateEditProduct(props: Props) {
	const {product, onClose, onCreate, onUpdate} = props;
	const {t} = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	const {
		register,
		handleSubmit,
		setValue,
		watch,
		formState: {errors},
	} = useForm<ProductFormState>({
		defaultValues: {
			product_name: product?.name || '',
			product_description: product?.description || '',
			region: product?.region || '', // Default, map if needed
			minimum_price: product?.minimumPrice || 0,
			stock_quantity: product?.stock || 0,
			product_dimension: product?.dimension || '',
			imageBase64: '',
		},
	});
	const imageBase64 = watch('imageBase64');

	// State for image preview
	const [imagePreview, setImagePreview] = useState<string | null>(product?.imageUrl || null);
	const [regions, setRegions] = useState<Region[]>([]);
	const [loadingRegions, setLoadingRegions] = useState(false);

	const regionOptions = useMemo(() => regions.map((r) => ({value: r.id, label: r.name})), [regions]);
	const orderedFields = useMemo(() => buildFieldLayout(ProductFormFields), []);

	useEffect(() => {
		initializeRegions();
	}, []);

	// Populate form when product changes (if needed, though defaultValues handles initial render)
	useEffect(() => {
		if (product) {
			setValue('product_name', product.name);
			setValue('product_description', product.description);
			setValue('stock_quantity', product.stock);
			setValue('product_dimension', product.dimension);
			setValue('region', product.region || '');
			setValue('minimum_price', product.minimumPrice || 0);

			if (product.imageUrl) {
				setImagePreview(product.imageUrl);
			}
			// Map other fields if they exist in DTO
		}
	}, [product, setValue, regions]);

	const initializeRegions = async () => {
		try {
			setLoadingRegions(true);
			const response = await getRegions();
			setRegions(
				[
					{id: '', name: t('select_region')} as Region,
					...response.data!.map((region) => ({id: region, name: region})),
				]
			);
			setLoadingRegions(false);
			updateState({type: 'idle'});

		} catch (error) {
			console.error('Failed to load regions:', error);
			updateState({type: 'error', message: t('failed_load_regions')});
			setLoadingRegions(false);
		}
	};
	const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
		const file = e.target.files?.[0];
		if (file) {
			if (!file.type.startsWith('image/')) {
				// Handle error
				return;
			}

			const reader = new FileReader();
			reader.onloadend = () => {
				const base64String = (reader.result as string).split(',')[1];
				setImagePreview(reader.result as string);
				setValue('imageBase64', base64String);
			};
			reader.readAsDataURL(file);
		}
	};
	const onFormSubmit = useCallback(async (data: ProductFormState) => {
		const productData: CreateProductDTO | UpdateProductDTO = {
			name: data.product_name,
			description: data.product_description,
			minimumPrice: Number(data.minimum_price), // cast string input to number if necessary
			stock: Number(data.stock_quantity),
			dimension: data.product_dimension,
			imageBase64: data.imageBase64,
			region: data.region == '' ? undefined : data.region,
		};
		console.log('Submitting product data:', productData);
		if (product)
			await onUpdateProduct(productData as UpdateProductDTO);
		else
			await onCreateProduct(productData as CreateProductDTO);
	}, [product, onCreate, onUpdate]);
	const onCreateProduct = async (createProductDTO: CreateProductDTO) => {
		try {
			updateState({type: 'loading', message: t('product_creating')});
			await createProduct(createProductDTO);
			await delay(1500);
			updateState({type: 'succeed', message: t('product_created_success')});
			onCreate?.(createProductDTO);
			await delay(1000);
			return onClose?.();
		} catch (e: any) {
			await delay(2000);

			// Display error message
			if (isHttpError(e) && e.message)
				updateState({type: 'error', message: e.message});
			else
				updateState({type: 'error', message: t('product_created_error')});

		} finally {
			await delay(2000);
			updateState({type: 'idle'});
		}
	}
	const onUpdateProduct = async (updateProductDTO: UpdateProductDTO) => {
		try {
			if (!product) return;
			updateState({type: 'loading', message: t('product_updating')});
			await updateProduct(product.id, updateProductDTO);
			await delay(1500);
			updateState({type: 'succeed', message: t('product_updated_success')});
			onUpdate?.(updateProductDTO);
			await delay(1000);
			return onClose?.();
		} catch (e: any) {
			await delay(2000);

			// Display error message
			if (isHttpError(e) && e.message)
				updateState({type: 'error', message: e.message});
			else
				updateState({type: 'error', message: t('product_updated_error')});

		} finally {
			await delay(2000);
			updateState({type: 'idle'});
		}
	}

	const renderField = useCallback(
		(field: InputField, key: string) => {
			const name = field.label as keyof ProductFormState;
			const isRequired = field.required;
			const errorMsg = errors[name]?.message;

			const commonProps = {
				id: name,
				disabled: field.disabled,
				label: isRequired ? `${t(field.label)} *` : t(field.label),
				placeholder: field.placeholderLocalizedKey ? t(field.placeholderLocalizedKey) : field.placeholder,
				isError: !!errorMsg,
				error: errorMsg,
			};

			if (field.type === 'textarea') {
				return <FormTextareaField
					key={key}
					rows={field.rows || 4}
					{...commonProps}
					{...register(name, {
						required: isRequired ? `${t(field.label)}  ${t('is')} ${t('required')}` : false,
					})}
				/>;
			}

			if (field.type === 'select') {
				// @ts-ignore
				return <FormSelectField
					key={key}
					icon={field.icon}
					options={regionOptions}
					{...commonProps}
					disabled={loadingRegions}
					{...register(name, {
						required: isRequired ? `${t(field.label)}  ${t('is')} ${t('required')}` : false,
					})}
				/>;
			}

			return (
				<FormInputField
					key={key}
					{...commonProps}
					type={field.type === 'number' ? 'number' : 'text'}
					step={field.step}
					min={field.min}
					icon={field.icon}
					{...register(name, {
						required: isRequired ? `${t(field.label)}  ${t('is')} ${t('required')}` : false,
						min: field.min ? {value: field.min, message: t('required_field')} : undefined, // simplified validation
					})}
				/>
			);
		},
		[errors, register, t, loadingRegions, regionOptions]
	);

	return (
		<LayoutGroup>
			<motion.div layout className={'modal-card create-product-card auto-width'} onClick={(e) => e.stopPropagation()}>
				{state.type === 'idle' && (
					<>
						<div className={'create-product-card-header'}>
							<Button className="modal-card-back-btn" icon="bi-x" onClick={() => onClose?.()} type="button" aria-label={t('aria_back_button')}/>

							<div className={'create-product-card-header-text-ctn'}>
								<h1 className={'create-product-card-h1'}>
									<i className="bi bi-bag-plus-fill create-product-card-header-icon"></i>
									{product ? t('edit_product') : t('create_product')}
								</h1>
							</div>
						</div>

						<div className="create_edit-product-card-body">
							<form
								className="create-product-card-form"
								onSubmit={handleSubmit(onFormSubmit)}
							>
								<div className="image-section">
									<span className="upload-input-area-label">{t('product_image')} *</span>
									<div className="upload-input-ctn">
										<input className="upload-input" type="file" accept="image/*" id="upload-input" onChange={handleImageUpload}/>
										<label htmlFor="upload-input" className={clsx('upload-input-label', imagePreview && 'has-preview', errors.imageBase64 && 'has-error')}>
											{imagePreview ? (
												<div className="upload-input-content">
													<div className={'upload-input-img-parent'}>
														<img src={imagePreview} alt="Preview" className="upload-input-img"/>
													</div>
													<p className="upload-input-change-txt">{t('change_image')}</p>
												</div>
											) : (
												<div className="upload-input-content">
													<i className="bi bi-upload upload-input-icon"/>
													<div>
														<p className="upload-input-txt-primary">{t('upload_image')}</p>
														<p className="upload-input-txt-secondary">{t('image_upload_instructions')}</p>
													</div>
												</div>
											)}
										</label>
									</div>
								</div>

								<div className="create-product-card-inputs-section">
									{orderedFields.map((item) =>
										item.type === 'group' ? (
											<div key={`group-${item.groupName}`} className="create-product-card-form-grid">
												{item.fields.map((field, fieldIndex) => renderField(field, `${item.groupName}-${field.label}-${fieldIndex}`))}
											</div>
										) : (
											renderField(item.field, `field-${item.field.label}`)
										)
									)}
								</div>

								<div className="create-product-card-submit-section">
									<Button
										className="submit-btn"
										label={product ? t('update_product_button') : t('create_product_button')}
										onClick={handleSubmit(onFormSubmit)}
									/>
								</div>
							</form>
						</div>
					</>
				)}

				{state.type !== 'idle' && <ComponentState state={state}/>}
			</motion.div>
		</LayoutGroup>
	);
}

export default CreateEditProduct;