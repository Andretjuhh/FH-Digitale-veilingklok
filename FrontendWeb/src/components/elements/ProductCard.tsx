import React, {useCallback} from 'react';
import {useRootContext} from "../contexts/RootContext";
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import CustomDropdown, {DropdownItem} from "../buttons/Dropdown";

type ProductCardProps = {
	index: number;
	product: ProductOutputDto
	onEdit?: (product: ProductOutputDto, index: number) => void;
	onDelete?: (product: ProductOutputDto, index: number) => void;
}

function ProductCard(props: ProductCardProps) {
	const {product, index} = props;
	const {t} = useRootContext();

	const menu: DropdownItem[] = [
		{id: 'edit', label: t('edit_product'), type: 'button', as: 'button', icon: 'bi bi-pencil-fill'},
		{id: 'delete', label: t('delete_product'), type: 'button', as: 'button', icon: 'bi bi-trash3-fill'},
	];

	const handleSelect = useCallback((id: string) => {
		if (id === 'edit' && props.onEdit) {
			props.onEdit(product, index);
		} else if (id === 'delete' && props.onDelete) {
			props.onDelete(product, index);
		}
	}, []);

	return (
		<div className="product-card">
			<div className="product-card__shine"/>
			<div className="product-card__glow"/>
			<div className="product-card__content">
				<div className="product-card__badge">NEW</div>
				<div className="product-card__image">
					<img src="/pictures/flower-test.avif" alt={t('alt_flower_picture')}/>
				</div>
				<div className="product-card__text">
					<p className="product-card__title">Premium Design</p>
					<p className="product-card__description">Hover to reveal stunning effects</p>
					<span className="product-card__icon-txt text-primary-800">
						<i className="bi bi-geo-alt-fill"></i>
						Amsterdam, Netherlands
					</span>
					<span className="product-card__icon-txt">
						<i className="bi bi-inbox-fill"></i>
						{t('stock')}: 1500 pcs
					</span>
				</div>
				<div className="product-card__footer">
					<div className="product-card__price">$49.99</div>
					<CustomDropdown
						className={'product-card-dropdown'}
						menuClassName={'product-card-dropdown-menu'}
						buttonClassName={'base-btn product-card__button'}
						itemButtonClassName={'product-card-dropdown-item-btn'}
						buttonChildren={<i className="bi bi-three-dots-vertical"></i>}
						items={menu}
						onItemSelect={({id}) => handleSelect(id)}
					/>
				</div>
			</div>
		</div>
	);
}

export default ProductCard;