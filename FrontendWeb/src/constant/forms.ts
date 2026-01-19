import { AccountType } from '../declarations/enums/AccountTypes';
import { FieldOrGroup, InputField } from '../declarations/types/FormField';

export const Regions = ['Noord-Holland', 'Zuid-Holland', 'Utrecht', 'Gelderland', 'Overijssel', 'Limburg', 'Friesland', 'Drenthe', 'Flevoland', 'Groningen', 'Zeeland'] as const;
export const Country = ['Nederland'] as const;

export const RegisterSteps = {
	[AccountType.Koper]: [
		[
			{ label: 'first_name', type: 'text', placeholder: 'Steve', required: true, group: 'name' },
			{ label: 'last_name', type: 'text', placeholder: 'Jobs', required: true, group: 'name' },
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true, icon: 'envelope-fill' },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true, icon: 'lock-fill' },
		],
		[
			{ label: 'phonenumber', type: 'text', required: true, placeholder: '+31 6 12345678', group: 'contact' },
			{ label: 'country', type: 'select', options: Country, required: true, placeholder: 'Nederland', group: 'contact' },
			// {label: 'region', type: 'select', options: Regions, required: true, placeholder: 'Zuid-Holland', group: 'location'},
			{ label: 'region', type: 'select', placeholder: 'Select Region', required: false, icon: 'geo-alt-fill', group: 'location' },
			{ label: 'postcode', type: 'text', placeholder: '1234AB', required: true, group: 'location' },
			{ label: 'address', type: 'text', placeholder: 'Van der Valkstraat 123', required: true },
		],
	],
	[AccountType.Kweker]: [
		[
			{ label: 'company_name', type: 'text', placeholder: 'Example BV', required: true, group: 'company_details' },
			{ label: 'kvk_number', type: 'text', placeholder: '12345678', required: true, group: 'company_details' },
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true, icon: 'envelope-fill' },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true, icon: 'lock-fill' },
		],
		[
			{ label: 'first_name', type: 'text', placeholder: 'Steve', required: true, group: 'name' },
			{ label: 'last_name', type: 'text', placeholder: 'Jobs', required: true, group: 'name' },
			{ label: 'phonenumber', type: 'text', placeholder: '+31 6 12345678', required: true },
		],
		[
			{ label: 'country', type: 'select', options: Country, required: true, placeholder: 'Nederland' },
			// {label: 'region', type: 'select', options: [...Regions], required: true, placeholder: 'Zuid-Holland', group: 'location'},
			{ label: 'region', type: 'select', placeholder: 'Select Region', required: false, icon: 'geo-alt-fill', group: 'location' },
			{ label: 'postcode', type: 'text', placeholder: '1234AB', required: true, group: 'location' },
			{ label: 'address', type: 'text', placeholder: 'Van der Valkstraat 123', required: true },
		],
	],
	[AccountType.Veilingmeester]: [
		[
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true, icon: 'envelope-fill' },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true, icon: 'lock-fill' },
		],
		[
			// {label: 'region', type: 'select', options: [...Regions], required: true, placeholder: 'Zuid-Holland', group: 'location'},
			{ label: 'region', type: 'select', placeholder: 'Select Region', required: false, icon: 'geo-alt-fill', group: 'location' },
			{ label: 'country', type: 'select', options: Country, required: true, placeholder: 'Nederland' },
			{ label: 'authorisation_code', type: 'text', placeholder: '123456', required: true },
		],
	],
	[AccountType.Admin]: [
		[
			{ label: 'email', type: 'email', placeholder: '', required: true, icon: 'envelope-fill' },
			{ label: 'password', type: 'password', placeholder: '', required: true, icon: 'lock-fill' },
		],
	],
} as const satisfies Readonly<Record<AccountType, readonly (readonly InputField[])[]>>;

export const ProductFormFields: InputField[] = [
	{ label: 'product_name', type: 'text', placeholder: undefined, placeholderLocalizedKey: 'enter_product_name', required: true, icon: 'box-seam-fill' },
	{ label: 'product_description', type: 'textarea', placeholder: undefined, placeholderLocalizedKey: 'enter_product_description', required: true, rows: 4 },
	{ label: 'region', type: 'select', placeholder: 'Select Region', required: false, icon: 'geo-alt-fill' },
	{ label: 'minimum_price', type: 'number', placeholder: '0.00', required: true, icon: 'currency-euro', group: 'pricing', step: '0.01', min: 1 },
	{ label: 'stock_quantity', type: 'number', placeholder: '0', required: true, icon: 'inboxes-fill', group: 'pricing', min: 0 },
	{ label: 'product_dimension', type: 'text', placeholder: 'e.g., 10 x 20 x 5 cm', required: false, icon: 'rulers' },
];

export const EditProductPriceFormFields: InputField[] = [
	{ disabled: true, label: 'product_name', type: 'text', placeholder: undefined, placeholderLocalizedKey: 'enter_product_name', required: true, icon: 'box-seam-fill' },
	{ disabled: true, label: 'product_description', type: 'textarea', placeholder: undefined, placeholderLocalizedKey: 'enter_product_description', required: true, rows: 4 },
	{ disabled: true, label: 'region', type: 'text', placeholder: undefined, required: false, icon: 'geo-alt-fill' },
	{ disabled: true, label: 'minimum_price', type: 'number', placeholder: '0.00', required: true, icon: 'currency-euro', group: 'pricing', step: '0.01', min: 1 },
	{ disabled: false, label: 'product_veiling_start_price', type: 'number', placeholder: '0.00', required: true, icon: 'bi-stopwatch', group: 'pricing', step: '0.01', min: 1 },
	{ disabled: true, label: 'product_dimension', type: 'text', placeholder: 'e.g., 10 x 20 x 5 cm', required: false, icon: 'rulers' },
];

export const buildFieldLayout = (fields: ReadonlyArray<InputField>): FieldOrGroup[] => {
	const orderedItems: FieldOrGroup[] = [];
	let currentGroup: { name: string; fields: InputField[] } | null = null;

	const flushGroup = () => {
		if (!currentGroup) return;
		orderedItems.push({ type: 'group', groupName: currentGroup.name, fields: currentGroup.fields });
		currentGroup = null;
	};

	fields.forEach((field) => {
		if (field.group) {
			if (currentGroup?.name === field.group) {
				currentGroup.fields.push(field);
			} else {
				flushGroup();
				currentGroup = { name: field.group, fields: [field] };
			}
		} else {
			flushGroup();
			orderedItems.push({ type: 'field', field });
		}
	});

	flushGroup();

	return orderedItems;
};
