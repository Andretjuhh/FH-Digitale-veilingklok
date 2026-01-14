import {LocalizationTexts} from '../../controllers/services/localization';

export interface InputField<T extends string = string> {
	icon?: string;
	label: LocalizationTexts;
	placeholder: string | undefined;
	placeholderLocalizedKey?: LocalizationTexts | undefined;  // Optional localization key for placeholder
	type: 'text' | 'email' | 'password' | 'select' | 'textarea' | 'number';
	required: boolean;
	options?: readonly T[];
	group?: string; // Optional group identifier - fields with the same group value will be rendered in a horizontal flexbox
	step?: string;
	min?: number;
	rows?: number;
	disabled?: boolean;
}

export type FieldOrGroup = { type: 'field'; field: InputField } | { type: 'group'; groupName: string; fields: InputField[] };
