import React, { useState, useEffect } from 'react';
import Button from '../../buttons/Button';
import FormInputField from '../../elements/FormInputField';
import { getAccountInfo, updateAccountInfo } from '../../../controllers/authentication';
import { AccountType } from '../../../types/AccountTypes';

// Map account type to the fields they should see
const fieldsByAccountType: Record<string, string[]> = {
	Koper: ['email', 'adress', 'postCode', 'regio', 'telephone'],
	Kweker: ['email', 'name', 'adress', 'postCode', 'telephone', 'regio'],
	Veilingmeester: ['email', 'regio'],
};

// Optional: nicer labels for display
const fieldLabels: Record<string, string> = {
	email: 'Email',
	adress: 'Adress',
	postCode: 'PostCode',
	telephone: 'Telefoonnummer',
	name: 'Bedrijfsnaam',
	regio: 'Regio',
};

function AccountSettings() {
	const [accountType, setAccountType] = useState<AccountType>();
	const [formData, setFormData] = useState<Record<string, any>>({});
	const [editFields, setEditFields] = useState<Record<string, boolean>>({});

	// Load account info
	useEffect(() => {
		async function load() {
			const info = await getAccountInfo();

			// Provide default empty strings for missing fields
			const defaults: Record<string, any> = {
				email: '',
				adress: '',
				postCode: '',
				telephone: '',
				name: '',
				regio: '',
			};

			setFormData({ ...defaults, ...info }); // merge info with defaults
			setAccountType(info.accountType);

			// Initialize edit states
			const initialEdit: Record<string, boolean> = {};
			Object.keys({ ...defaults, ...info }).forEach((key) => (initialEdit[key] = false));
			setEditFields(initialEdit);
		}
		load();
	}, []);

	// Toggle individual field editing
	const toggleEdit = (field: string) => {
		setEditFields((prev) => ({ ...prev, [field]: !prev[field] }));
	};

	// Save changes (you can customize this to call your API)
	const handleSave = async () => {
		// Ask for confirmation before sending the request
		const confirmSave = window.confirm('Are you sure you want to save the changes?');
		if (!confirmSave) return; // user canceled

		try {
			// Include all fields from formData
			const updatedData = { ...formData };

			// Call API
			await updateAccountInfo(updatedData);

			// Reset edit states
			const resetEdit: Record<string, boolean> = {};
			Object.keys(editFields).forEach((key) => (resetEdit[key] = false));
			setEditFields(resetEdit);

			alert('Account updated!');
		} catch (error: any) {
			alert(`Failed to update account: ${error.message || error}`);
		}
	};

	return (
		<div className="settings-content">
			<h1 className="text-2xl font-semibold mb-4">Account Settings</h1>

			<div className="settings-form">
				{accountType &&
					fieldsByAccountType[accountType].map((field) => (
						<div key={field} className={`settings-input-field ${editFields[field] ? 'editing' : ''}`}>
							<FormInputField
								id={field}
								label={fieldLabels[field] || field}
								value={formData[field] || ''}
								onChange={(e) => setFormData({ ...formData, [field]: e.target.value })}
								disabled={!editFields[field]}
							/>
							<Button className="settings-edit-button" icon="bi bi-pencil" onClick={() => toggleEdit(field)} type="button" />
						</div>
					))}
			</div>

			<Button id="save" className="settings-save-button" label="Save" type="button" onClick={handleSave} />
		</div>
	);
}

export default AccountSettings;
