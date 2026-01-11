// External imports
import {LayoutGroup, motion} from "framer-motion";
import React, {useCallback, useState} from 'react';

// Internal imports
import {OrderKwekerOutput} from "../../../declarations/dtos/output/OrderKwekerOutput";
import {useRootContext} from "../../contexts/RootContext";
import {useComponentStateReducer} from "../../../hooks/useComponentStateReducer";
import Button from "../../buttons/Button";
import {ClientAvatar} from "../../elements/ClientAvatar";
import {delay, formatEur} from "../../../utils/standards";
import {OrderStatus} from "../../../declarations/enums/OrderStatus";
import OrderStatusDropdown from "../../buttons/OrderStatusDropdown";
import {updateOrderStatus} from "../../../controllers/server/kweker";
import ComponentState from "../../elements/ComponentState";
import {StatusBadge} from "../../elements/StatusBadge";
import {isHttpError} from "../../../declarations/types/HttpError";

type Props = {
	order: OrderKwekerOutput;
	editMode?: boolean;
	printMode?: boolean;
	onClose?: () => void;
};

function OrderDetails(props: Props) {
	const {order, editMode, printMode, onClose} = props;
	const {t} = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	const [status, setStatus] = useState<OrderStatus>(order.status);

	const handleStatusChange = (newStatus: OrderStatus) => {
		setStatus(newStatus);
	}

	const updateHandler = useCallback(async () => {
		try {
			if (status === order.status) return; // No change
			updateState({type: 'loading', message: t('order_updating')});
			await updateOrderStatus(order.id, status);
			await delay(1500);
			updateState({type: 'succeed', message: t('order_update_success')});
			order.status = status;
		} catch (e: any) {
			await delay(2000);

			// Display error message
			if (isHttpError(e) && e.message)
				updateState({type: 'error', message: e.message});
			else
				updateState({type: 'error', message: t('order_update_error')});

			// Revert status on error
			setStatus(order.status);
		} finally {
			await delay(1500);
			updateState({type: 'idle'});
		}
	}, [order, status]);

	return (
		<LayoutGroup>
			<motion.div
				layout
				className={'order-card'}
				onClick={(e) => e.stopPropagation()}
			>
				{
					state.type === 'idle' && (
						<>
							<div className={'order-card-header'}>
								{
									!printMode &&
									<Button
										className="order-header-back-button"
										icon="bi-x"
										onClick={() => onClose?.()}
										type="button" aria-label={t('aria_back_button')}/>
								}
								<i className="bi bi-clipboard-check-fill order-card-logo"></i>
								<div className={'order-card-header-text-ctn'}>
									<h2 className={'order-card-h2'}>
										Order
									</h2>
									<h1 className={'order-card-h1'}>
										{order.id}
									</h1>
									<div className={'order-header-row align-self-start'}>
										<i className="bi bi-calendar-check-fill order-header-icon"></i>
										<span className={'order-header-txt'}>
											Placed on: {new Date(order.createdAt).toDateString()}
										</span>
									</div>
									<div className={'order-header-row align-self-start'}>
										<i className="bi bi-check-circle-fill order-header-icon"></i>
										<span className={'order-header-txt'}>
												Order Status:
											</span>
										<StatusBadge status={status}/>
										{editMode && <OrderStatusDropdown currentValue={status} onChange={handleStatusChange}/>}
									</div>
								</div>
							</div>
							<div className={'order-body'}>
								<div className={'order-client-ctn'}>
									<div className={'order-client-info-wrapper'}>
										<h4 className="order-client-info-h4">
											<i className="bi bi-person-circle mr-2"></i> Klantgegevens
										</h4>

										<span className="order-client-info-txt font-semibold text-base mt-2 mb-2">
											<ClientAvatar name={`${order.koperInfo.firstName} ${order.koperInfo.lastName}`}/>
											{order.koperInfo.firstName} {order.koperInfo.lastName}
										</span>
										<p className="order-client-info-txt">
											<i className="bi bi-envelope-fill mr-2"></i> {order.koperInfo.email}
										</p>
										<p className="order-client-info-txt">
											<i className="bi bi-telephone-fill mr-2"></i> {order.koperInfo.telephone}
										</p>
									</div>

									<div className={'order-client-info-wrapper'}>
										<h4 className="order-client-info-h4">
											<i className="bi bi-geo-alt-fill mr-2"></i> Afleveradres
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
										<i className="bi bi-basket-fill mr-2"></i> Bestelde Producten
									</h4>
									<div className="order-products-list">
										<div className={'order-product'}>
											<div className={'order-product-img-ctn'}>
												<img src={order.product.imageUrl} className={'order-product-img'}/>
											</div>
											<div className={'flex flex-col flex-1 h-auto'}>
												<p className={'order-product-name'}> {order.product.name}</p>
												<span className={'order-product-desc'}>
													 {order.product.description}
												</span>
												<span className={'order-product-price'}>
											        eenheidprijs {formatEur(order.totalPrice / order.quantity)}
												</span>
											</div>
											<div className={'flex flex-col gap-1'}
											>
												<span className={'order-product-total'}>
												{formatEur(order.totalPrice)}
											</span>
												<div className={'order-product-badge'}>
													Aantal x {order.quantity}
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>

							{
								editMode && <div className={'order-edit-actions'}>
									<i className={'order-line-sep'}/>
									<div className={'flex flex-row align-middle justify-center gap-2'}>
										<Button icon={'bi-check'} className={'order-edit-btn'} label={'Update Order'} onClick={updateHandler}/>
									</div>
								</div>
							}
						</>
					)
				}

				{state.type !== 'idle' && (
					<ComponentState state={state}/>
				)}
			</motion.div>
		</LayoutGroup>
	);
}

export default OrderDetails;