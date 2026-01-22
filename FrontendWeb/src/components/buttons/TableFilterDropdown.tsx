import React from 'react';

// Internal imports
import CustomDropdown, { DropdownItem } from './Dropdown';
import clsx from 'clsx';
import { joinClsx } from '../../utils/classPrefixer';

type Props = {
	menuItems: { id: string | number | undefined; label: string }[];
	selectedItemId: string | number | undefined;
	onSelectItem: (id: string | number | undefined) => void;
	className?: string;
};

export default function TableFilterDropdown(props: Props) {
	const { menuItems, selectedItemId, onSelectItem, className } = props;
	const [option, setOption] = React.useState<string | number | undefined>(selectedItemId);

	const menu: DropdownItem<any>[] = menuItems.map((item) => ({
		id: item.id,
		label: item.label,
		type: 'button',
		as: 'button',
	}));

	return (
		<CustomDropdown
			className={' '}
			menuClassName={'language-picker-menu'}
			buttonClassName={clsx('base-btn app-table-filter-btn ', className)}
			itemButtonClassName={clsx('language-picker-item-btn', joinClsx(className, 'item-btn'))}
			buttonChildren={
				<>
					<i className={clsx('base-btn-icon bi-chevron-down', joinClsx('language-picker-item-btn', 'icon'))} />
					<span className={clsx('base-btn-txt ', joinClsx('language-picker-item-btn', 'txt'))}>
						{menu.find((item) => item.id === option)?.label}
					</span>
				</>
			}
			items={menu}
			onItemSelect={({ id }) => {
				setOption(id);
				onSelectItem(id);
			}}
		/>
	);
}
