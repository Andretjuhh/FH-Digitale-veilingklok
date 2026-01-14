import React, {useCallback, useRef} from 'react';

// Internal imports
import CustomDropdown, {DropdownItem} from './Dropdown';
import clsx from 'clsx';
import {joinClsx} from '../../utils/classPrefixer';
import {useRootContext} from '../contexts/RootContext';
import {AccountType} from "../../declarations/enums/AccountTypes";
import {getRandomColor} from "../../utils/standards";

type Props = {
	className?: string;
};

function AccountAvatar({className}: Props) {
	const {t, account, removeAuthentication, navigate} = useRootContext();
	const defaultColor = useRef<string>(getRandomColor(`${account?.firstName} ${account?.lastName}`));

	const menuOptions = useRef<DropdownItem[]>([
		{id: 'logout', label: t("logout"), icon: 'bi-door-open-fill', type: 'button', as: 'button'},
		{id: 'manage-account', label: t("manage_account"), icon: 'bi-person-lines-fill', type: 'button', as: 'button'},
		{id: 'settings', label: t("settings"), icon: 'bi-gear-fill', type: 'button', as: 'button'},
	]);

	const getAvatarName = () => {
		if (account?.accountType == AccountType.Kweker) return account.companyName;
		else if (account?.accountType == AccountType.Koper) return `${account?.firstName} ${account?.lastName}`;
		else return account?.accountType;
	}
	const handleItemSelect = useCallback((item: DropdownItem) => {
		if (item.id === 'logout') {
			removeAuthentication();
			navigate('/', {replace: true});
		}
		if (item.id === 'settings') {
			navigate('/settings');
		}
		if (item.id === 'manage-account') {
			navigate('/settings?section=account');
		}
	}, [navigate, removeAuthentication]);

	return (
		<CustomDropdown
			className={'account-avatar-dropdown'}
			menuClassName={'account-avatar-menu'}
			buttonClassName={clsx('base-btn account-avatar-btn', className)}
			itemButtonClassName={clsx('account-avatar-item-btn', joinClsx(className, 'item-btn'))}
			buttonChildren={
				<>
					<div className={'account-avatar-round'} style={{backgroundColor: defaultColor.current} as React.CSSProperties}>
						<h1 className={'account-initial-txt'}>{getAvatarName()?.charAt(0)}</h1>
					</div>
					<div className={'account-avatar-info'}>
						<span
							className={'account-avatar-title'}>{getAvatarName()}</span>
						<span className={'account-avatar-txt'}>{account?.accountType == 'Veilingmeester' ? account?.region : account?.accountType}</span>
					</div>
				</>
			}
			items={menuOptions.current}
			onItemSelect={handleItemSelect}
		/>

	);
}

export default React.memo(AccountAvatar);
