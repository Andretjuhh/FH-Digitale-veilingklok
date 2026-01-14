import React from 'react';
import {ProductOutputDto} from "../../declarations/dtos/output/ProductOutputDto";
import {formatEur} from "../../utils/standards";
import clsx from "clsx";

type Props = {
	product: ProductOutputDto;
}

function AuctionProductCard(props: Props) {
	const {product} = props;

	return (
		<div className={'auction-product-card'}>
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

				{
					(product.auctionedPrice && product.minimumPrice) &&
					(<span
						className={clsx('auction-product-card-percentage', product.auctionedPrice == product.minimumPrice && 'equal', product.auctionedPrice < product.minimumPrice && 'negative')}>
					 <i className={'bi-caret-up-fill'}/> {Math.round(((product.auctionedPrice - product.minimumPrice) / product.minimumPrice) * 100)}%
					</span>)
				}

			</div>
		</div>
	);
}

export default AuctionProductCard;