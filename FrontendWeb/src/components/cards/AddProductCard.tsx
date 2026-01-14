import React, {useCallback, useEffect} from 'react';
import {useRootContext} from "../contexts/RootContext";
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import {delay, formatEur} from "../../utils/standards";
import clsx from "clsx";
import Button from "../buttons/Button";
import {useComponentStateReducer} from "../../hooks/useComponentStateReducer";
import Modal from "../elements/Modal";
import {ComponentStateCard} from "../elements/ComponentState";
import {addProductToVeilingKlok, removeProductFromVeilingKlok} from "../../controllers/server/veilingmeester";
import {isHttpError} from "../../declarations/types/HttpError";

type ProductActions = 'add' | 'remove';

type ProductCardProps = {
	index: number;
	added?: boolean;
	mode?: 'selling' | 'meester' | 'normal';
	product: ProductOutputDto;
	veilingId?: string;
	onAction?(type: ProductActions, product: ProductOutputDto, index: number): void;
}

function AddProductCard(props: ProductCardProps) {
	const {veilingId, product, index, mode = 'normal'} = props;
	const {t} = useRootContext();

	const [added, setAdded] = React.useState(props.added ?? product.auctionPlanned);
	const [state, updateState] = useComponentStateReducer();

	useEffect(() => {
			setAdded(props.added ?? product.auctionPlanned);
		}, [props.added]
	);

	const removeProduct = useCallback(async () => {
		if (!veilingId) return;

		try {
			updateState({type: 'loading', message: t('veilingklok_removing_product')});
			await removeProductFromVeilingKlok(veilingId, product.id);
			await delay(1500);
			updateState({type: 'succeed', message: t('veilingklok_product_removed')});

			setAdded(false);
			props.onAction?.('remove', product, index);
		} catch (e: any) {
			if (isHttpError(e) && e.message)
				updateState({type: 'error', message: e.message});
			else
				updateState({type: 'error', message: t('veilingklok_product_remove_error')});
		} finally {
			await delay(2000);
			updateState({type: 'idle'});
		}
	}, [veilingId, product, index, props.onAction, t, updateState]);
	const addProduct = useCallback(async () => {
		if (!veilingId) return;

		try {
			updateState({type: 'loading', message: t('veilingklok_adding_product')});
			await addProductToVeilingKlok(veilingId, product.id, product.auctionedPrice ?? 0);
			await delay(1500);
			updateState({type: 'succeed', message: t('veilingklok_product_added')});

			setAdded(true);
			props.onAction?.('add', product, index);
		} catch (e: any) {
			if (isHttpError(e) && e.message)
				updateState({type: 'error', message: e.message});
			else
				updateState({type: 'error', message: t('veilingklok_product_add_error')});

		} finally {
			await delay(2000);
			updateState({type: 'idle'});
		}
	}, [veilingId, product, index, props.onAction, t, updateState]);

	const handleSelect = useCallback(() => {
		if (added) {
			removeProduct();
		} else {
			addProduct();
		}
	}, [added, removeProduct, addProduct]);

	return (
		<>
			<div className="product-card">
				<div className="product-card__shine"/>
				<div className="product-card__glow"/>
				<div className="product-card__content">
					{
						(mode == 'meester' && added) &&
						<div className={clsx("product-card__badge !opacity-100 !scale-100 !z-10", product.auctionPlanned ? 'bg-green-500' : 'bg-yellow-400')}>
							{t('added')}
						</div>
					}
					<div className="product-card__image">
						<img src="/pictures/flower-test.avif" alt={t('alt_flower_picture')}/>
					</div>
					<div className="product-card__text">
						<p className="product-card__title">{product.name}</p>
						<p className="product-card__description">{product.description}</p>
						{
							product.region &&
							<span className="product-card__icon-txt text-primary-800">
						<i className="bi bi-geo-alt-fill"></i>
								{product.region}, Netherlands
					</span>
						}
						<span className="product-card__icon-txt">
						<i className="bi bi-inbox-fill"></i>
							{t('stock_quantity')}: {product.stock} pcs
					</span>
					</div>
					<div className="product-card__footer">
						<div className="product-card__footer-prices">
							{mode == 'meester' ?
								<>
									<div className="product-card__price-secondary">{formatEur(product.minimumPrice ?? 0.00)}</div>
									<div className={clsx("product-card__price-primary", !product.auctionedPrice && 'text-red-500')}>
										<i className="bi bi-caret-up-fill"></i>
										{product.auctionedPrice ? formatEur(product.auctionedPrice) : '--.--'}
									</div>
								</>
								:
								<>
									<div className="product-card__price">{formatEur(product.minimumPrice ?? 0.00)}</div>
								</>
							}
						</div>

						<Button
							className={clsx('base-btn product-card__button', added && 'red')}
							icon={!added ? 'bi-plus-lg' : 'bi-x'}
							onClick={handleSelect}
						/>
					</div>
				</div>
			</div>

			<Modal enabled={state.type !== 'idle'} onClose={() => state.type !== 'loading' && updateState({type: 'idle'})}>
				<ComponentStateCard state={state}/>
			</Modal>
		</>
	);
}

export default AddProductCard;



