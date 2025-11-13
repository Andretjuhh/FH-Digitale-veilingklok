import React from 'react';

// Internal imports
import CustomDropdown, {DropdownItem} from "./Dropdown";
import {SupportedLanguages} from "../../controllers/localization";
import clsx from "clsx";
import {joinClsx} from "../../utils/classPrefixer";

type Props = {
	className?: string;
}

function LanguagePicker({className}: Props) {
	const languages: DropdownItem<SupportedLanguages>[] = [
		{id: 'en', label: 'English', type: 'button', as: 'button'},
		{id: 'es', label: 'Español', type: 'button', as: 'button'},
		{id: 'fr', label: 'Français', type: 'button', as: 'button'},
		{id: 'nl', label: 'Nederlands', type: 'button', as: 'button'},
	];

	// Find the label of the current language
	const currentLanguageLabel = languages.find(lang => lang.id === window.application.languageCode)?.label || window.application.t('language')

	return (
		<CustomDropdown
			menuClassName={'rounded-xl'}
			buttonClassName={clsx('base-btn language-picker-btn app-header-s-btn-language', className)}
			itemButtonClassName={clsx('language-picker-item-btn', joinClsx(className, 'item-btn'))}
			buttonChildren={
				<>
					<i className={clsx("base-btn-icon btn-fill-anim-icon bi bi-globe-americas", joinClsx(className, 'icon'))}/>
					<span className={clsx("base-btn-txt btn-fill-anim-txt", joinClsx(className, 'txt'))}>
						{currentLanguageLabel}
					</span>
				</>
			}
			items={languages}
			onItemSelect={({id}) => window.application.changeLanguage(id)}
		/>

		// <Button
		// 	className={'app-home-s-btn app-header-s-btn-language  !bg-primary-main'}
		// 	label={window.application.languageCode.toUpperCase()}
		// 	icon={'bi-globe-americas'}
		// />
	);
}

export default LanguagePicker;