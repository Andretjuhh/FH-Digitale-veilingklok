export interface ContactFormData {
	firstName: string;
	lastName: string;
	email: string;
	message: string;
	telephone?: string;
}

export type ContactFormValidationErrors = {
	firstName: boolean;
	lastName: boolean;
	email: boolean;
	message: boolean;
	telephone: boolean;
}