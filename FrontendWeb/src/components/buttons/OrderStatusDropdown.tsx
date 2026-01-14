import React, {useCallback, useState} from 'react';
import clsx from 'clsx';

// Internal imports
import CustomDropdown, {DropdownItem} from './Dropdown';
import {joinClsx} from '../../utils/classPrefixer';
import {useRootContext} from '../contexts/RootContext';
import {OrderStatus} from "../../declarations/enums/OrderStatus";

type Props = {
	className?: string;
	currentValue: OrderStatus;
	onChange?: (status: OrderStatus) => void;
};

function OrderStatusDropdown({className, currentValue, onChange}: Props) {
	const {t} = useRootContext();
	const [value, setValue] = useState<OrderStatus>(currentValue);

	// Update local state if prop changes
	React.useEffect(() => {
		setValue(currentValue);
	}, [currentValue]);

	const status: DropdownItem[] = [
		{id: '0', label: t('open'), type: 'button', as: 'button'},
		{id: '1', label: t('processing'), type: 'button', as: 'button'},
		{id: '2', label: t('processed'), type: 'button', as: 'button'},
		{id: '3', label: t('delivered'), type: 'button', as: 'button'},
		{id: '4', label: t('cancelled'), type: 'button', as: 'button'},
		{id: '5', label: t('returned'), type: 'button', as: 'button'},
	];

	// Helper to get numeric value even if input is string or enum key
	const getNumericStatus = (val: any): number => {
		if (typeof val === 'number') return val;
		const parsed = parseInt(val);
		if (!isNaN(parsed)) return parsed;
		// Handle potential string enum keys
		return (OrderStatus as any)[val] || 0;
	};
	const numericValue = getNumericStatus(value);

	// Filter statuses based on current value types
	const filteredStatus = status.filter((item) => parseInt(item.id) >= numericValue);

	const handleSelect = useCallback((id: string) => {
		const newStatus = parseInt(id) as OrderStatus;
		setValue(newStatus);
		if (onChange) {
			onChange(newStatus);
		}
	}, [onChange]);

	return (
		<CustomDropdown
			className={'order-status-dropdown'}
			menuClassName={'order-status-dropdown-menu'}
			buttonClassName={clsx('base-btn order-status-dropdown-btn', className)}
			itemButtonClassName={clsx('order-status-dropdown-item-btn', joinClsx(className, 'item-btn'))}
			buttonChildren={
				<>
					<i className={'base-btn-icon order-status-dropdown-item-btn-icon bi bi-pencil-square'}/>
				</>
			}
			items={filteredStatus}
			onItemSelect={({id}) => handleSelect(id)}
			showOverlay={true}
		/>
	);
}

export default OrderStatusDropdown;
