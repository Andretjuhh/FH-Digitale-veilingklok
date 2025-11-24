import { LocalizationTexts } from '../controllers/localization';

export interface InputField<T extends string = string> {
	label: LocalizationTexts;
	placeholder?: string;
	type: 'text' | 'email' | 'password' | 'select';
	options?: readonly T[];
	required?: boolean;
}
