import React from 'react';

// Internal imports
import Button from "./Button";

function LanguagePicker() {
	return (
		<Button
			className={'app-home-s-btn app-header-s-btn-language  !bg-primary-main'}
			label={window.application.languageCode.toUpperCase()}
			icon={'bi-globe-americas'}
		/>
	);
}

export default LanguagePicker;