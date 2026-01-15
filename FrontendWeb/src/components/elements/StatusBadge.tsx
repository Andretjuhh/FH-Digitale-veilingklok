import React from "react";
import {OrderStatus} from "../../declarations/enums/OrderStatus";
import {useRootContext} from "../contexts/RootContext";
import {getNormalizedOrderStatus, getNormalizedVeilingKlokStatus, getOrderStatusString, getVeilingKlokStatusString} from "../../utils/standards";
import {VeilingKlokStatus} from "../../declarations/enums/VeilingKlokStatus";

export const StatusBadge: React.FC<{ status: OrderStatus }> = ({status}) => {
	const {t} = useRootContext();
	const statusText = getOrderStatusString(status);
	const statusClass = statusText ? `app-table-status-${statusText.toLowerCase()}` : '';

	const getLabel = (s: OrderStatus) => {
		switch (s) {
			case OrderStatus.Open:
				return t('open');
			case OrderStatus.Processing:
				return t('processing');
			case OrderStatus.Processed:
				return t('processed');
			case OrderStatus.Delivered:
				return t('delivered');
			case OrderStatus.Cancelled:
				return t('cancelled');
			case OrderStatus.Returned:
				return t('returned');
			default:
				return s;
		}
	};

	return (
		<span className={`app-table-status-badge ${statusClass}`}>
			<span className="app-table-status-dot"></span>
			{getLabel(getNormalizedOrderStatus(status) ?? status)}
		</span>
	);
};


export const KlokStatusBadge: React.FC<{ status: VeilingKlokStatus | string }> = ({status}) => {
	const {t} = useRootContext();
	const statusText = getVeilingKlokStatusString(status);
	const statusClass = statusText ? `app-table-status-${statusText.toLowerCase()}` : '';

	const getLabel = (s: VeilingKlokStatus) => {
		switch (s) {
			case VeilingKlokStatus.Scheduled:
				return t('scheduled');
			case VeilingKlokStatus.Started:
				return t('started');
			case VeilingKlokStatus.Paused:
				return t('paused');
			case VeilingKlokStatus.Stopped:
				return t('stopped');
			case VeilingKlokStatus.Ended:
				return t('ended');
			default:
				return s;
		}
	};

	return (
		<span className={`app-table-status-badge ${statusClass}`}>
			<span className="app-table-status-dot"></span>
			{getLabel(getNormalizedVeilingKlokStatus(status) ?? status as any)}
		</span>
	);
};
