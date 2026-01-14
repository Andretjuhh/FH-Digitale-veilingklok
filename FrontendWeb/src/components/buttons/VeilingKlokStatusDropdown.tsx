import React, {useCallback, useState} from 'react';

// Internal imports
import CustomDropdown, {DropdownItem} from './Dropdown';
import {useRootContext} from '../contexts/RootContext';
import {VeilingKlokStatus} from "../../declarations/enums/VeilingKlokStatus";

type Props = {
	className?: string;
	currentValue: VeilingKlokStatus;
	onChange?: (status: VeilingKlokStatus) => void;
};

function VeilingKlokStatusDropdown({className, currentValue, onChange}: Props) {
	const {t} = useRootContext();
	const [value, setValue] = useState<VeilingKlokStatus>(currentValue);

	// Update local state if prop changes
	React.useEffect(() => {
		setValue(currentValue);
	}, [currentValue]);

	const status: DropdownItem[] = [
		{id: '1', label: t('scheduled'), type: 'button', as: 'button'},
		{id: '2', label: t('started'), type: 'button', as: 'button'},
		{id: '3', label: t('paused'), type: 'button', as: 'button'},
		{id: '4', label: t('stopped'), type: 'button', as: 'button'},
		{id: '5', label: t('ended'), type: 'button', as: 'button'},
	];

	// Helper to get numeric value even if input is string or enum key
	const getNumericStatus = (val: any): number => {
		if (typeof val === 'number') return val;
		const parsed = parseInt(val);
		if (!isNaN(parsed)) return parsed;
		// Handle potential string enum keys
		return (VeilingKlokStatus as any)[val] || 0;
	};
	const numericValue = getNumericStatus(value);

	// Filter statuses based on current value types
	// For clocks, we generally move forward, but Paused (3) -> Started (2) is a valid transition that goes backward in ID.
	// So we might want to disable filtering or customize it.
	// For now, adhering to "similar component" request, I will include similar logic
	// but I will comment out the filter or make it permissive if it restricts valid flows.
	// Actually, let's keep all options available for the admin to correct mistakes,
	// or maybe the user wants strict flow. Given I lack specs, I will show all, or filter?
	// User said "similar component". OrderStatusDropdown filters.
	// But OrderStatus flow is strictly linear usually.
	// I'll leave the filter logic but maybe comment it out or adjust?
	// Let's remove the filter for now to be safe, as Clocks are more interactive.
	const filteredStatus = status; // .filter((item) => parseInt(item.id) >= numericValue);

	const handleSelect = useCallback((id: string) => {
		const newStatus = parseInt(id) as VeilingKlokStatus;
		setValue(newStatus);
		if (onChange) {
			onChange(newStatus);
		}
	}, [onChange]);

	const currentLabel = status.find((s) => s.id === numericValue.toString())?.label || numericValue.toString();

	return (
		<CustomDropdown
			className={className}
			buttonLabel={currentLabel}
			items={filteredStatus}
			onItemSelect={(item) => handleSelect(item.id)}
		/>
	);
}

export default VeilingKlokStatusDropdown;

