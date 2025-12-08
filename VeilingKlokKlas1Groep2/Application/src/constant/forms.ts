import { AccountType } from '../types/AccountTypes';
import { FieldOrGroup, InputField } from '../types/FormField';

export const Regions = ['Noord-Holland', 'Zuid-Holland', 'Utrecht', 'Gelderland', 'Overijssel', 'Limburg', 'Friesland', 'Drenthe', 'Flevoland', 'Groningen', 'Zeeland'] as const;

export const RegisterSteps = {
	[AccountType.Koper]: [
		[
			{ label: 'first_name', type: 'text', placeholder: 'Steve', required: true, group: 'name' },
			{ label: 'last_name', type: 'text', placeholder: 'Jobs', required: true, group: 'name' },
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true, icon: 'envelope-fill' },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true, icon: 'lock-fill' },
		],
		[
			{ label: 'region', type: 'select', options: [...Regions], required: false, placeholder: 'Den Haag', group: 'location' },
			{ label: 'postcode', type: 'text', placeholder: '1234AB', required: false, group: 'location' },
			{ label: 'address', type: 'text', placeholder: 'Van der Valkstraat 123', required: false },
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
			{ label: 'phonenumber', type: 'text', placeholder: '+31 6 12345678', required: false },
		],
		[
			{ label: 'region', type: 'select', options: [...Regions], required: false, placeholder: 'Den Haag', group: 'location' },
			{ label: 'postcode', type: 'text', placeholder: '1234AB', required: false, group: 'location' },
			{ label: 'address', type: 'text', placeholder: 'Van der Valkstraat 123', required: false },
		],
	],
	[AccountType.Veilingmeester]: [
		[
			{ label: 'email', type: 'email', placeholder: 'you@example.com', required: true, icon: 'envelope-fill' },
			{ label: 'password', type: 'password', placeholder: '••••••••', required: true, icon: 'lock-fill' },
		],
		[
			{ label: 'region', type: 'select', options: [...Regions], required: true, placeholder: undefined },
			{ label: 'authorisation_code', type: 'text', placeholder: '123456', required: true },
		],
	],
} as const satisfies Readonly<Record<AccountType, readonly (readonly InputField[])[]>>;

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
