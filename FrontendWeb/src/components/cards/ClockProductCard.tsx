import React from 'react';
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import {formatEur} from "../../utils/standards";
import clsx from "clsx";
import Button from "../buttons/Button";

type Props = {
	clockRunning?: boolean;
	product: ProductOutputDto;
	onStartAuctionClick?: () => void;

}

function ClockProductCard(props: Props) {
	const {product, clockRunning, onStartAuctionClick} = props;

	return (
		<div className={clsx('auction-product-card', clockRunning && 'auction-product-card-running')}>
			<div className={'auction-product-card-img-ctn'}>
				<img className={'auction-product-card-img'} src="/pictures/flower-test.avif"/>
			</div>

			<div className={'auction-product-card-texts'}>
				<p className={'auction-product-card-title'}>{product.name}</p>
				<p className={'auction-product-card-description'}>{product.description}</p>
			</div>

			<div className={'auction-product-card-digits'}>
				<span className={'auction-product-card-price'}>
					{product.auctionedPrice ? formatEur(product.auctionedPrice) : '-'}
				</span>

				<span
					className={clsx('auction-product-card-percentage', product.stock == 0 && 'negative')}>
					 <i className={'bi-inbox-fill'}/> {product.stock}
				</span>

			</div>

			<Button
				disabled={clockRunning || product.stock == 0}
				className={'auction-product-card-start-auction-btn'}
				onClick={onStartAuctionClick}
				icon={'bi-skip-end-fill'}
			/>
		</div>
	);
}

export default ClockProductCard;