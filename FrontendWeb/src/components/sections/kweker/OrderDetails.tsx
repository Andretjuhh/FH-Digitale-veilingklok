import {LayoutGroup, motion} from "framer-motion";
import React from 'react';
import {OrderKwekerOutput} from "../../../declarations/dtos/output/OrderKwekerOutput";
import {useRootContext} from "../../contexts/RootContext";
import {useComponentStateReducer} from "../../../hooks/useComponentStateReducer";
import Button from "../../buttons/Button";
import {StatusBadge} from "../../elements/Table";
import {ClientAvatar} from "../../elements/ClientAvatar";
import {formatEur} from "../../../utils/standards";

type Props = {
	order: OrderKwekerOutput;
	editMode?: boolean;
};

function OrderDetails(props: Props) {
	const {order, editMode} = props;
	const {t, navigate, authenticateAccount} = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	return (
		<LayoutGroup>
			<motion.div layout className={'order-card'}>
				{
					state.type === 'idle' && (
						<>
							<div className={'order-card-header'}>
								<Button className="order-header-back-button" icon="bi-x" onClick={() => {
								}} type="button" aria-label={t('back_button_aria')}/>
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
										<StatusBadge status={order.status}/>
									</div>
								</div>
							</div>

							<div className={'order-body'}>
								<div className={'order-client-ctn'}>
									<div className={'order-client-info-wrapper'}>
										<h4 className="order-client-info-h4">
											<i className="bi bi-person-circle mr-2"></i> Klantgegevens
										</h4>

										<p className="order-client-info-txt font-semibold text-base mt-2 mb-2">
											<ClientAvatar name={`${order.koperInfo.firstName} ${order.koperInfo.lastName}`}/>
											{order.koperInfo.firstName} {order.koperInfo.lastName}
										</p>
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
												<img src={order.product.imageUrl || '/pictures/flower-test.avif'} className={'order-product-img'}/>
											</div>

											<div className={'flex flex-col flex-1 h-auto'}>
												<p className={'order-product-name'}>
													{order.product.name}
												</p>
												<span className={'order-product-description'}>
											  EA {formatEur(order.totalPrice / order.quantity)}
											</span>
											</div>
											<div className={'flex flex-col'}>
												<div className={'order-product-badge'}>
													Aantal {order.quantity}
												</div>
												<span className={'order-product-total'}>
												{formatEur(order.totalPrice)}
											</span>
											</div>
										</div>
									</div>
								</div>
							</div>
						</>
					)
				}
			</motion.div>
		</LayoutGroup>
	);
}

export default OrderDetails;