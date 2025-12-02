import { LocalizationTexts } from '../controllers/localization';

export interface InputField<T extends string = string> {
	label: LocalizationTexts;
	placeholder: string | undefined;
	type: 'text' | 'email' | 'password' | 'select';
	required: boolean;
	options?: readonly T[];
	group?: string; // Optional group identifier - fields with the same group value will be rendered in a horizontal flexbox
}

export type FieldOrGroup = { type: 'field'; field: InputField } | { type: 'group'; groupName: string; fields: InputField[] };
