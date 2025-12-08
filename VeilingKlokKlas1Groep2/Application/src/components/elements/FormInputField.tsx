import React, { useRef, useEffect } from 'react';
import { joinClsx } from '../../utils/classPrefixer';
import clsx from 'clsx';

type Props = {
	id: string;
	label: string;
	error?: string;
	isError?: boolean;
	className?: string;
	mainClassName?: string;
	icon?: string;
} & Omit<React.InputHTMLAttributes<HTMLInputElement>, 'className' | 'id'>;

// ⬇️ MUST use forwardRef so RHF works correctly
const FormInputField = React.forwardRef<HTMLInputElement, Props>((props, ref) => {
	const { id, icon, isError, label, error, mainClassName, className, ...inputProps } = props;
	const iconRef = useRef<HTMLElement>(null);
	const wrapperRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		if (wrapperRef.current) {
			if (icon && iconRef.current) {
				const iconWidth = iconRef.current.offsetWidth;
				wrapperRef.current.style.setProperty('--icon-width', `${iconWidth}px`);
			}
		}
	}, [icon]);

	return (
		<div className={clsx(mainClassName, joinClsx(className, 'ptn'))}>
			<label htmlFor={id} className={clsx('input-field-label', joinClsx(className, 'input'))}>
				{label}
			</label>

			<div ref={wrapperRef} className={clsx('input-field-wrapper', joinClsx(className, 'wrapper'))}>
				{icon && <i ref={iconRef} className={clsx('input-field-icon bi', `bi-${icon}`, joinClsx(className, 'icon'))} />}
				{/* ⬇️ forward the ref and all RHF props */}
				<input id={id} ref={ref} className={clsx('input-field', joinClsx(className, 'input-field'))} {...inputProps} />
			</div>

			{isError && error && <div className={clsx('input-field-error-feedback', joinClsx(className, 'error-feedback'))}>{error}</div>}
		</div>
	);
});

// Give component a display name (React dev tools requirement)
FormInputField.displayName = 'FormInputField';

export default FormInputField;
