import React, { useState, useEffect } from 'react';
import Button from '../../buttons/Button';
import FormInputField from '../../elements/FormInputField';
import { getAccountInfo, updateAccountInfo } from '../../../controllers/authentication';

function AccountSettings() {
	const [email, setEmail] = useState('user@example.com');
	const [adress, setAdress] = useState('123 Main St');
	const [region, setRegion] = useState('Den Haag');
	// Individual edit states
	const [editEmail, setEditEmail] = useState(false);
	const [editPassword, setEditPassword] = useState(false);
	const [editAddress, setEditAddress] = useState(false);
	const [editRegion, setEditRegion] = useState(false);

	// Handlers
	const toggleEmail = () => setEditEmail(p => !p);
	const togglePassword = () => setEditPassword(p => !p);
	const toggleAddress = () => setEditAddress(p => !p);
	const toggleRegion = () => setEditRegion(p => !p);

	useEffect(() => {
		async function load() {
			const info = await getAccountInfo();
			setEmail(info.email);
			setAdress(info.adress);
			setRegion(info.region);
		}
		load();
	}, []);

	async function saveAccount() {
    const updatedData = {
        email,
        adress,
        region,
		

    };

    try {
        const updatedAccount = await updateAccountInfo(updatedData);

        // Update global app state if needed
        if (window.application) {
            window.application.account = updatedAccount;
        }

        // Turn off editing
        setEditEmail(false);
        setEditAddress(false);
        setEditRegion(false);

        alert("Account settings updated successfully!");

    } catch (err) {
        console.error("Error updating account settings:", err);
        alert("Failed to update account settings.");
    }
}



	return (
		<div className="settings-content">
			<h1 className="text-2xl font-semibold">Account Settings</h1>
			<div className="settings-form">
				<div className={`settings-input-field ${editEmail ? "editing" : ""}`}>
					<FormInputField id="email" label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} disabled={!editEmail} />
					<Button id="edit" className="settings-edit-button" icon="bi bi-pencil" onClick = {toggleEmail} type = "button"/>
				</div>
				<div className={`settings-input-field ${editPassword ? "editing" : ""}`}>
					<FormInputField id="password" label="Password" type="password" value={email} disabled={!editPassword}/>
					<Button id="edit" className="settings-edit-button" icon="bi bi-pencil" onClick = {togglePassword} type = "button"/>
				</div>
				<div className={`settings-input-field ${editAddress ? "editing" : ""}`}>
					<FormInputField id="adress" label="Adress" type="text" value={adress} onChange={(e) => setAdress(e.target.value)} disabled={!editAddress} />
					<Button id="edit" className="settings-edit-button" icon="bi bi-pencil" onClick = {toggleAddress} type = "button"/>
				</div>
				<div className={`settings-input-field ${editRegion ? "editing" : ""}`}>
					<FormInputField id="region" label="Region" type="text" value={region} onChange={(e) => setRegion(e.target.value)} disabled={!editRegion} />
					<Button id="edit" className="settings-edit-button" icon="bi bi-pencil" onClick = {toggleRegion} type = "button"/>
				</div>
			</div>
			<Button id="save" className="settings-save-button" label='Save' onClick={saveAccount} type='button'/>
		</div>
	);
}
export default AccountSettings;
