import React, {ReactNode, useState} from 'react';
import Dropdown from 'react-bootstrap/Dropdown';
import clsx from 'clsx';
import {joinClsx} from "../../utils/classPrefixer";

export interface DropdownItem<T extends string = string> {
	id: T;
	label: string;
	icon?: string;
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
	showOverlay?: boolean;

	// Item customization
	customItemRenderer?: (item: DropdownItem<T>) => ReactNode;
	onItemSelect?: (item: DropdownItem<T>) => void;

	// General
	id?: string;
	className?: string;
	disabled?: boolean;
}

const overlayStyle: React.CSSProperties = {
	position: 'fixed',
	top: 0,
	left: 0,
	width: '100vw',
	height: '100vh',
	backgroundColor: 'rgba(0, 0, 0, 0.3)',
	zIndex: 1050,
	inset: 0
};

function CustomDropdown<T extends string = string>({
	                                                   showOverlay = false,
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
	const [open, setOpen] = useState(false);


	const handleItemClick = (item: DropdownItem<T>) => {
		if (item.action) {
			item.action();
		}
		if (onItemSelect) {
			onItemSelect(item);
		}
	};

	return (
		<>
			{
				(open && showOverlay) && (
					<div
						className="dropdown-overlay"
						onClick={() => setOpen(false)}
						style={overlayStyle}
					/>
				)
			}

			<Dropdown
				className={className}
				show={open}
				onToggle={(isOpen) => setOpen(isOpen)}
			>
				<Dropdown.Toggle variant={buttonVariant} id={id} className={buttonClassName} disabled={disabled}>
					{buttonChildren || buttonLabel}
				</Dropdown.Toggle>

				<Dropdown.Menu
					align={menuAlignEnd ? 'end' : 'start'}
					className={clsx(menuClassName, 'w-full')}
					style={{zIndex: 1060}}
				>
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
								{
									item.icon &&
									<i className={
										clsx(joinClsx(itemButtonClassName, 'icon'),
											"bi", item.icon)}
									/>
								}
								{item.label}
							</Dropdown.Item>
						);
					})}
				</Dropdown.Menu>
			</Dropdown>
		</>
	);
}

export default CustomDropdown;
