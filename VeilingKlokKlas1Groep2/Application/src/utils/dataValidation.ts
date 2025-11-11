import {ContactFormData, ContactFormValidationErrors} from "../types/ContactFormData";

export function isContactFormValid(formData: FormData): [boolean, ContactFormData | undefined, ContactFormValidationErrors | undefined] {
	const errors = {} as ContactFormValidationErrors;

	const firstName = formData.get('name')?.toString().trim() as string;
	const lastName = formData.get('lname')?.toString().trim() as string;
	const email = formData.get('email')?.toString().trim() as string;
	const message = formData.get('message')?.toString().trim() as string;
	const telephone = formData.get('phone')?.toString().trim() as string;

	// If any of these data is empty return the corresponding error
	if (!firstName) {
		errors.firstName = true;
	}
	if (!lastName) {
		errors.lastName = true;
	}
	if (!email) {
		errors.email = true;
	}
	if (!message) {
		errors.message = true;
	}
	if (!telephone) {
		errors.telephone = true;
	}

	// console.log("errors", errors);
	// console.log({
	// 	firstName,
	// 	lastName,
	// 	email,
	// 	message,
	// 	telephone,
	// });

	// If there are any errors, return them
	if (Object.keys(errors).length > 0) {
		return [false, undefined, errors];
	}

	// If no errors, return the form data
	return [
		true,
		{
			firstName,
			lastName,
			email,
			message,
			telephone,
		},
		undefined
	];
}