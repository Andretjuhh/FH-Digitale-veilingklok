import React from 'react';

interface FormTextareaFieldProps extends Omit<React.TextareaHTMLAttributes<HTMLTextAreaElement>, 'className'> {
	id: string;
	label: string | React.ReactNode;
	error?: string;
	isError?: boolean;
	className?: string;
	mainClassName?: string;
}

const FormTextareaField = React.forwardRef<HTMLTextAreaElement, FormTextareaFieldProps>((props, ref) => {
	const {id, isError, label, error, mainClassName, className, ...textareaProps} = props;

	return (
		<div className={mainClassName}>
			<label htmlFor={id} className="input-field-label">
				{label}
			</label>

			<div className="input-field-wrapper">
				<textarea id={id} ref={ref} className="textarea-field" {...textareaProps} />
			</div>

			{isError && error && <div className="input-field-error-feedback">{error}</div>}
		</div>
	);
});

FormTextareaField.displayName = 'FormTextareaField';

export default FormTextareaField;