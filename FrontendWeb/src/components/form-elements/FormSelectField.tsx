import React, { useEffect, useRef } from 'react';
import { joinClsx } from '../../utils/classPrefixer';
import clsx from 'clsx';

// Types
interface FormSelectFieldProps extends Omit<React.SelectHTMLAttributes<HTMLSelectElement>, 'className'> {
	id: string;
	label: string | React.ReactNode;
	error?: string;
	isError?: boolean;
	className?: string;
	mainClassName?: string;
	icon?: string;
	options?: Array<{ value: string; label: string }>;
}

// FormSelectField Component
const FormSelectField = React.forwardRef<HTMLSelectElement, FormSelectFieldProps>((props, ref) => {
	const { id, icon, isError, label, error, mainClassName, className, options, ...selectProps } = props;
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
				<select id={id} ref={ref} className={clsx('select-field', joinClsx(className, 'select-field'))} {...selectProps}>
					{options?.map((option) => (
						<option key={option.value} value={option.value}>
							{option.label}
						</option>
					))}
				</select>
			</div>

			{isError && error && <div className={clsx('input-field-error-feedback', joinClsx(className, 'error-feedback'))}>{error}</div>}
		</div>
	);
});

FormSelectField.displayName = 'FormSelectField';

FormSelectField.displayName = 'FormSelectField';
export default FormSelectField;
