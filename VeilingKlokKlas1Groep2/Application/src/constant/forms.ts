import { AccountType } from '../types/AccountTypes';
import { InputField } from '../types/FormField';

const Regions = ['Noord-Holland', 'Zuid-Holland', 'Utrecht', 'Gelderland', 'Overijssel', 'Limburg', 'Friesland', 'Drenthe', 'Flevoland', 'Groningen', 'Zeeland'] as const;

const RegisterSteps: Readonly<Record<AccountType, InputField[][]>> = {
	[AccountType.Koper]: [
		[
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true },
			{ label: 'first_name', type: 'text', placeholder: 'Steve', required: true },
			{ label: 'last_name', type: 'text', placeholder: 'Jobs' , required: true},
		],
		[
			{ label: 'address', type: 'text', placeholder: '' },
			{ label: 'postcode', type: 'text', placeholder: '1234 AB' },
			{ label: 'region', type: 'select', options: [...Regions] },
		],
	],
	[AccountType.Kweker]: [
		[
			{ label: 'company_name', type: 'text', placeholder: 'Example BV', required: true },
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true },
		],
		[{ label: 'kvk_number', type: 'text', placeholder: '12345678', required: true }],
		[
			{ label: 'phonenumber', type: 'text', placeholder: '+31 6 12345678' },
			{ label: 'address', type: 'text', placeholder: 'Street 123' },
			{ label: 'postcode', type: 'text', placeholder: '1234 AB' },
			{ label: 'region', type: 'select', options: [...Regions] },
		],
	],
	[AccountType.Veilingmeester]: [
		[
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true },
		],
		[
			{ label: 'region', type: 'select', options: [...Regions] },
			{ label: 'authorisation_code', type: 'text', placeholder: '123456', required: true },
		],
	],
};

export { RegisterSteps, Regions };
