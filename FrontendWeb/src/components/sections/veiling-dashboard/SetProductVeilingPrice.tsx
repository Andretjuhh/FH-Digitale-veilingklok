import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {ProductOutputDto} from "../../../declarations/dtos/output/ProductOutputDto";
import {useRootContext} from "../../contexts/RootContext";
import {useComponentStateReducer} from "../../../hooks/useComponentStateReducer";
import {useForm} from "react-hook-form";
import {buildFieldLayout, EditProductPriceFormFields} from "../../../constant/forms";
import {InputField} from "../../../declarations/types/FormField";
import FormTextareaField from "../../form-elements/FormTextareaField";
import FormSelectField from "../../form-elements/FormSelectField";
import FormInputField from "../../form-elements/FormInputField";
import {LayoutGroup, motion} from "framer-motion";
import Button from "../../buttons/Button";
import clsx from "clsx";
import ComponentState from "../../elements/ComponentState";
import {delay} from "../../../utils/standards";
import {isHttpError} from "../../../declarations/types/HttpError";
import {updateProductPrice} from "../../../controllers/server/veilingmeester";


type Props = {
	product: ProductOutputDto;
	onClose?: () => void;
	onSubmit?: (product: ProductOutputDto) => void;
};

// Define the form data shape
type ProductFormState = {
	imageBase64: string;
	product_name: string;
	product_description: string;
	region: string;
	minimum_price: number;
	product_veiling_start_price: number;
	product_dimension: string;
};

function SetProductVeilingPrice(props: Props) {
	const {product, onClose, onSubmit} = props;
	const {t} = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	const {
		register,
		handleSubmit,
		setValue,
		formState: {errors},
	} = useForm<ProductFormState>({
		defaultValues: {
			product_name: product.name,
			product_description: product.description,
			product_dimension: product.dimension || '',
			region: product.region || '',
			minimum_price: product.minimumPrice || 0,
			product_veiling_start_price: product.auctionedPrice || 0,
		},
	});

	// State for image preview
	const [imagePreview, setImagePreview] = useState<string | null>(product?.imageUrl || null);

	// Populate form when product changes (if needed, though defaultValues handles initial render)
	useEffect(() => {
		setValue('product_name', product.name);
		setValue('product_description', product.description);
		setValue('product_veiling_start_price', product.auctionedPrice || 0);
		setValue('product_dimension', product.dimension);
		setValue('region', product.region || '');
		setValue('minimum_price', product.minimumPrice || 0);
		if (product.imageUrl) {
			setImagePreview(product.imageUrl);
		}

	}, [product, setValue]);

	const orderedFields = useMemo(() => buildFieldLayout(EditProductPriceFormFields), []);
	const onFormSubmit = useCallback(async (data: ProductFormState) => {
		product.auctionedPrice = data.product_veiling_start_price;
		console.log('Submitting product data:', product);
		try {
			updateState({type: 'loading', message: t('product_updating')});
			await updateProductPrice(product.id, product.auctionedPrice);
			await delay(1500);
			updateState({type: 'succeed', message: t('product_updated_success')});
			onSubmit?.(product);
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
	}, [product]);
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
					options={[]}
					{...commonProps}
					disabled={true}
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
		[errors, register, t]
	);

	return (
		<LayoutGroup>
			<motion.div layout className={'create-product-card'} onClick={(e) => e.stopPropagation()}>
				{state.type === 'idle' && (
					<>
						<div className={'create-product-card-header'}>
							<Button className="modal-card-back-btn" icon="bi-x" onClick={() => onClose?.()} type="button" aria-label={t('aria_back_button')}/>
							<div className={'create-product-card-header-text-ctn'}>
								<h1 className={'create-product-card-h1'}>
									<i className="bi bi-tags-fill create-product-card-header-icon"></i>
									{t('set_auction_product_price')}
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
										<label htmlFor="upload-input" className={clsx('upload-input-label', imagePreview && 'has-preview', errors.imageBase64 && 'has-error')}>
											<div className="upload-input-content">
												<div className={'upload-input-img-parent'}>
													<img src="/pictures/flower-test.avif" alt="Preview" className="upload-input-img"/>
												</div>
											</div>
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

export default SetProductVeilingPrice;