import React from 'react';
import {joinClsx} from "../../utils/classPrefixer";
import clsx from "clsx";

type Props = {
	label: string;
	error: string;
	isError: boolean;
	className?: string;
	mainClassName?: string;
} & Omit<React.InputHTMLAttributes<HTMLInputElement>, 'className'>;

function FormInputField(props: Props) {
	const {isError, label, error, mainClassName, className, ...inputProps} = props;
	return (
		<div className={clsx(mainClassName, className)}>
			<label
				htmlFor={inputProps.id}
				className={clsx("input-field-label", joinClsx(className, 'input'))}
			>{label}
			</label>
			<input className={clsx("input-field")} {...inputProps}/>
			{
				isError && (
					<div className={clsx("input-field-error-feedback", joinClsx(className, 'error-feedback'))}>
						{error}
					</div>
				)}
		</div>
	);
}

export default FormInputField;