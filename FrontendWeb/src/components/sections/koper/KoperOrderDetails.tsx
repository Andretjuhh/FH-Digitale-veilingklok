// External imports
import {LayoutGroup, motion} from 'framer-motion';
import React, {useCallback, useEffect, useState} from 'react';

// Internal imports
import {OrderKoperOutputDto} from '../../../declarations/dtos/output/OrderKoperOutputDto';
import {useRootContext} from '../../contexts/RootContext';
import {useComponentStateReducer} from '../../../hooks/useComponentStateReducer';
import Button from '../../buttons/Button';
import {ClientAvatar} from '../../elements/ClientAvatar';
import {delay, formatEur} from '../../../utils/standards';
import ComponentState from '../../elements/ComponentState';
import {StatusBadge} from '../../elements/StatusBadge';
import {getOrder} from '../../../controllers/server/koper';
import {isHttpError} from '../../../declarations/types/HttpError';

type Props = {
	orderId: string;
	printMode?: boolean;
	onClose?: () => void;
};

function KoperOrderDetails(props: Props) {
	const {orderId, printMode, onClose} = props;
	const {t} = useRootContext();
	const [state, updateState] = useComponentStateReducer({type: 'loading', message: t('loading_orders')});
	const [order, setOrder] = useState<OrderKoperOutputDto>();

	useEffect(() => {
		initialize().then(null);
	}, [orderId]);

	const initialize = useCallback(async () => {
		try {
			updateState({type: 'loading', message: t('loading_orders')});
			const response = await getOrder(orderId);
			setOrder(response.data);
			await delay(1500);
			updateState({type: 'idle', message: t('loaded_orders')});
		} catch (e) {
			// Display error message
			if (isHttpError(e) && e.message) updateState({type: 'error', message: e.message});
			else updateState({type: 'error', message: t('failed_load_orders')});
		}
	}, [orderId]);

	return (
		<LayoutGroup>
			<motion.div layout className={'modal-card order-card auto-width'} onClick={(e) => e.stopPropagation()}>
				{state.type === 'idle' && order && (
					<>
						<div className={'order-card-header'}>
							{!printMode && <Button className="order-header-back-button" icon="bi-x" onClick={() => onClose?.()} type="button" aria-label={t('aria_back_button')}/>}
							<i className="bi bi-clipboard-check-fill order-card-logo"></i>
							<div className={'order-card-header-text-ctn'}>
								<h2 className={'order-card-h2'}>Order</h2>
								<h1 className={'order-card-h1'}>{order.id}</h1>
								<div className={'order-header-row align-self-start'}>
									<i className="bi bi-calendar-check-fill order-header-icon"></i>
									<span className={'order-header-txt'}>Placed on: {new Date(order.createdAt).toDateString()}</span>
								</div>
								<div className={'order-header-row align-self-start'}>
									<i className="bi bi-check-circle-fill order-header-icon"></i>
									<span className={'order-header-txt'}>Order Status:</span>
									<StatusBadge status={order.status}/>
								</div>
							</div>
						</div>
						<div className={'order-body'}>
							<div className={'order-client-ctn'}>
								<div className={'order-client-info-wrapper'}>
									<h4 className="order-client-info-h4">
										<i className="bi bi-person-circle mr-2"></i> {t('kweker_informations')}
									</h4>

									<span className="order-client-info-txt font-semibold text-base mt-2 mb-2">
										<ClientAvatar name={order.kwekerInfo.companyName}/>
										{order.kwekerInfo.companyName}
									</span>
									<p className="order-client-info-txt">
										<i className="bi bi-envelope-fill mr-2"></i> {order.kwekerInfo.email}
									</p>
									<p className="order-client-info-txt">
										<i className="bi bi-telephone-fill mr-2"></i> {order.kwekerInfo.telephone}
									</p>
								</div>

								<div className={'order-client-info-wrapper'}>
									<h4 className="order-client-info-h4">
										<i className="bi bi-geo-alt-fill mr-2"></i>
										{t('deliver_adres')}
									</h4>
									<div className="space-y-1">
										<p className="text-sm text-zinc-800 leading-relaxed">
											{order.koperInfo.address.street}
											<br/>
											<span className="font-medium">
												{order.koperInfo.address.postalCode} {order.koperInfo.address.city}
											</span>
											<br/>
											<span className="text-zinc-500 uppercase text-xs tracking-wider">
												{order.koperInfo.address.regionOrState}, {order.koperInfo.address.country}
											</span>
										</p>
									</div>
								</div>
							</div>
							<div className={'order-products'}>
								<h4 className="order-client-info-h4">
									<i className="bi bi-basket-fill mr-2"></i> {t('ordered_products')}
								</h4>
								<div className="order-products-list">
									{order.products.map((prod) => (
										<div className={'order-product'}>
											<div className={'order-product-img-ctn'}>
												<img src={prod.productImageUrl} className={'order-product-img'}/>
											</div>
											<div className={'flex flex-col flex-1 h-auto'}>
												<p className={'order-product-name'}> {prod.productName}</p>
												<span className={'order-product-desc'}>{prod.productDescription}</span>
												<span className={'order-product-price'}>eenheidprijs {formatEur(prod.priceAtPurchase!)}</span>
											</div>
											<div className={'flex flex-col gap-1'}>
												<span className={'order-product-total'}>{formatEur(prod.priceAtPurchase! * prod.quantity)}</span>
												<div className={'order-product-badge'}>Aantal x {prod.quantity}</div>
											</div>
										</div>
									))}
								</div>
							</div>
						</div>
					</>
				)}

				{state.type !== 'idle' && <ComponentState state={state}/>}
			</motion.div>
		</LayoutGroup>
	);
}

export default KoperOrderDetails;
