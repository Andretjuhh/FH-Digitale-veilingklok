import React, {ReactNode} from 'react';
import Dropdown from 'react-bootstrap/Dropdown';
import clsx from 'clsx';

export interface DropdownItem<T extends string = string> {
	id: T;
	label: string;
	action?: () => void;
	href?: string;
	disabled?: boolean;
	divider?: boolean;
	header?: boolean;
	type?: 'button' | 'link';
	as?: React.ElementType;
}

export interface CustomDropdownProps<T extends string = string> {
	// Button customization
	buttonLabel?: string;
	buttonVariant?: string;
	buttonClassName?: string;
	buttonChildren?: ReactNode;
	itemButtonClassName?: string;

	// Menu customization
	items: DropdownItem<T>[];
	menuClassName?: string;
	menuAlignEnd?: boolean;

	// Item customization
	customItemRenderer?: (item: DropdownItem<T>) => ReactNode;
	onItemSelect?: (item: DropdownItem<T>) => void;

	// General
	id?: string;
	className?: string;
	disabled?: boolean;
}

function CustomDropdown<T extends string = string>({
	                                                   buttonLabel = 'Dropdown',
	                                                   buttonVariant = 'light',
	                                                   buttonClassName = '',
	                                                   buttonChildren,
	                                                   items,
	                                                   menuClassName = '',
	                                                   menuAlignEnd = false,
	                                                   customItemRenderer,
	                                                   onItemSelect,
	                                                   id = 'dropdown-custom',
	                                                   className = '',
	                                                   disabled = false,
	                                                   itemButtonClassName
                                                   }: CustomDropdownProps<T>) {
	const handleItemClick = (item: DropdownItem<T>) => {
		if (item.action) {
			item.action();
		}
		if (onItemSelect) {
			onItemSelect(item);
		}
	};

	return (
		<Dropdown className={className}>
			<Dropdown.Toggle variant={buttonVariant} id={id} className={buttonClassName} disabled={disabled}>
				{buttonChildren || buttonLabel}
			</Dropdown.Toggle>

			<Dropdown.Menu align={menuAlignEnd ? 'end' : 'start'} className={clsx(menuClassName, 'w-full')}>
				{items.map((item) => {
					// Divider
					if (item.divider) {
						return <Dropdown.Divider key={String(item.id)}/>;
					}

					// Header
					if (item.header) {
						return <Dropdown.Header key={String(item.id)}>{item.label}</Dropdown.Header>;
					}

					// Custom renderer
					if (customItemRenderer) {
						return (
							<div
								key={String(item.id)}
								onClick={() => !item.disabled && handleItemClick(item)}
								style={{cursor: item.disabled ? 'not-allowed' : 'pointer'}}
							>
								{customItemRenderer(item)}
							</div>
						);
					}

					// Default item
					return (
						<Dropdown.Item
							key={String(item.id)}
							href={item.href}
							onClick={() => handleItemClick(item)}
							disabled={item.disabled}
							as={item.as}
							type={item.type}
							className={itemButtonClassName}
						>
							{item.label}
						</Dropdown.Item>
					);
				})}
			</Dropdown.Menu>
		</Dropdown>
	);
}

export default CustomDropdown;
