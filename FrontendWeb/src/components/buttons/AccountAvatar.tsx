import React, {useCallback, useRef} from 'react';

// Internal imports
import CustomDropdown, {DropdownItem} from './Dropdown';
import clsx from 'clsx';
import {joinClsx} from '../../utils/classPrefixer';
import {useRootContext} from '../contexts/RootContext';
import {getRandomColorHSL} from "../../utils/standards";

type Props = {
	className?: string;
};

function AccountAvatar({className}: Props) {
	const {t, account, removeAuthentication, navigate} = useRootContext();
	const defaultColor = useRef<string>(getRandomColorHSL());
	const menuOptions = useRef<DropdownItem[]>([
		{id: 'logout', label: t("logout"), type: 'button', as: 'button'},
	]);

	const handleItemSelect = useCallback((item: DropdownItem) => {
			if (item.id === 'logout') {
				removeAuthentication();
				navigate('/', {replace: true});
			}
		}
		, []);

	return (
		<CustomDropdown
			className={'account-avatar-dropdown'}
			menuClassName={'account-avatar-menu'}
			buttonClassName={clsx('base-btn account-avatar-btn', className)}
			itemButtonClassName={clsx('account-avatar-item-btn', joinClsx(className, 'item-btn'))}
			buttonChildren={
				<>
					<div className={'account-avatar-round'} style={{'--random-color': defaultColor.current} as React.CSSProperties}>
						<h1 className={'account-initial-txt'}>{account?.firstName?.charAt(0)}</h1>
					</div>
					<div className={'account-avatar-info'}>
						<span className={'account-avatar-title'}>{account?.firstName} {account?.lastName}</span>
						<span className={'account-avatar-txt'}>{account?.accountType}</span>
					</div>
				</>
			}
			items={menuOptions.current}
			onItemSelect={handleItemSelect}
		/>

	);
}

export default React.memo(AccountAvatar);
