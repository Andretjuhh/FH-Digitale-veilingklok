import React, { useState, useEffect } from 'react';
import Button from '../../buttons/Button';
import FormInputField from '../../elements/FormInputField';
import { getAccountInfo } from '../../../controllers/authentication';

function AccountSettings() {
	const [email, setEmail] = useState('user@example.com');
	const [adress, setAdress] = useState('123 Main St');
	const [region, setRegion] = useState('Den Haag');

	useEffect(() => {
		async function load() {
			const info = await getAccountInfo();
			setEmail(info.email);
			setAdress(info.adress);
			setRegion(info.region);
		}
		load();
	}, []);

	return (
		<div className="settings-content">
			<h1 className="text-2xl font-semibold">Account Settings</h1>
			<div className="settings-form">
				<div className="flex flex-col gap-1">
					<FormInputField id="emaill" label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
				</div>
				<div className="flex flex-col gap-1">
					<FormInputField id="password" label="Password" type="password" value={email} />
				</div>
				<div className="flex flex-col gap-1">
					<FormInputField id="adress" label="Adress" type="text" value={adress} onChange={(e) => setAdress(e.target.value)} />
				</div>
				<div className="flex flex-col gap-1">
					<FormInputField id="region" label="Region" type="text" value={region} onChange={(e) => setRegion(e.target.value)} />
				</div>
			</div>
		</div>
	);
}
export default AccountSettings;
