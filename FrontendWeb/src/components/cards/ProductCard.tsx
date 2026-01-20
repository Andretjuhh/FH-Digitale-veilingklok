import React, {useCallback} from 'react';
import {useRootContext} from '../contexts/RootContext';
import {ProductOutputDto} from '../../declarations/dtos/output/ProductOutputDto';
import CustomDropdown, {DropdownItem} from '../buttons/Dropdown';
import {formatEur} from '../../utils/standards';
import clsx from 'clsx';

export type ProductActions = 'edit' | 'delete' | 'set_pricing' | 'prijs_history';

type ProductCardProps = {
	index: number;
	isKoper?: boolean;
	mode?: 'selling' | 'meester' | 'normal';
	product: ProductOutputDto;
	onAction?(type: ProductActions, product: ProductOutputDto, index: number): void;
};

function ProductCard(props: ProductCardProps) {
	const {product, index, mode = 'normal', isKoper = false} = props;
	const {t} = useRootContext();

	const normalMenu: DropdownItem<ProductActions>[] = [
		{id: 'edit', label: t('edit_product'), type: 'button', as: 'button', icon: 'bi bi-pencil-fill'},
		{id: 'delete', label: t('delete_product'), type: 'button', as: 'button', icon: 'bi bi-trash3-fill'},
	];
	const meesterMenu: DropdownItem<ProductActions>[] = [{id: 'set_pricing', label: t('set_auction_product_price'), type: 'button', as: 'button', icon: 'bi bi-euro'}];

	const koperMenu: DropdownItem<ProductActions>[] = [{id: 'prijs_history', label: t('koper_footer_history'), type: 'button', as: 'button', icon: 'bi bi-euro'}];

	const handleSelect = useCallback(
		(type: ProductActions) => {
			props.onAction?.(type, product, index);
		},
		[props.onAction, product, index],
	);

	return (
		<div className="product-card">
			<div className="product-card__shine"/>
			<div className="product-card__glow"/>
			<div className="product-card__content">
				{(mode == 'meester' || mode == 'normal') && <div
					className={clsx('product-card__badge !opacity-100 !scale-100 !z-10', product.auctionPlanned ? 'bg-green-500' : 'bg-yellow-400')}>{product.auctionPlanned ? t('scheduled_for_auction') : t('no_veiling_planned')}</div>}
				<div className="product-card__image">
					<img src="/pictures/flower-test.avif" alt={t('alt_flower_picture')}/>
				</div>
				<div className="product-card__text">
					<p className="product-card__title">{product.name}</p>
					<p className="product-card__description">{product.description}</p>
					{product.region && (
						<span className="product-card__icon-txt text-primary-800">
							<i className="bi bi-geo-alt-fill"></i>
							{product.region}, Netherlands
						</span>
					)}
					<span className="product-card__icon-txt">
						<i className="bi bi-inbox-fill"></i>
						{t('stock_quantity')}: {product.stock} pcs
					</span>
				</div>
				<div className="product-card__footer">
					<div className="product-card__footer-prices">
						{mode == 'meester' ? (
							<>
								<div className="product-card__price-secondary">{formatEur((isKoper ? product.auctionedPrice : product.minimumPrice) ?? 0.0)}</div>
								<div className={clsx('product-card__price-primary', !product.auctionedPrice && 'text-red-500')}>
									<i className="bi bi-caret-up-fill"></i>
									{product.auctionedPrice ? formatEur(product.auctionedPrice) : '--.--'}
								</div>
							</>
						) : (
							<>
								<div className="product-card__price">{formatEur((isKoper ? product.auctionedPrice : product.minimumPrice) ?? 0.0)}</div>
							</>
						)}
					</div>

					{mode != 'selling' && (
						<CustomDropdown
							className={'product-card-dropdown'}
							menuClassName={'product-card-dropdown-menu'}
							buttonClassName={'base-btn product-card__button'}
							itemButtonClassName={'product-card-dropdown-item-btn'}
							buttonChildren={<i className="bi bi-three-dots-vertical"></i>}
							items={mode == 'meester' ? meesterMenu : isKoper ? koperMenu : normalMenu}
							onItemSelect={({id}) => handleSelect(id)}
						/>
					)}
				</div>
			</div>
		</div>
	);
}

export default ProductCard;
