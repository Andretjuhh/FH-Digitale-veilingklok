// External imports
import React from 'react';
import clsx from "clsx";

// Internal imports
import {joinClsx} from "../../utils/classPrefixer";


type ButtonProps = {
	labelClassName?: string;
	label?: string;
	icon?: string;
} & React.ButtonHTMLAttributes<HTMLButtonElement>;

function Button(props: ButtonProps) {
	const {label, icon, className, labelClassName, ...rest} = props;
	return <button className={clsx('base-btn', className)} {...rest}>
		{icon && <i className={clsx("base-btn-icon", joinClsx(className, 'icon'), "bi", icon)}/>}
		{label && <span className={clsx('base-btn-txt', joinClsx(className, 'txt'), labelClassName)}>{label}</span>}
	</button>;
}

export default Button;
