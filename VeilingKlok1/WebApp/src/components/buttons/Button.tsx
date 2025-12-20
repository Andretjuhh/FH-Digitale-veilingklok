// External imports
import React from 'react';
import clsx from 'clsx';

// Internal imports
import { joinClsx } from '../../utils/classPrefixer';

type ButtonProps = {
	labelClassName?: string;
	label?: string;
	img?: string;
	icon?: string;
	imgAlt?: string;
} & React.ButtonHTMLAttributes<HTMLButtonElement>;

function Button(props: ButtonProps) {
	const { label, icon, img, imgAlt, className, labelClassName, ...rest } = props;
	return (
		<button className={clsx('base-btn', className)} {...rest}>
			{icon && <i className={clsx('base-btn-icon bi', joinClsx(className, 'icon'), icon)} />}
			{img && <img src={img} className={clsx('base-btn-img', joinClsx(className, 'img'))} alt={imgAlt} />}
			{label && <span className={clsx('base-btn-txt', joinClsx(className, 'txt'), labelClassName)}>{label}</span>}
		</button>
	);
}

export default Button;
