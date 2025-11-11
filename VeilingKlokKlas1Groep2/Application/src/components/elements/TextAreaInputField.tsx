import React from 'react';
import {joinClsx} from "../../utils/classPrefixer";
import clsx from "clsx";

type Props = {
	id: string;
	label: string;
	error: string;
	isError: boolean;
	className?: string;
	mainClassName?: string;
} & Omit<React.DetailedHTMLProps<React.TextareaHTMLAttributes<HTMLTextAreaElement>, HTMLTextAreaElement>, 'className' | 'id'>;

function TextAreaInputField(props: Props) {
	const {id, isError, label, error, mainClassName, className, ...inputProps} = props;

	return (
		<div className={clsx(mainClassName, className)}>
			<label
				htmlFor={id}
				className={clsx("input-field-label", joinClsx(className, 'input'))}
			>{label}
			</label>
			<textarea id={id} className={clsx("input-field")} {...inputProps}/>
			{
				isError && (
					<div className={clsx("input-field-error-feedback", joinClsx(className, 'error-feedback'))}>
						{error}
					</div>
				)}
		</div>
	);
}

export default TextAreaInputField;